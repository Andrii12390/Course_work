using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
namespace Course_work
{
    internal class DanilevskiyMethod
    {
        private Matrix _matrix;


        public Matrix Matrix { get => _matrix; set => _matrix = value; }


        public DanilevskiyMethod(Matrix matrix)
        {
            Matrix = matrix;
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
            for (int x = 0; x < matrixA.Count; x++)
            {
                if (row - 1 != x)
                    B[row - 1][x] = -matrixA[row][x] / matrixA[row][row - 1];
                else
                    B[row - 1][row - 1] = 1 / matrixA[row][row - 1];
                Matrix.Iterations++;
            }
            return B;
        }
        public (Matrix, List<Matrix>) GetNormalForm()
        {
            Matrix A = new Matrix(Matrix.MatrixData);
            List<Matrix> arrayB = new List<Matrix>();
            for (int i = A.MatrixData.Count - 1; i > 0; i--)
            {
                Matrix.Iterations++;
                if (A.MatrixData[i][i - 1] == 0)
                {
                    for (int j = 0; j < A.MatrixData.Count; j++)
                    {
                        double temp = A.MatrixData[j][i];
                        A.MatrixData[j][i] = A.MatrixData[j][i - 1];
                        A.MatrixData[j][i - 1] = temp;
                    }
                    List<double> tempRow = new List<double>(A.MatrixData[i]);
                    A.MatrixData[i] = new List<double>(A.MatrixData[i - 1]);
                    A.MatrixData[i - 1] = tempRow;
                }

                Matrix B = new Matrix(GetB(A.MatrixData, i));
                arrayB.Add(B);
                Matrix BReverse = new Matrix(FindInverseMatrix(B.MatrixData));
                A = BReverse * A * B;
            }
            return (A, arrayB);
        }
        public (List<double>, List<Matrix>, double[]) GetEigenValues()
        {
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
                    double realPart = root.Real;
                    if (realPart == 0 || Math.Abs(realPart) > 1e10)
                    {
                        throw new OverflowException("an overflow occurred when calculating roots.");
                    }
                    eigenValues.Add(realPart);
                }
            }
            if (eigenValues.Count == 0)
            {
                throw new Exception("There are no real solution");
            }
            Array.Reverse(coefficients);
            return (eigenValues, arrayB, coefficients);
        }
        public List<List<double>> GetEigenVectors(List<double> ownValues, List<Matrix> similarityMatrices)
        {
            Matrix similarityMatrix = similarityMatrices[0];
            for (int i = 1; i < similarityMatrices.Count; i++)
            {
                similarityMatrix = similarityMatrix * similarityMatrices[i];
            }
            Matrix ownVectors = new Matrix(Enumerable.Range(0, Matrix.MatrixData.Count).Select(k => ownValues.Select(val => Math.Pow(val, Matrix.MatrixData.Count - k - 1)).ToList()).ToList());
            List<List<double>> transposedVectors = ownVectors.GetTransposedMatrix();

            for (int i = 0; i < transposedVectors.Count; i++)
            {
                transposedVectors[i] = similarityMatrix * transposedVectors[i];
            }
            return transposedVectors;
        }
    }
}
