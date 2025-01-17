namespace CapacitatedVehicleRoutingProblem.Models.Statistics
{
    public class ConfigurationStatistics
    {
        public int PopulationSize { get; set; }
        public string CrossoverOperator { get; set; }
        public string MutationOperator { get; set; }
        public string SelectionOperator { get; set; }
        public string ReplacementOperator { get; set; }
        public int TournamentSize { get; set; }
        public int EliteCount { get; set; }
        public double CrossoverRate { get; set; }
        public double MutationRate { get; set; }
        public int MaxGenerations { get; set; }
        public string FitnessFunction { get; set; }
        public double Best { get; set; }
        public double Worst { get; set; }
        public double Mean { get; set; }
        public double StdDev { get; set; }
    }
} 