using System;
using System.Collections.Generic;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Provides selection operators for choosing parent solutions during evolution.
    /// Implements various selection strategies to maintain selection pressure while
    /// preserving population diversity.
    /// </summary>
    public static class Selection
    {
        private static Random random = new Random();

        /// <summary>
        /// Performs tournament selection to create a new population of parent solutions.
        /// For each selection:
        /// 1. Randomly selects tournament_size individuals
        /// 2. Chooses the best individual from the tournament
        /// 3. Creates a deep copy of the winner
        /// </summary>
        /// <param name="population">Current population of solutions</param>
        /// <param name="populationSize">Size of population to maintain</param>
        /// <param name="tournamentSize">Number of solutions in each tournament</param>
        /// <param name="fitnessFunction">Function to evaluate solution fitness</param>
        /// <returns>Selected parent solutions</returns>
        public static List<List<Vehicle>> TournamentSelection(
            List<List<Vehicle>> population,
            int populationSize,
            int tournamentSize,
            Func<List<Vehicle>, double> fitnessFunction)
        {
            var selected = new List<List<Vehicle>>();

            // Select individuals until we reach desired population size
            while (selected.Count < populationSize)
            {
                // Create tournament
                var tournament = new List<List<Vehicle>>();
                for (int i = 0; i < tournamentSize; i++)
                {
                    int randomIndex = random.Next(population.Count);
                    tournament.Add(population[randomIndex]);
                }

                // Select winner (best fitness)
                var winner = tournament
                    .OrderBy(solution => fitnessFunction(solution))
                    .First();

                // Add deep copy of winner to selected population
                selected.Add(
                    winner.Select(v => new Vehicle(
                        v.Id, 
                        v.Capacity, 
                        new List<Customer>(v.Route)))
                    .ToList()
                );
            }

            return selected;
        }

        /// <summary>
        /// Helper method to select a single winner from a tournament.
        /// Ensures proper handling of tournament size constraints and
        /// creates deep copies of selected solutions.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when population is empty or tournament size is invalid
        /// </exception>
        private static List<Vehicle> SelectSingleTournamentWinner(
            List<List<Vehicle>> population,
            int tournamentSize,
            Func<List<Vehicle>, double> fitnessFunction)
        {
            if (population == null || population.Count == 0)
                throw new ArgumentException("Population cannot be null or empty");

            if (tournamentSize <= 0 || tournamentSize > population.Count)
                throw new ArgumentException($"Invalid tournament size: {tournamentSize}");

            var tournament = new List<List<Vehicle>>();
            
            // Select random participants
            for (int i = 0; i < tournamentSize; i++)
            {
                int randomIndex = random.Next(population.Count);
                tournament.Add(population[randomIndex]);
            }

            // Return the best solution from tournament
            return tournament
                .OrderBy(solution => fitnessFunction(solution))
                .First()
                .Select(v => new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route)))
                .ToList();
        }
    }
}
