using Course_work;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

public enum Method
{
    Danilevskiy,
    Rotation
}
namespace Course_work
{
    public partial class MainWindow : Window
    {
        private MatrixController _matrixController;
        private GraphController _graphController;
        private FileController _fileController = new FileController();
        private List<EigenPair> _eigenPairs;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void matrixSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Content.ToString(), out int size))
                {
                    _matrixController = new MatrixController(size);
                    _matrixController.generateMatrixTextBoxes(MatrixGrid);
                }
                else
                {
                    MessageBox.Show("Invalid matrix size");
                }
            }
        }
        private void exitButton(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void calculateButton(object sender, RoutedEventArgs e)
        {
            Method method = SelectedMethod.SelectedIndex == 0 ? Method.Danilevskiy : Method.Rotation;
            if (_matrixController == null || !_matrixController.validateMatrixData(MatrixGrid))
            {
                MessageBox.Show("Please enter valid decimal values in all matrix cells.");
                return;
            }
            if (SelectedMethod.SelectedIndex == -1)
            {
                MessageBox.Show("You should select the method");
                return;
            }
            try
            {
                switch (method)
                {
                    case Method.Danilevskiy:
                        _matrixController.CalculateDanilevskiy();
                        _graphController = new GraphController(_matrixController.PolynomialCoefficients, _matrixController.EigenValues.Min(),
                                                          _matrixController.EigenValues.Max(), _matrixController.EigenValues.ToArray());
                        break;
                    case Method.Rotation:
                        double tolerance = double.Parse((SelectedTolerance.SelectedItem as ComboBoxItem)?.Content.ToString());
                        _matrixController.CalculateRotation(tolerance);
                        break;
                    default:
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
        }

        private void buttonClear(object sender, RoutedEventArgs e)
        {
            _matrixController.Matrix = new Matrix(_matrixController.Matrix.MatrixData.Count);
            _matrixController?.clearMatrixData(MatrixGrid);
            plotView.Visibility = Visibility.Collapsed;
            EigenDataGrid.ItemsSource = null;
        }

        private void buttonSave(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = SelectedFile.Text;
            _fileController.saveToFile(selectedFilePath, _eigenPairs, _matrixController.Matrix.MatrixData);
        }
        private void GenerateMatrixButton(object sender, RoutedEventArgs e)
        {
            _matrixController.generateRandomMatrix(MatrixGrid);
        }
        private void ButtonShowComplexity(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Number of iterations: {_matrixController.Iterations}\n" +
                            $"Calculation time: {_matrixController.CalculationTime} ms");
        }
        private void buttonSelectFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                SelectedFile.Text = saveFileDialog.FileName;
            }
        }

        private void buildGraphButton(object sender, RoutedEventArgs e)
        {
            try
            {
                plotView.Visibility = Visibility.Visible;
                plotView.Model = _graphController?.buildGraph();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}