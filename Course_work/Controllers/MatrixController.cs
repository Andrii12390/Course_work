using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
public enum Method
{
    Danilevskiy,
    Rotation
}
namespace Course_work
{
    internal class MatrixController
    {
        private Matrix _matrix;
        private int _iterations;
        private List<double> _eigenValues;
        private List<List<double>> _eigenVectors;
        private double[] _polynomialCoefficients;


        public Matrix Matrix { get => _matrix; set => _matrix = value; }
        public int Iterations { get => _iterations; set => _iterations = value; }
        public List<double> EigenValues { get => _eigenValues; set => _eigenValues = value; }
        public List<List<double>> EigenVectors { get => _eigenVectors; set => _eigenVectors = value; }
        public double[] PolynomialCoefficients { get => _polynomialCoefficients; set => _polynomialCoefficients = value; }


        public MatrixController(int size)
        {
            Matrix = new Matrix(size);
        }
        public void ResetIterations() => Matrix.Iterations = 0;
        public void GenerateMatrixTextBoxes(Grid matrixGrid)
        {
            matrixGrid.Children.Clear();
            matrixGrid.RowDefinitions.Clear();
            matrixGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < Matrix.MatrixData[0].Count; i++)
            {
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < Matrix.MatrixData[0].Count; i++)
            {
                matrixGrid.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < Matrix.MatrixData[0].Count; j++)
                {
                    AddMatrixLabel(matrixGrid, i, j);
                    AddMatrixTextBox(matrixGrid, i, j);
                }
            }
        }
        private void AddMatrixLabel(Grid grid, int row, int column)
        {
            Label label = new Label
            {
                Content = $"A{row + 1}{column + 1}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column * 2);
            grid.Children.Add(label);
        }
        private void AddMatrixTextBox(Grid grid, int row, int column)
        {
            TextBox textBox = new TextBox
            {
                Text = "0",
                TextAlignment = TextAlignment.Center,
                MaxLength = 14,
                Width = 50,
                Height = 23,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            Grid.SetRow(textBox, row);
            Grid.SetColumn(textBox, column * 2 + 1);
            grid.Children.Add(textBox);
            TrackTextBoxChanges(textBox, row, column);
        }
        public void CalculateDanilevskiy()
        {
            try
            {
                ResetIterations();
                DanilevskiyMethod danilevskiyMethod = new DanilevskiyMethod(Matrix);
                (EigenValues, List<Matrix> similarityMatrices, double[] polyCoeffs) = danilevskiyMethod.GetEigenValues();
                PolynomialCoefficients = polyCoeffs;
                EigenVectors = danilevskiyMethod.GetEigenVectors(EigenValues, similarityMatrices);
                Iterations = danilevskiyMethod.Matrix.Iterations;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Danilevskiy method calculation: {ex.Message}");
            }
        }
        public void CalculateRotation(double tolerance)
        {
            try
            {
                ResetIterations();
                RotationMethod rotationMethod = new RotationMethod(Matrix);
                (EigenValues, List<Matrix> rotationMatrices) = rotationMethod.GetEigenvalues(tolerance);
                EigenVectors = rotationMethod.GetEigenVectors(rotationMatrices, tolerance);
                Iterations = rotationMethod.Matrix.Iterations;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Rotation method calculation: {ex.Message}");
            }
        }
        public void GenerateRandomMatrix(Grid matrixGrid, Method method)
        {
            try
            {
                if (method == Method.Danilevskiy) { GenerateRandomMatrix(matrixGrid); }
                else { GenerateSymmetricMatrix(matrixGrid); }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while generating matrix"); return;
            }
        }
        private void GenerateRandomMatrix(Grid matrixGrid)
        {
            Random random = new Random();
            for (int i = 0; i < Matrix.MatrixData.Count; i++)
            {
                for (int j = 0; j < Matrix.MatrixData[0].Count; j++)
                {
                    double value = random.NextDouble() * 100;
                    Matrix.MatrixData[i][j] = value;
                    TextBox textBox = GetTextBoxAtGridPosition(i, j, matrixGrid);
                    if (textBox != null) textBox.Text = $"{value:F2}";
                }
            }
        }
        private void GenerateSymmetricMatrix(Grid matrixGrid)
        {
            Random random = new Random();
            for (int i = 0; i < Matrix.MatrixData.Count; i++)
            {
                for (int j = i; j < Matrix.MatrixData.Count; j++)
                {
                    double value = random.NextDouble() * 100;
                    Matrix.MatrixData[i][j] = value;
                    Matrix.MatrixData[j][i] = value;
                    TextBox textBox1 = GetTextBoxAtGridPosition(i, j, matrixGrid);
                    if (textBox1 != null) textBox1.Text = $"{value:F2}";
                    if (i != j)
                    {
                        TextBox textBox2 = GetTextBoxAtGridPosition(j, i, matrixGrid);
                        if (textBox2 != null) textBox2.Text = $"{value:F2}";
                    }
                }
            }
        }
        private TextBox GetTextBoxAtGridPosition(int row, int column, Grid matrixGrid)
        {
            return matrixGrid.Children.OfType<TextBox>().FirstOrDefault(tb => Grid.GetRow(tb) == row && Grid.GetColumn(tb) == GetTextBoxColumn(column));
        }
        private int GetTextBoxColumn(int column) => column * 2 + 1;
        public bool ValidateMatrixData(Grid matrixGrid)
        {
            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox)
                {
                    TextBox? currentTextBox = element as TextBox;
                    if (currentTextBox == null || string.IsNullOrEmpty(currentTextBox.Text) || !IsValidDouble(currentTextBox.Text))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool IsValidDouble(string input, double min = -10000.0, double max = 10000.0, int decimalPart = 5)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a valid real number.");
                return false;
            }
            if (input.Contains(' '))
            {
                MessageBox.Show("Enter a real number without spaces");
                return false;
            }
            if (double.TryParse(input, out double value))
            {
                if (double.IsInfinity(value))
                {
                    MessageBox.Show($"Value should be in range[{min}, {max}]");
                    return false;
                }
                string[] parts = input.Split('.');
                if (parts.Length > 1 && parts[1].Length > decimalPart)
                {
                    MessageBox.Show($"Max decimal part is {decimalPart} symbols");
                    return false;
                }
                if (value >= min && value <= max)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show($"Value should be in range[{min}, {max}]");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid real number.");
                return false;
            }
        }
        private void TrackTextBoxChanges(TextBox textBox, int rowIndex, int columnIndex)
        {
            textBox.LostFocus += (sender, e) =>
            {
                if (IsValidDouble(textBox.Text))
                {
                    Matrix.MatrixData[rowIndex][columnIndex] = double.Parse(textBox.Text);
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
                else
                {
                    textBox.BorderBrush = Brushes.Red;
                }
            };
        }
        public Matrix GetMatrix() => Matrix;
        public void ClearMatrixData(Grid matrixGrid)
        {
            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    textBox.Text = "0";
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
            }
        }

    }
}

