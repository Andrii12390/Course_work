using System.IO;
using System.Windows;

namespace Course_work
{
    internal class FileController
    {
        private List<List<double>> _matrix;
        private List<EigenPair> _eigenPairs;
        public List<List<double>> Matrix { get => _matrix; set => _matrix = value; }
        public List<EigenPair> EigenPairs { get => _eigenPairs; set => _eigenPairs = value; }
        public void SaveToFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && EigenPairs != null && EigenPairs.Count > 0 && filePath != "__________")
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Matrix");
                    foreach (List<double> row in Matrix)
                    {
                        writer.WriteLine(String.Join(" ", row));
                    }
                    foreach (EigenPair pair in EigenPairs)
                    {
                        writer.WriteLine($"Eigen value: {pair.EigenValue} | Eigen vector: [{pair.EigenVectorString}]");
                    }
                    writer.WriteLine(new string('-', 90));
                }
                MessageBox.Show("Data saved successfully.");
            }
            else
            {
                MessageBox.Show("No data to save or you didn't select file path.");
            }
        }
    }
}