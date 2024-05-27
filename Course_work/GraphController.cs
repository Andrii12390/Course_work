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

        public PlotModel buildGraph()
        {
            try
            {
                return graph.buildGraph();
            }
            catch (Exception)
            {
                throw new Exception("Program cannot build this graph");
            }
        }
    }

}
