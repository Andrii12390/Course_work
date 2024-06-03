namespace Course_work
{
    internal class RotationMethod
    {
        private Matrix _matrix;


        public Matrix Matrix { get => _matrix; set => _matrix = value; }


        public RotationMethod(Matrix matrix)
        {
            Matrix = matrix ?? throw new ArgumentNullException($"{nameof(matrix)} cannot be null");
        }
        private double GetRotationAngle(List<List<double>> matrix, int i, int j)
        {
            if (matrix[i][i] - matrix[j][j] == 0)
            {
                return Math.PI / 4.0;
            }
            return 0.5 * Math.Atan(2 * matrix[i][j] / (matrix[i][i] - matrix[j][j]));
        }
        private List<List<double>> GetRotationMatrix(int size, int i, int j, double angle)
        {
            List<List<double>> unitMatrix = Matrix.GetUnitMatrix();
            unitMatrix[i][i] = Math.Cos(angle);
            unitMatrix[i][j] = -Math.Sin(angle);
            unitMatrix[j][i] = Math.Sin(angle);
            unitMatrix[j][j] = Math.Cos(angle);
            return unitMatrix;
        }
        private (int, int) GetLargestNonDiagonalElement(List<List<double>> matrix)
        {
            double maxAbs = Math.Abs(matrix[0][1]);
            int rowIndex = 0, columnIndex = 1;
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    Matrix.Iterations++;
                    if (i != j && Math.Abs(matrix[i][j]) > maxAbs)
                    {
                        maxAbs = Math.Abs(matrix[i][j]);
                        rowIndex = i;
                        columnIndex = j;
                    }
                }
            }
            return (rowIndex, columnIndex);
        }
        private double SumOfSquaresOfNotDiagonalElements(List<List<double>> matrix)
        {
            double sum = 0;
            for (int i = 0; i < matrix[0].Count; i++)
            {
                for (int j = 0; j < matrix[0].Count; j++)
                {
                    if (i != j)
                    {
                        sum += matrix[i][j] * matrix[i][j];
                    }
                }
            }
            return sum;
        }
        public (List<double>, List<Matrix>?) GetEigenvalues(double epsilon)
        {
            if (!Matrix.IsSymmetrical())
            {
                throw new ArgumentException("Matrix should be symmetrical");
            }
            else if (Matrix.IsDiagonal())
            {
                return (ExtractEigenvalues(Matrix.MatrixData, epsilon), null);
            }
            int iterations = 0;
            bool operationCondition = true;
            List<Matrix> rotationMatrices = new List<Matrix>();
            Matrix currentMatrix = new Matrix(Matrix.MatrixData);
            while (iterations < 1e4 && operationCondition)
            {
                (int i, int j) = GetLargestNonDiagonalElement(currentMatrix.MatrixData);
                double angle = GetRotationAngle(currentMatrix.MatrixData, i, j);
                Matrix rotationMatrix = new Matrix(GetRotationMatrix(currentMatrix.MatrixData[0].Count, i, j, angle));
                rotationMatrices.Add(rotationMatrix);
                Matrix transposedMatrix = new Matrix(rotationMatrix.GetTransposedMatrix());
                currentMatrix = (transposedMatrix.Multiply(currentMatrix, ref Matrix.RefIterations)).Multiply(rotationMatrix, ref Matrix.RefIterations);
                double sumOfSquares = SumOfSquaresOfNotDiagonalElements(currentMatrix.MatrixData);
                if (sumOfSquares < epsilon)
                {
                    operationCondition = false;
                }
                iterations++;
            }
            List<double> eigenvalues = ExtractEigenvalues(currentMatrix.MatrixData, epsilon);
            return (eigenvalues, rotationMatrices);
        }
        private List<double> ExtractEigenvalues(List<List<double>> matrix, double epsilon)
        {
            List<double> eigenvalues = new List<double>();
            int decimalPlaces = (int)Math.Ceiling(-Math.Log10(epsilon));
            for (int i = 0; i < matrix.Count; i++)
            {
                Matrix.Iterations++;
                eigenvalues.Add(Math.Round(matrix[i][i], decimalPlaces));
            }
            return eigenvalues;
        }
        private List<List<double>> ExtractEigenVectors(List<List<double>> matrix, double epsilon)
        {
            int decimalPlaces =  (int)Math.Ceiling(-Math.Log10(epsilon));
            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix.Count; j++)
                {
                    Matrix.Iterations++;
                    matrix[i][j] = Math.Round(matrix[i][j], decimalPlaces);
                }
            }
            return matrix;
        }
        public List<List<double>> GetEigenVectors(List<Matrix> rotationMatrixes, double epsilon)
        {
            if(Matrix.IsDiagonal())
            {
                var Vectors = ExtractEigenVectors(Matrix.GetTransposedMatrix(), epsilon);
                for (int i = 0;i < Vectors.Count;i++)
                {
                    if (Vectors[i][i] == 0)
                        Vectors[i][i] = 1;
                }
                return Vectors;
            }
            Matrix resultMatrix = rotationMatrixes[0];

            foreach (Matrix matrix in rotationMatrixes.Skip(1))
            {
                resultMatrix = resultMatrix.Multiply(matrix, ref Matrix.RefIterations);
            }
            Matrix eigenVectors = new Matrix(Matrix.GetEmptyMatrix(Matrix.MatrixData[0].Count));
            eigenVectors.MatrixData = ExtractEigenVectors(resultMatrix.MatrixData, epsilon);
            eigenVectors.MatrixData = eigenVectors.GetTransposedMatrix();
            return eigenVectors.MatrixData;
        }
    }
}
