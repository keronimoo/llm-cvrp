using System;
using System.Collections.Generic;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Provides mutation operators for modifying solutions during evolution.
    /// Each operator implements different strategies for introducing diversity
    /// while maintaining solution feasibility.
    /// </summary>
    public static class Mutation
    {
        private static Random random = new Random();

        /// <summary>
        /// Performs a swap mutation by exchanging positions of two randomly selected
        /// customers within a randomly chosen vehicle's route.
        /// This helps explore local neighborhood solutions while maintaining route structure.
        /// </summary>
        /// <param name="solution">Solution to be mutated</param>
        public static void SwapMutation(List<Vehicle> solution)
        {
            // Select random vehicle with at least 2 customers
            var validVehicles = solution.Where(v => v.Route.Count >= 2).ToList();
            if (!validVehicles.Any()) return;

            var vehicle = validVehicles[random.Next(validVehicles.Count)];


            int pos1 = random.Next(vehicle.Route.Count);
            int pos2 = random.Next(vehicle.Route.Count);

            (vehicle.Route[pos1], vehicle.Route[pos2]) = (vehicle.Route[pos2], vehicle.Route[pos1]);
        }

        /// <summary>
        /// Performs insertion mutation by moving a customer from one vehicle to another.
        /// </summary>
        /// <param name="solution">Solution to be mutated</param>
        public static void InsertMutation(List<Vehicle> solution)
        {
            var sourceVehicles = solution.Where(v => v.Route.Count > 0).ToList();
            if (!sourceVehicles.Any()) return;

            var sourceVehicle = sourceVehicles[random.Next(sourceVehicles.Count)];
            var targetVehicle = solution[random.Next(solution.Count)];

            if (sourceVehicle == targetVehicle) return;

            // Select random customer
            int customerIndex = random.Next(sourceVehicle.Route.Count);
            var customer = sourceVehicle.Route[customerIndex];

            // Only proceed if target vehicle has capacity
            if (targetVehicle.Load + customer.Demand <= targetVehicle.Capacity)
            {
                sourceVehicle.Route.RemoveAt(customerIndex);

                // Insert at random position in target route
                int insertPos = targetVehicle.Route.Count == 0 ?
                    0 : random.Next(targetVehicle.Route.Count + 1);

                targetVehicle.Route.Insert(insertPos, customer);

                sourceVehicle.UpdateLoad();
                targetVehicle.UpdateLoad();
            }
        }

        
        /// <summary>
        /// Performs scramble mutation by randomly reordering a subsequence of customers
        /// within a vehicle's route. This provides more dramatic changes than swap
        /// mutation and helps escape local optima.
        /// </summary>
        /// <param name="solution">Solution to be mutated</param>
        public static void ScrambleMutation(List<Vehicle> solution)
        {
            // Select vehicle with non-empty route
            var validVehicles = solution.Where(v => v.Route.Count >= 2).ToList();
            if (!validVehicles.Any()) return;

            var vehicle = validVehicles[random.Next(validVehicles.Count)];
            
            // Select subsequence bounds
            int start = random.Next(vehicle.Route.Count - 1); // Ensure at least 2 elements
            int end = random.Next(start + 1, vehicle.Route.Count);
            int length = end - start + 1;

            // Store original load
            double originalLoad = vehicle.Load;

            // Get and shuffle subsequence
            var subsequence = vehicle.Route.GetRange(start, length);
            for (int i = subsequence.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (subsequence[i], subsequence[j]) = (subsequence[j], subsequence[i]);
            }

            // Replace in original route
            vehicle.Route.RemoveRange(start, length);
            vehicle.Route.InsertRange(start, subsequence);
            
            vehicle.UpdateLoad();

            // Verify capacity constraint
            if (vehicle.Load > vehicle.Capacity)
            {
                // Restore original route order
                vehicle.Route.RemoveRange(start, length);
                vehicle.Route.InsertRange(start, subsequence.OrderBy(c => c.Id));
                vehicle.Load = originalLoad;
            }
        }
    }
}
