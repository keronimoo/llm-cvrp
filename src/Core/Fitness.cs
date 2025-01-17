using CapacitatedVehicleRoutingProblem.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Provides fitness evaluation functions for CVRP solutions.
    /// Each function implements different optimization objectives and constraint handling.
    /// </summary>
    public static class Fitness
    {
        private static DistanceMatrix _distanceMatrix;

        public static void Initialize(List<Customer> customers, Depot depot)
        {
            _distanceMatrix = new DistanceMatrix(customers, depot);
        }

        /// <summary>
        /// Evaluates solution cost based on total route distance while heavily penalizing
        /// capacity violations using a quadratic penalty function.
        /// 
        /// Penalty = capacityPenalty * (violation/capacity)^2
        /// </summary>
        /// <param name="solution">Vehicle routing solution to evaluate</param>
        /// <returns>Total cost including penalties</returns>
        public static double CalculateDistance(List<Vehicle> solution)
        {
            double totalCost = 0;
            foreach (var vehicle in solution)
            {
                if (vehicle.Route.Count == 0) continue;

                // Add distance from depot to first customer
                totalCost += _distanceMatrix.GetDepotDistance(vehicle.Route[0]);

                // Add distances between customers
                for (int i = 0; i < vehicle.Route.Count - 1; i++)
                {
                    totalCost += _distanceMatrix.GetDistance(vehicle.Route[i], vehicle.Route[i + 1]);
                }

                // Add distance back to depot
                totalCost += _distanceMatrix.GetDepotDistance(vehicle.Route[vehicle.Route.Count - 1]);
            }
            return totalCost;
        }

        /// <summary>
        /// Evaluates solution cost based on total route distance while heavily penalizing
        /// capacity violations using a quadratic penalty function.
        /// 
        /// Penalty = capacityPenalty * (violation/capacity)^2
        /// </summary>
        /// <param name="solution">Vehicle routing solution to evaluate</param>
        /// <returns>Total cost including penalties</returns>
        public static double CalculateDistanceWithCapacity(List<Vehicle> solution)
        {
            double capacityPenalty = 2000;
            double totalCost = CalculateDistance(solution);

            foreach (var vehicle in solution)
            {
                if (vehicle.Load > vehicle.Capacity)
                {
                    double violation = vehicle.Load - vehicle.Capacity;
                    totalCost += capacityPenalty * Math.Pow(violation / vehicle.Capacity, 2);
                }
            }
            
            return totalCost;
        }


        public static DistanceMatrix GetDistanceMatrix()
        {
            return _distanceMatrix;
        }
    }
}