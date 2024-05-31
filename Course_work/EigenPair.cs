
namespace Course_work
{
    internal class EigenPair
    {
        private double _eigenValue;
        private double[] _eigenVector;
        private string _eigenVectorString;

        public double EigenValue { get => _eigenValue; set => _eigenValue = value; }
        public string EigenVectorString { get => _eigenVectorString; }
        public double[] EigenVector {
            get => _eigenVector;
            set
            {
                _eigenVector = value;
                _eigenVectorString = string.Join(", ", _eigenVector);
            }
        }

    }
}
