using CapacitatedVehicleRoutingProblem.Models.Statistics;
using System.Text;

namespace CapacitatedVehicleRoutingProblem.Utils
{
    public static class StatisticsWriter
    {
        public static void CreateStatisticsFile(string configDir, List<RunStatistics> allRunStats)
        {
            Directory.CreateDirectory(configDir);

            var stats = new StringBuilder();
            stats.AppendLine("Best,Worst,Mean,StdDev");
            
            foreach (var run in allRunStats)
            {
                stats.AppendLine($"{run.Best:F2},{run.Worst:F2},{run.Mean:F2},{run.StdDev:F2}");
            }

            File.WriteAllText(Path.Combine(configDir, "statistics.csv"), stats.ToString());
        }

        public static void CreateGlobalStatisticsFile(string resultsDir, List<ConfigurationStatistics> allConfigStats)
        {
            Directory.CreateDirectory(resultsDir);

            var stats = new StringBuilder();
            
            stats.AppendLine("PopulationSize,CrossoverOperator,MutationOperator," +
                           "SelectionOperator,ReplacementOperator,TournamentSize," +
                           "EliteCount,CrossoverRate,MutationRate,MaxGenerations," +
                           "FitnessFunction,Best,Worst,Mean,StdDev");

            foreach (var config in allConfigStats)
            {
                stats.AppendLine(
                    $"{config.PopulationSize}," +
                    $"{config.CrossoverOperator}," +
                    $"{config.MutationOperator}," +
                    $"{config.SelectionOperator}," +
                    $"{config.ReplacementOperator}," +
                    $"{config.TournamentSize}," +
                    $"{config.EliteCount}," +
                    $"{config.CrossoverRate}," +
                    $"{config.MutationRate}," +
                    $"{config.MaxGenerations}," +
                    $"{config.FitnessFunction}," +
                    $"{config.Best:F2}," +
                    $"{config.Worst:F2}," +
                    $"{config.Mean:F2}," +
                    $"{config.StdDev:F2}");
            }

            File.WriteAllText(Path.Combine(resultsDir, "global_statistics.csv"), stats.ToString());
        }
    }
} 