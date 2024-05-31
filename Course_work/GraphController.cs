using OxyPlot;
using System.Windows;

namespace Course_work
{

    internal class GraphController
    {
        private Graph graph;

        public GraphController(double[] polynomialCoefficients, double minX, double maxX, double[] roots)
        {
            graph = new Graph(polynomialCoefficients, minX, maxX, roots);
        }

        public PlotModel BuildGraph()
        {
            try
            {
                return graph.BuildGraph();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while building graph: {ex.Message}");
            }
        }
    }

}
