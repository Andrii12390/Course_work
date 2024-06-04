using System.Numerics;
using System.Windows;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
namespace Course_work
{
    internal class DanilevskiyMethod
    {
        private Matrix _matrix;


        public Matrix Matrix { get => _matrix; set => _matrix = value; }
        private int index1 = 0, index2 = 0;

        public DanilevskiyMethod(Matrix matrix)
        {
            Matrix = matrix ?? throw new ArgumentNullException($"{nameof(matrix)} cannot be null");
        }
        private List<List<double>> FindInverseMatrix(List<List<double>> matrixToInvert)
        {
            Matrix<double> convertedMatrix = Matrix<double>.Build.DenseOfRows(matrixToInvert);
            Matrix<double> invertedMatrix = convertedMatrix.Inverse();
            List<List<double>> invertedMatrixList = new List<List<double>>();
            for (int i = 0; i < invertedMatrix.RowCount; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < invertedMatrix.ColumnCount; j++)
                {
                    row.Add(invertedMatrix[i, j]);
                    Matrix.Iterations++;
                }
                invertedMatrixList.Add(row);
            }
            return invertedMatrixList;
        }
        private List<List<double>> GetB(List<List<double>> matrixA, int row)
        {
            List<List<double>> B = Matrix.GetUnitMatrix();
            int size = matrixA.Count;

            for (int col = 0; col < size; col++)
            {
                B[row - 1][col] = (row - 1 == col) ? 1.0 / matrixA[row][row - 1] : -matrixA[row][col] / matrixA[row][row - 1];
            }

            return B;
        }
        public (Matrix, List<Matrix>) GetNormalForm()
        {
            Matrix A = new Matrix(Matrix.MatrixData);
            List<Matrix> arrayB = new List<Matrix>();
            int size = A.MatrixData.Count;

            for (int i = size - 1; i > 0; i--)
            {
                if (A.MatrixData[i][i - 1] == 0)
                {
                    A.MatrixData = SwapColumnsAndRows(A, i-1, i-2);
                    index1 = i - 1;
                    index2 = i - 2;
                }
                Matrix B = new Matrix(GetB(A.MatrixData, i));
                ChechDeterminant(B.MatrixData);
                arrayB.Add(B);
                Matrix BInverse = new Matrix(FindInverseMatrix(B.MatrixData));
                A = BInverse.Multiply(A, ref Matrix.RefIterations).Multiply(B, ref Matrix.RefIterations);
            }
            return (A, arrayB);
        }
        private void ChechDeterminant(List<List<double>> matrix)
        {
            DenseMatrix dense = DenseMatrix.OfColumnArrays(matrix.Select(row => row.ToArray()).ToArray());
            double determinant = dense.Determinant();
            if (Math.Abs(determinant) < 1e-30 || determinant == 0)
            {
                throw new ArgumentException("Determinant of the matrix is ​​zero, program cannot continue the calculation");
            }
        }
        private List<List<double>> SwapColumnsAndRows(Matrix matrix, int col1, int col2)
        {
            for (int i = 0; i < matrix.MatrixData.Count; i++)
            {
                double temp = matrix.MatrixData[i][col1];
                matrix.MatrixData[i][col1] = matrix.MatrixData[i][col2];
                matrix.MatrixData[i][col2] = temp;
            }
            for (int j = 0; j < matrix.MatrixData.Count; j++)
            {
                double temp = matrix.MatrixData[col1][j];
                matrix.MatrixData[col1][j] = matrix.MatrixData[col2][j];
                matrix.MatrixData[col2][j] = temp;
            }
            return matrix.MatrixData;
        }
        public (List<double>, List<Matrix>?, double[]?) GetEigenValues()
        {
            ChechDeterminant(Matrix.MatrixData);
            if (Matrix.IsDiagonal())
            {
                return (Matrix.MatrixData.Select((row, index) => row[index]).ToList(), null, null);
            }
            var (coefficientsMatrix, arrayB) = GetNormalForm();
            List<double> polynomialCoefficients = coefficientsMatrix.MatrixData[0];
            double[] coefficients = new double[polynomialCoefficients.Count + 1];
            coefficients[0] = 1;
            for (int i = 0; i < polynomialCoefficients.Count; i++)
            {
                Matrix.Iterations++;
                coefficients[i + 1] = -polynomialCoefficients[i];
            }
            Array.Reverse(coefficients);
            Polynomial poly = new Polynomial(coefficients);
            Complex[] roots = poly.Roots();
            List<double> eigenValues = new List<double>();
            foreach (var root in roots)
            {
                Matrix.Iterations++;
                if (root.Imaginary == 0)
                {
                    if (Double.IsInfinity(poly.Evaluate(root.Real)) || Math.Abs(root.Real) > 1e25 || (root.Real == 0 && coefficients[0] != 0))
                    {
                        throw new OverflowException("An overflow occurred when calculating roots.");
                    }
                    eigenValues.Add(root.Real);
                }
            }
            if (eigenValues.Count == 0)
            {
                throw new Exception("There are no real solution");
            }
            Array.Reverse(coefficients);
            return (eigenValues, arrayB, coefficients);
        }
        public List<List<double>> GetEigenVectors(List<double> eigenValues, List<Matrix> similarityMatrices)
        {
            if (Matrix.IsDiagonal())
            {
                var eigenVectors = Matrix.GetTransposedMatrix(ref Matrix.RefIterations);
                for(int i = 0; i < eigenValues.Count; i++)
                {
                    if (eigenVectors[i][i] == 0)
                        eigenVectors[i][i] = 1;
                }
                return eigenVectors;
            }
            Matrix similarityMatrix = similarityMatrices[0];
            for (int i = 1; i < similarityMatrices.Count; i++)
            {
                similarityMatrix = similarityMatrix.Multiply(similarityMatrices[i], ref Matrix.RefIterations);
            }
            Matrix ownVectors = new Matrix(Enumerable.Range(0, Matrix.MatrixData.Count).Select(k => eigenValues.Select(val => Math.Pow(val, Matrix.MatrixData.Count - k - 1)).ToList()).ToList());
            List<List<double>> transposedVectors = ownVectors.GetTransposedMatrix(ref Matrix.RefIterations);

            for (int i = 0; i < transposedVectors.Count; i++)
            {
                transposedVectors[i] = similarityMatrix.MultiplyByVector(transposedVectors[i], ref Matrix.RefIterations);
            }
            if(index1 != 0 && index2 != 0)
            {
                for (int i = 0; i < transposedVectors.Count; i++)
                {
                    (transposedVectors[i][index1], transposedVectors[i][index2]) = (transposedVectors[i][index2],transposedVectors[i][index1]); 
                }
            }
            return transposedVectors;
        }
    }
}
