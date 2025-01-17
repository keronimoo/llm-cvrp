namespace CapacitatedVehicleRoutingProblem.Utils
{
    public static class DirectoryManager
    {
        public static string CreateResultsDirectory()
        {
            string resultsDir = Path.Combine(Directory.GetCurrentDirectory(), "Results");
            Directory.CreateDirectory(resultsDir);
            return resultsDir;
        }

        public static string CreateConfigDirectory(int populationSize, string crossoverName, string mutationName)
        {
            string resultsDir = CreateResultsDirectory();
            string configName = $"Pop{populationSize}_{crossoverName}Crossover_{mutationName}Mutation";
            string configDir = Path.Combine(resultsDir, configName);
            Directory.CreateDirectory(configDir);
            return configDir;
        }

        public static string CreateRunDirectory(string configDir, int runNumber)
        {
            string runDir = Path.Combine(configDir, $"Run_{runNumber}");
            Directory.CreateDirectory(runDir);
            return runDir;
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
} 