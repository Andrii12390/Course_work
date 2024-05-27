using System.IO;
using System.Windows;

namespace Course_work
{
    internal class FileController
    {
        public void saveToFile(string filePath, List<EigenPair> eigenPairs, List<List<double>> matrix)
        {
            if (!string.IsNullOrEmpty(filePath) && eigenPairs != null && eigenPairs.Count > 0 && filePath != "__________")
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Matrix");
                    foreach (List<double> row in matrix)
                    {
                        writer.WriteLine(String.Join(" ", row));
                    }
                    foreach (EigenPair pair in eigenPairs)
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