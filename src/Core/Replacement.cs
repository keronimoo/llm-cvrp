using System;
using System.Collections.Generic;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Provides replacement strategies for generational transitions in the genetic algorithm.
    /// This class implements methods to determine how the next generation's population is formed
    /// from the current population and offspring solutions.
    /// </summary>
    public static class Replacement
    {
        private static Random random = new Random();

        /// <summary>
        /// Implements an elitist replacement strategy where the best solutions from the current
        /// population are preserved while the rest are replaced with the best offspring solutions.
        /// This ensures that the best solutions found so far are never lost during evolution.
        /// </summary>
        /// <param name="currentPopulation">The current generation's population of solutions</param>
        /// <param name="offspring">The newly generated offspring solutions</param>
        /// <param name="eliteCount">Number of best solutions to preserve from current population</param>
        /// <param name="fitnessFunction">Function to evaluate solution fitness</param>
        /// <returns>
        /// A new population combining elite solutions from current population and best offspring,
        /// maintaining the original population size
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Sorts both current population and offspring by fitness
        /// 2. Preserves the top 'eliteCount' solutions from current population
        /// 3. Fills remaining slots with best offspring solutions
        /// 4. Ensures proper deep copying of solutions to prevent reference issues
        /// 5. Validates the new population size matches the original
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the resulting population size doesn't match the original population size
        /// </exception>
        public static List<List<Vehicle>> ElitistReplacement(
            List<List<Vehicle>> currentPopulation,
            List<List<Vehicle>> offspring,
            int eliteCount,
            Func<List<Vehicle>, double> fitnessFunction)
        {
            var nextGeneration = new List<List<Vehicle>>();

            // Sort both populations by fitness
            var sortedCurrent = currentPopulation
                .OrderBy(solution => fitnessFunction(solution))
                .ToList();

            var sortedOffspring = offspring
                .OrderBy(solution => fitnessFunction(solution))
                .ToList();

            // Keep the best solutions from current population (elitism)
            nextGeneration.AddRange(
                sortedCurrent.Take(eliteCount)
                .Select(solution => solution.Select(v =>
                    new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route))).ToList())
            );

            // Fill the rest with the best offspring
            int remainingSlots = currentPopulation.Count - eliteCount;
            nextGeneration.AddRange(
                sortedOffspring.Take(remainingSlots)
                .Select(solution => solution.Select(v =>
                    new Vehicle(v.Id, v.Capacity, new List<Customer>(v.Route))).ToList())
            );

            // Verify population size
            if (nextGeneration.Count != currentPopulation.Count)
            {
                throw new InvalidOperationException(
                    $"Invalid next generation size. Expected: {currentPopulation.Count}, Got: {nextGeneration.Count}");
            }

            return nextGeneration;
        }
    }
}