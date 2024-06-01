namespace Course_work
{
    internal class Matrix
    {
        private List<List<double>> _matrix;
        private int _iterations;


        public int Iterations { get => _iterations; set => _iterations = value; }
        public List<List<double>> MatrixData { get => _matrix; set => _matrix = value; }

        public ref int RefIterations
        {
            get => ref _iterations;  
        }
        public Matrix(List<List<double>> matrix)
        {
            MatrixData = matrix;
        }
        public Matrix(int size)
        {
            MatrixData = GetEmptyMatrix(size);
        }
        public static List<List<double>> GetEmptyMatrix(int size)
        {
            List<List<double>> matrix = new List<List<double>>(size);
            for (int i = 0; i < size; i++)
            {
                matrix.Add(new List<double>(new double[size]));
            }
            return matrix;
        }
        public List<List<double>> GetEmptyMatrix(int rows, int cols)
        {
            List<List<double>> emptyMatrix = new List<List<double>>();
            for (int i = 0; i < rows; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    row.Add(0);
                    Iterations++;
                }
                emptyMatrix.Add(row);
            }
            return emptyMatrix;
        }
        public List<List<double>> GetUnitMatrix()
        {
            List<List<double>> unitMatrix = GetEmptyMatrix(MatrixData.Count);
            for (int i = 0; i < MatrixData[0].Count; i++)
            {
                unitMatrix[i][i] = 1;
                Iterations++;
            }
            return unitMatrix;
        }
        public bool IsSymmetrical()
        {
            for (int i = 0; i < MatrixData[0].Count; i++)
            {
                for (int j = 0; j < MatrixData[0].Count; j++)
                {
                    Iterations++;
                    if (MatrixData[i][j] != MatrixData[j][i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool IsDiagonal()
        {
            int num = 0;
            for (int i = 0; i < MatrixData[0].Count; i++)
            {
                for (int j = 0; j < MatrixData[0].Count; j++)
                {
                    Iterations++;
                    if (i != j && MatrixData[i][j] == 0)
                    {
                        num++;
                    }
                }
            }
            return num == MatrixData[0].Count * MatrixData[0].Count - MatrixData[0].Count;
        }
        public List<List<double>> GetTransposedMatrix()
        {
            List<List<double>> transposedMatrix = GetEmptyMatrix(MatrixData[0].Count, MatrixData.Count);
            for (int i = 0; i < MatrixData.Count; i++)
            {
                for (int j = 0; j < MatrixData[i].Count; j++)
                {
                    Iterations++;
                    transposedMatrix[j][i] = MatrixData[i][j];
                }
            }
            return transposedMatrix;
        }

        public Matrix Multiply(Matrix matrixB, ref int iterations)
        {
            List<List<double>> resultMatrix = GetEmptyMatrix(matrixB.MatrixData.Count);

            for (int i = 0; i < matrixB.MatrixData.Count; i++)
            {
                for (int j = 0; j < matrixB.MatrixData.Count; j++)
                {
                    for (int k = 0; k < matrixB.MatrixData.Count; k++)
                    {
                        resultMatrix[i][j] += MatrixData[i][k] * matrixB.MatrixData[k][j];
                        iterations++;
                    }
                }
            }
            return new Matrix(resultMatrix);
        }
        public static List<double> operator *(Matrix matrix, List<double> vector)
        {
            int size = matrix.MatrixData.Count;
            List<double> resultVector = new List<double>(new double[size]);

            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j < matrix.MatrixData[0].Count; j++)
                {
                    sum += matrix.MatrixData[i][j] * vector[j];
                    matrix.Iterations++;
                }
                resultVector[i] = sum;
            }

            return resultVector;
        }
    }
}
