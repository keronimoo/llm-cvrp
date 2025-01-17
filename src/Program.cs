using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models.Statistics;
using CapacitatedVehicleRoutingProblem.Models;
using CapacitatedVehicleRoutingProblem.Core;
using CapacitatedVehicleRoutingProblem.Utils;
using CapacitatedVehicleRoutingProblem.Models.Configurations;

namespace CapacitatedVehicleRoutingProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Parse the dataset
                string filePath = "XML100_2144_06.vrp";
                var (customers, vehicles, depot) = DatasetParser.ParseDataset(filePath);

                Console.WriteLine("Dataset successfully parsed.");
                Console.WriteLine($"Number of customers: {customers.Count}");
                Console.WriteLine($"Number of vehicles: {vehicles.Count}");
                Console.WriteLine($"Capacity: {vehicles[0].Capacity}");
                Console.WriteLine($"Depot location: ({depot.X}, {depot.Y})");

                Fitness.Initialize(customers, depot); // Initialize Distance Matrix

                // Ask user for run mode
                Console.WriteLine("\nSelect run mode:");
                Console.WriteLine("1. Single Configuration");
                Console.WriteLine("2. Full Experiment");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    RunSingleConfiguration(customers, vehicles, depot);
                }
                else if (choice == "2")
                {
                    RunFullExperiment(customers, vehicles, depot);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Exiting...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void RunSingleConfiguration(List<Customer> customers, List<Vehicle> vehicles, Depot depot)
        {
            Console.WriteLine("\nEnter configuration parameters:");

            // Get population size
            Console.Write("Population Size (e.g., 100, 150, 200): ");
            int popSize = int.Parse(Console.ReadLine());

            // Get crossover operator
            Console.WriteLine("\nSelect Crossover Operator:");
            Console.WriteLine("1. Edge Recombination");
            Console.WriteLine("2. Order");
            Console.WriteLine("3. Partially Mapped");
            string crossoverChoice = Console.ReadLine();

            Func<List<Vehicle>, List<Vehicle>, List<Vehicle>> crossoverOperator = crossoverChoice switch
            {
                "1" => Crossover.EdgeRecombinationCrossover,
                "2" => Crossover.OrderCrossover,
                "3" => Crossover.PartiallyMappedCrossover,
                _ => Crossover.EdgeRecombinationCrossover
            };

            // Get mutation operator
            Console.WriteLine("\nSelect Mutation Operator:");
            Console.WriteLine("1. Scramble");
            Console.WriteLine("2. Swap");
            Console.WriteLine("3. Insert");
            string mutationChoice = Console.ReadLine();

            Action<List<Vehicle>> mutationOperator = mutationChoice switch
            {
                "1" => Mutation.ScrambleMutation,
                "2" => Mutation.SwapMutation,
                "3" => Mutation.InsertMutation,
                _ => Mutation.ScrambleMutation
            };

            // Create configuration
            var config = new GeneticAlgorithmConfig
            {
                PopulationSize = popSize,
                MaxGenerations = 300,
                CrossoverRate = 0.85,
                MutationRate = 0.5,
                TournamentSize = 2,
                EliteCount = 10,
                SelectionOperator = Selection.TournamentSelection,
                ReplacementStrategy = Replacement.ElitistReplacement,
                CrossoverOperator = crossoverOperator,
                MutationOperator = mutationOperator,
                FitnessFunction = Fitness.CalculateDistanceWithCapacity
            };

            // Create results directory
            string resultsDir = DirectoryManager.CreateResultsDirectory();
            string configDir = Path.Combine(resultsDir, $"SingleRun_{popSize}_{crossoverChoice}_{mutationChoice}");
            string runDir = Path.Combine(configDir, "Run_1");
            Directory.CreateDirectory(runDir);

            // Run GA
            var visualization = new Visualization(depot, customers, configDir, runDir);
            var ga = new GeneticAlgorithm(customers, vehicles, config, depot, visualization);
            ga.InitializePopulation();
            ga.Evolve();

            // Get and display final results
            var runStats = ga.GetFinalStatistics();
            var bestSolution = ga.GetBestSolution();

            Console.WriteLine("\nFinal Solution:");
            Console.WriteLine($"Total Cost: {runStats.Best:F2}");
            Console.WriteLine("\nRoutes:");

            foreach (var vehicle in bestSolution)
            {
                Console.WriteLine($"\nVehicle {vehicle.Id}:");
                Console.WriteLine($"Load: {vehicle.Load:F1}/{vehicle.Capacity:F1}");
                Console.Write("Route: Depot -> ");

                foreach (var customer in vehicle.Route)
                {
                    Console.Write($"C{customer.Id} -> ");
                }
                Console.WriteLine("Depot");
            }

            // Calculate and display capacity utilization
            double totalCapacity = bestSolution.Sum(v => v.Capacity);
            double totalLoad = bestSolution.Sum(v => v.Load);
            

            
            
            // Save results
            StatisticsWriter.CreateStatisticsFile(configDir, new List<RunStatistics> { runStats });
        }

        static void RunFullExperiment(List<Customer> customers, List<Vehicle> vehicles, Depot depot)
        {
            var populationSizes = new[] { 100, 150, 200 };
            var crossoverOperators = new Dictionary<string, Func<List<Vehicle>, List<Vehicle>, List<Vehicle>>>
            {
                { "EdgeRecombination", Crossover.EdgeRecombinationCrossover },
                { "Order", Crossover.OrderCrossover },
                { "PartiallyMapped", Crossover.PartiallyMappedCrossover }
            };
            var mutationOperators = new Dictionary<string, Action<List<Vehicle>>>
            {
                { "Scramble", Mutation.ScrambleMutation },
                { "Swap", Mutation.SwapMutation },
                { "Insert", Mutation.InsertMutation }
            };

            // Create base results directory
            string resultsDir = DirectoryManager.CreateResultsDirectory();
            var allConfigurationStats = new List<ConfigurationStatistics>();

            int totalConfigs = populationSizes.Length * crossoverOperators.Count * mutationOperators.Count;
            int currentConfig = 0;

            foreach (var popSize in populationSizes)
            {
                foreach (var (crossoverName, crossover) in crossoverOperators)
                {
                    foreach (var (mutationName, mutation) in mutationOperators)
                    {
                        currentConfig++;
                        string configName = $"Pop{popSize}_{crossoverName}Crossover_{mutationName}Mutation";

                        Console.WriteLine();
                        Console.WriteLine($"Processing configuration {currentConfig}/{totalConfigs}:");
                        Console.WriteLine($"Population Size: {popSize}");
                        Console.WriteLine($"Crossover: {crossoverName}");
                        Console.WriteLine($"Mutation: {mutationName}");
                        Console.WriteLine("----------------------------------------");

                        string configDir = DirectoryManager.CreateConfigDirectory(popSize, crossoverName, mutationName);
                        var allRunStats = new List<RunStatistics>();
                        var allConvergenceData = new List<List<double>>();
                        var allBestCosts = new List<double>();

                        for (int run = 1; run <= 1; run++)
                        {
                            Console.WriteLine($"  Run {run}/10");

                            string runDir = DirectoryManager.CreateRunDirectory(configDir, run);

                            var config = new GeneticAlgorithmConfig
                            {
                                PopulationSize = popSize,
                                MaxGenerations = 320,
                                CrossoverRate = 0.85,
                                MutationRate = 0.1,
                                TournamentSize = 2,
                                EliteCount = 10,
                                SelectionOperator = Selection.TournamentSelection,
                                ReplacementStrategy = Replacement.ElitistReplacement,
                                CrossoverOperator = crossover,
                                MutationOperator = mutation,
                                FitnessFunction = Fitness.CalculateDistanceWithCapacity
                            };

                            var visualization = new Visualization(depot, customers, configDir, runDir);
                            var ga = new GeneticAlgorithm(customers, vehicles, config, depot, visualization);
                            ga.InitializePopulation();
                            ga.Evolve();

                            var runStats = ga.GetFinalStatistics();
                            allRunStats.Add(runStats);
                            allConvergenceData.Add(ga.BestSolutionCosts);

                            allBestCosts.Add(ga.BestSolutionCosts.Min());
                        }

                        Console.WriteLine($"Configuration {currentConfig} completed.");
                        Console.WriteLine("----------------------------------------");

                        // Create statistics file and convergence plot for this configuration
                        StatisticsWriter.CreateStatisticsFile(configDir, allRunStats);
                        Visualization.CreateConvergencePlot(configDir, allConvergenceData);

                        // Add configuration statistics to global list
                        allConfigurationStats.Add(new ConfigurationStatistics
                        {
                            PopulationSize = popSize,
                            CrossoverOperator = crossoverName,
                            MutationOperator = mutationName,
                            SelectionOperator = "TournamentSelection",
                            ReplacementOperator = "ElitistReplacement",
                            TournamentSize = 2,
                            EliteCount = 10,
                            CrossoverRate = 0.85,
                            MutationRate = 0.1,
                            MaxGenerations = 320,
                            FitnessFunction = "CalculateDistanceWithCapacity",
                            Best = allBestCosts.Min(),
                            Worst = allBestCosts.Max(),
                            Mean = allBestCosts.Average(),
                            StdDev = Math.Sqrt(allBestCosts.Average(x => Math.Pow(x - allBestCosts.Average(), 2)))
                        });
                    }
                }
            }

            // Create global statistics file
            StatisticsWriter.CreateGlobalStatisticsFile(resultsDir, allConfigurationStats);
        }
    }
}
