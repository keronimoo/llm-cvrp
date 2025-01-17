namespace CapacitatedVehicleRoutingProblem.Utils
{
    public static class StatisticsHelper
    {
        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double mean = values.Average();
            return Math.Sqrt(values.Average(x => Math.Pow(x - mean, 2)));
        }

        public static double CalculateMean(IEnumerable<double> values)
        {
            return values.Average();
        }
    }
} 