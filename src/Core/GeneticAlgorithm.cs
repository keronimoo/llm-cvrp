using CapacitatedVehicleRoutingProblem.Models;
using CapacitatedVehicleRoutingProblem.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Utils;
using CapacitatedVehicleRoutingProblem.Models.Configurations;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Implements a genetic algorithm solver for the Capacitated Vehicle Routing Problem (CVRP).
    /// This class manages the evolutionary process including population initialization, selection,
    /// crossover, mutation, and replacement strategies.
    /// </summary>
    public class GeneticAlgorithm
    {
        private readonly GeneticAlgorithmConfig _config;
        private readonly List<Customer> _customers;
        private readonly List<Vehicle> _vehicles;
        private readonly Random _random;
        private readonly Depot _depot;
        private readonly Population _population;
        private readonly Visualization _visualization;
        private int _currentGeneration;
        private double _bestSolutionCost = double.MaxValue;
        private List<Vehicle> _bestSolution;

        public List<double> BestSolutionCosts { get; private set; }
        public List<double> AverageSolutionCosts { get; private set; }

        /// <summary>
        /// Initializes a new instance of the genetic algorithm solver.
        /// </summary>
        /// <param name="customers">List of customers to be serviced</param>
        /// <param name="vehicles">Available vehicles for routing</param>
        /// <param name="config">Configuration parameters for the genetic algorithm</param>
        /// <param name="depot">Central depot location</param>
        /// <param name="visualization">Visualization component for solution rendering</param>
        public GeneticAlgorithm(
            List<Customer> customers,
            List<Vehicle> vehicles,
            GeneticAlgorithmConfig config,
            Depot depot,
            Visualization visualization)
        {
            _config = config;
            _customers = customers;
            _vehicles = vehicles;
            _random = new Random();
            _depot = depot;
            _population = new Population(_config.PopulationSize, customers, vehicles);
            _visualization = visualization;
            BestSolutionCosts = new List<double>();
            AverageSolutionCosts = new List<double>();
        }

        public void InitializePopulation()
        {
            _population.Initialize();
        }

        /// <summary>
        /// Executes the evolutionary process for the configured number of generations.
        /// The algorithm follows these steps for each generation:
        /// 1. Selection of parent solutions
        /// 2. Crossover to create offspring
        /// 3. Mutation of offspring
        /// 4. Replacement to form next generation
        /// </summary>
        public void Evolve()
        {
            Console.WriteLine("Running ...");
            _currentGeneration = 0;

            UpdateStatistics();

            for (_currentGeneration = 1; _currentGeneration < _config.MaxGenerations; _currentGeneration++)
            {
                try
                {
                    var currentPopulation = _population.GetAllSolutions();

                    // Selection
                    var selected = _config.SelectionOperator(
                        currentPopulation,
                        _config.PopulationSize,
                        _config.TournamentSize,
                        solution => _config.FitnessFunction(solution));

                    // Crossover
                    var offspring = ApplyCrossover(selected);

                    // Mutation
                    ApplyMutation(offspring);

                    // Replacement
                    var nextGeneration = _config.ReplacementStrategy(
                        currentPopulation,
                        offspring,
                        _config.EliteCount,
                        solution => _config.FitnessFunction(solution));

                    // Update population
                    _population.UpdatePopulation(nextGeneration);

                    // Update statistics
                    UpdateStatistics();


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in generation {_currentGeneration}: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Applies crossover operator to selected parents to generate offspring solutions.
        /// Preserves elite solutions and applies crossover based on the configured rate.
        /// </summary>
        /// <param name="population">Current population of solutions</param>
        /// <returns>New population after crossover</returns>
        private List<List<Vehicle>> ApplyCrossover(List<List<Vehicle>> population)
        {
            var newPopulation = new List<List<Vehicle>>();

            // Keep elite solutions
            newPopulation.AddRange(population.Take(_config.EliteCount)
                .Select(solution => solution.Select(v =>
                    new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route))).ToList()));

            // Apply crossover to rest of population
            for (int i = _config.EliteCount; i < population.Count; i += 2)
            {
                if (i + 1 >= population.Count)
                {
                    newPopulation.Add(new List<Vehicle>(population[i].Select(v =>
                        new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route)))));
                    break;
                }

                if (_random.NextDouble() < _config.CrossoverRate)
                {
                    /*
                    Vehicle1: Depot → C1 → C2 → C3 → Depot
                    Vehicle2: Depot → C4 → C5 → C6 → Depot
"                   1x2 2x1
                    Vehicle1: Depot → C2 → C4 → C1 → Depot
                    Vehicle2: Depot → C6 → C3 → C5 → Depot
                    */
                    var offspring1 = _config.CrossoverOperator(population[i], population[i + 1]);
                    var offspring2 = _config.CrossoverOperator(population[i + 1], population[i]);
                    newPopulation.Add(offspring1);
                    newPopulation.Add(offspring2);
                }
                else
                {
                    // Keep parents if no crossover
                    newPopulation.AddRange(new[] { population[i], population[i + 1] }
                        .Select(p => p.Select(v =>
                            new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route))).ToList()));
                }
            }

            return newPopulation;
        }


        private void ApplyMutation(List<List<Vehicle>> population)
        {
            for (int i = _config.EliteCount; i < population.Count; i++)
            {
                if (_random.NextDouble() < _config.MutationRate)
                {
                    try
                    {
                        _config.MutationOperator(population[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Mutation error: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateStatistics()
        {
            var currentPopulation = _population.GetAllSolutions();
            var costs = currentPopulation.Select(solution => _config.FitnessFunction(solution)).ToList();

            double currentBestCost = costs.Min();
            if (currentBestCost < _bestSolutionCost)
            {
                _bestSolutionCost = currentBestCost;
                _bestSolution = currentPopulation[costs.IndexOf(currentBestCost)]
                    .Select(v => new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route)))
                    .ToList();
            }

            double averageCost = StatisticsHelper.CalculateMean(costs);

            BestSolutionCosts.Add(_bestSolutionCost);
            AverageSolutionCosts.Add(averageCost);

            if (_currentGeneration % 20 == 0)
            {
                var currentStats = new RunStatistics
                {
                    Best = _bestSolutionCost,
                    Worst = costs.Max(),
                    Mean = averageCost,
                    StdDev = StatisticsHelper.CalculateStandardDeviation(costs)
                };

                Console.Write($"Generation {_currentGeneration}:");
                Console.Write($"  Best Solution Cost: {currentStats.Best:F2}");
                Console.Write($"  Average Cost: {currentStats.Mean:F2}  ");
                Console.WriteLine($"");

                _visualization.SetHighlightedVehicle(0);
                _visualization.DisplaySolution(_bestSolution, _currentGeneration, _bestSolutionCost);
            }
        }

        public RunStatistics GetFinalStatistics()
        {
            var costs = _population.GetAllSolutions()
                .Select(solution => _config.FitnessFunction(solution))
                .ToList();

            return new RunStatistics
            {
                Best = costs.Min(),
                Worst = costs.Max(),
                Mean = StatisticsHelper.CalculateMean(costs),
                StdDev = StatisticsHelper.CalculateStandardDeviation(costs)
            };
        }

        public List<Vehicle> GetBestSolution()
        {
            return _bestSolution ?? _population.GetBestSolution(solution => _config.FitnessFunction(solution));
        }
    }
}