using System;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");

        List<List<double>> matrix = new List<List<double>>
        {
            new List<double> {0, 0, 0},
            new List<double> {0, 0, 0},
            new List<double> {0, 0, 0}
        };

        DenseMatrix denseB = DenseMatrix.OfColumnArrays(matrix.Select(row => row.ToArray()).ToArray());
        double detB = denseB.Determinant();

        if (detB == 0)
        {
            throw new InvalidOperationException("Matrix B is singular (det(B) = 0). The method cannot proceed.");
        }

        // Додайте подальшу обробку, якщо визначник не дорівнює нулю
    }
}