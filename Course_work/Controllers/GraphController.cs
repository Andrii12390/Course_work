using OxyPlot;
using System.Windows;
namespace Course_work
{

    internal class GraphController
    {
        private Graph _graph;

        public GraphController(double[] polynomialCoefficients, double minX, double maxX, double[] roots)
        {
            _graph = new Graph(polynomialCoefficients, minX, maxX, roots);
        }

        public PlotModel? BuildGraph()
        {
            try
            {
                return _graph.BuildGraph();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while building graph: {ex.Message}"); return null;
            }
        }
    }
}
