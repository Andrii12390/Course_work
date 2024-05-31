using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
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
        public void GenerateRandomMatrix(Grid matrixGrid)
        {
            Random random = new Random();
            int rowCount = Matrix.MatrixData.Count;
            int columnCount = Matrix.MatrixData[0].Count;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    double value = (random.NextDouble() * 100);
                    Matrix.MatrixData[i][j] = value;
                    TextBox textBox = matrixGrid.Children.OfType<TextBox>().FirstOrDefault(tb => Grid.GetRow(tb) == i && Grid.GetColumn(tb) == j * 2 + 1);
                    textBox.Text = $"{value:F2}";
                }
            }
        }
        public bool ValidateMatrixData(Grid matrixGrid)
        {
            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox)
                {
                    TextBox currentTextBox = element as TextBox;
                    string input = currentTextBox.Text.Trim();
                    if (string.IsNullOrEmpty(input) || !IsValidDouble(input))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool IsValidDouble(string input)
        {
            double min = -10000, max = 10000;
            int decimalPart = 5;
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
                MessageBox.Show("Enter a real number");
                return false;
            }
        }
        private void TrackTextBoxChanges(TextBox textBox, int rowIndex, int columnIndex)
        {
            textBox.LostFocus += (sender, e) =>
            {
                string input = textBox.Text;
                if (!input.Contains(" ") && IsValidDouble(input))
                {
                    Matrix.MatrixData[rowIndex][columnIndex] = double.Parse(input);
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                }
                else
                {
                    textBox.BorderBrush = Brushes.Red;
                }
            };
        }
        public Matrix GetMatrix()
        {
            return Matrix;
        }
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
