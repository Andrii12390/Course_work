using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;


namespace Course_work
{
    public partial class MainWindow : Window
    {
        private MatrixController _matrixController;
        private GraphController? _graphController;
        private FileController _fileController = new FileController();
        private List<EigenPair> _eigenPairs;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnMatrixSizeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int size))
                {
                    _matrixController = new MatrixController(size);
                    _matrixController.GenerateMatrixTextBoxes(MatrixGrid);
                }
                else
                {
                    MessageBox.Show("Invalid matrix size");
                }
            }
        }
        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void OnCalculateButtonClick(object sender, RoutedEventArgs e)
        {
            Method method = SelectedMethod.SelectedIndex == 0 ? Method.Rotation : Method.Danilevskiy;
            if (_matrixController == null || !_matrixController.ValidateMatrixData(MatrixGrid))
            {
                MessageBox.Show("Please enter valid decimal values in all matrix cells."); return;
            }
            else if (SelectedMethod.SelectedIndex == -1)
            {
                MessageBox.Show("You should select the method"); return;
            }
            try
            {
                switch (method)
                {
                    case Method.Danilevskiy:
                        _matrixController?.CalculateDanilevskiy();
                        _graphController = new GraphController(_matrixController.PolynomialCoefficients, _matrixController.EigenValues.Min(),
                                                          _matrixController.EigenValues.Max(), _matrixController.EigenValues.ToArray());
                        break;
                    case Method.Rotation:
                        _matrixController?.CalculateRotation(double.Parse((SelectedTolerance.SelectedItem as ComboBoxItem)?.Content.ToString()));
                        break;
                    default:
                        MessageBox.Show("An enxpected error occured while calculating");
                        break;
                }
                _eigenPairs = _matrixController.EigenValues.Select((value, index) => new EigenPair
                {
                    EigenValue = value,
                    EigenVector = _matrixController.EigenVectors[index].ToArray()
                }).ToList();

                EigenDataGrid.ItemsSource = _eigenPairs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            _fileController.EigenPairs = _eigenPairs;
            _fileController.Matrix = _matrixController.Matrix.MatrixData;
        }
        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            _matrixController.Matrix = new Matrix(_matrixController.Matrix.MatrixData.Count);
            _matrixController?.ClearMatrixData(MatrixGrid);
            plotView.Visibility = Visibility.Collapsed;
            EigenDataGrid.ItemsSource = null;
        }
        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            _fileController.SaveToFile(SelectedFile.Text);
        }
        private void OnGenerateMatrixButtonClick(object sender, RoutedEventArgs e)
        {
            _matrixController.GenerateRandomMatrix(MatrixGrid, SelectedMethod.SelectedIndex == 0 ? Method.Rotation : Method.Danilevskiy);
        }
        private void OnShowComplexityButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Number of iterations: {_matrixController.Iterations}");
        }
        private void OnSelectFileButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                SelectedFile.Text = saveFileDialog.FileName;
            }
        }
        private void OnBuildGraphButtonClick(object sender, RoutedEventArgs e)
        {
            plotView.Visibility = Visibility.Visible;
            plotView.Model = _graphController?.BuildGraph();
        }
    }
}