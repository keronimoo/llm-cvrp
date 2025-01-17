using System;
using System.Collections.Generic;
using CapacitatedVehicleRoutingProblem.Core;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Models.Configurations
{
    /// <summary>
    /// Configuration class for the genetic algorithm, containing all parameters
    /// and operator definitions needed for the evolutionary process.
    /// </summary>
    public class GeneticAlgorithmConfig
    {

        // Population parameters
        /// <summary>Size of population to maintain across generations</summary>
        public int PopulationSize { get; set; }
        
        /// <summary>Maximum number of generations to evolve</summary>
        public int MaxGenerations { get; set; }
        
        // Genetic operator rates
        /// <summary>Probability of applying crossover to selected parents</summary>
        public double CrossoverRate { get; set; }
        
        /// <summary>Probability of applying mutation to offspring</summary>
        public double MutationRate { get; set; }
        
        // Selection parameters
        /// <summary>Number of solutions to compete in tournament selection</summary>
        public int TournamentSize { get; set; }
        
        /// <summary>Number of best solutions to preserve unchanged</summary>
        public int EliteCount { get; set; }
        
        // Operator delegates
        /// <summary>Function to evaluate solution fitness</summary>
        public Func<List<Vehicle>, double> FitnessFunction { get; set; }
        
        /// <summary>Selection operator for choosing parents</summary>
        public Func<List<List<Vehicle>>, int, int, Func<List<Vehicle>, double>, List<List<Vehicle>>> SelectionOperator { get; set; }
        
        /// <summary>Crossover operator for creating offspring</summary>
        public Func<List<Vehicle>, List<Vehicle>, List<Vehicle>> CrossoverOperator { get; set; }
        
        /// <summary>Mutation operator for introducing diversity</summary>
        public Action<List<Vehicle>> MutationOperator { get; set; }
        
        /// <summary>Strategy for creating next generation population</summary>
        public Func<List<List<Vehicle>>, List<List<Vehicle>>, int, Func<List<Vehicle>, double>, List<List<Vehicle>>> 
            ReplacementStrategy { get; set; }

        /// <summary>
        /// Initializes configuration with default parameter values and operators.
        /// </summary>
        public GeneticAlgorithmConfig()
        {
            PopulationSize = 100;
            MaxGenerations = 1000;
            CrossoverRate = 0.8;
            MutationRate = 0.1;
            TournamentSize = 5;
            EliteCount = 2;
            
            // Set default operators
            SelectionOperator = Selection.TournamentSelection;
            CrossoverOperator = Crossover.OrderCrossover;
            MutationOperator = Mutation.SwapMutation;
            FitnessFunction = Fitness.CalculateDistanceWithCapacity;
            
            // Set default replacement strategy to Elitist Replacement
            ReplacementStrategy = Replacement.ElitistReplacement;
        }

        
    }
} 