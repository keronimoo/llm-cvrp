using System;
using System.Collections.Generic;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Manages distance calculations and caching between all locations (customers and depot)
    /// in the CVRP problem. Uses a matrix structure for O(1) distance lookups.
    /// </summary>
    public class DistanceMatrix
    {
        private readonly double[,] _distances;
        private readonly Dictionary<int, int> _customerToIndex;
        private readonly int _depotIndex;

        /// <summary>
        /// Initializes the distance matrix by pre-calculating all pairwise distances
        /// between customers and depot. Stores depot at index 0 for efficient access.
        /// </summary>
        /// <param name="customers">List of customers in the problem</param>
        /// <param name="depot">Central depot location</param>
        public DistanceMatrix(List<Customer> customers, Depot depot)
        {
            int size = customers.Count + 1; // +1 for depot
            _distances = new double[size, size];
            _customerToIndex = new Dictionary<int, int>();

            // Assign depot to index 0
            _depotIndex = 0;

            // Create mapping of customer IDs to matrix indices
            for (int i = 0; i < customers.Count; i++)
            {
                _customerToIndex[customers[i].Id] = i + 1;
            }

            // Calculate distances between all points
            // Depot distances (index 0)
            for (int i = 0; i < customers.Count; i++)
            {
                _distances[0, i + 1] = CalculateDistance(depot.X, depot.Y, customers[i].X, customers[i].Y);
                _distances[i + 1, 0] = _distances[0, i + 1];
            }

            // Customer to customer distances
            for (int i = 0; i < customers.Count; i++)
            {
                for (int j = i + 1; j < customers.Count; j++)
                {
                    _distances[i + 1, j + 1] = CalculateDistance(
                        customers[i].X, customers[i].Y,
                        customers[j].X, customers[j].Y);
                    _distances[j + 1, i + 1] = _distances[i + 1, j + 1];
                }
            }
        }

        /// <summary>
        /// Calculates Euclidean distance between two points.
        /// </summary>
        /// <returns>Direct distance between the points</returns>
        private double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        /// <summary>
        /// Retrieves pre-calculated distance between two customers.
        /// </summary>
        /// <param name="customer1">First customer</param>
        /// <param name="customer2">Second customer</param>
        /// <returns>Distance between the customers</returns>
        public double GetDistance(Customer customer1, Customer customer2)
        {
            int index1 = _customerToIndex[customer1.Id];
            int index2 = _customerToIndex[customer2.Id];
            return _distances[index1, index2];
        }

        /// <summary>
        /// Retrieves pre-calculated distance between depot and a customer.
        /// </summary>
        /// <param name="customer">Target customer</param>
        /// <returns>Distance from depot to customer</returns>
        public double GetDepotDistance(Customer customer)
        {
            int index = _customerToIndex[customer.Id];
            return _distances[_depotIndex, index];
        }

        /// <summary>
        /// Prints the distance matrix in a formatted table for debugging purposes.
        /// Shows depot (D) and customer (C) distances with labels.
        /// </summary>
        public void PrintMatrix()
        {
            int size = _distances.GetLength(0);
            
            // Print header row
            Console.Write("     D  "); // Depot header
            for (int i = 1; i < size; i++)
            {
                Console.Write($"  C{i:D2} "); // Customer headers
            }
            Console.WriteLine();
            
            // Print separator
            Console.WriteLine(new string('-', 6 * size + 4));
            
            // Print matrix rows
            for (int i = 0; i < size; i++)
            {
                if (i == 0)
                    Console.Write("D  | "); // Depot row label
                else
                    Console.Write($"C{i:D2}| "); // Customer row labels
                    
                for (int j = 0; j < size; j++)
                {
                    Console.Write($"{_distances[i,j]:F1} "); // Format distances to 1 decimal place
                }
                Console.WriteLine();
            }
        }
    }
}