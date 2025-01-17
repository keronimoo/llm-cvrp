using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Utils
{
    /// <summary>
    /// Parses CVRP problem instances from standardized dataset files.
    /// Supports common CVRP benchmark format with sections for coordinates,
    /// demands, and depot information.
    /// </summary>
    public class DatasetParser
    {
        /// <summary>
        /// Parses a CVRP dataset file and extracts problem instance data.
        /// File format should contain:
        /// - Vehicle capacity
        /// - Node coordinates
        /// - Customer demands
        /// - Depot location
        /// </summary>
        /// <param name="filePath">Path to the dataset file</param>
        /// <returns>
        /// Tuple containing:
        /// - List of customers with locations and demands
        /// - List of vehicles with capacity
        /// - Depot location
        /// </returns>
        public static (List<Customer>, List<Vehicle>, Depot) ParseDataset(string filePath)
        {
            var customers = new List<Customer>();
            var vehicles = new List<Vehicle>();
            Depot depot = null;

            string[] lines = File.ReadAllLines(filePath);
            bool readingCoordinates = false;
            bool readingDemands = false;
            bool readingDepot = false;

            int capacity = 0;

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("CAPACITY"))
                {
                    // Extract vehicle capacity
                    capacity = int.Parse(trimmedLine.Split(':')[1].Trim());
                }
                else if (trimmedLine.StartsWith("NODE_COORD_SECTION"))
                {
                    readingCoordinates = true;
                    continue;
                }
                else if (trimmedLine.StartsWith("DEMAND_SECTION"))
                {
                    readingCoordinates = false;
                    readingDemands = true;
                    continue;
                }
                else if (trimmedLine.StartsWith("DEPOT_SECTION"))
                {
                    readingDemands = false;
                    readingDepot = true;
                    continue;
                }
                else if (trimmedLine.StartsWith("EOF"))
                {
                    break;
                }

                // Read node coordinates
                if (readingCoordinates)
                {
                    var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        int id = int.Parse(parts[0]);
                        double x = double.Parse(parts[1]);
                        double y = double.Parse(parts[2]);

                        customers.Add(new Customer(id, x, y, 0)); // Initialize demand as 0 for now
                    }
                }

                // Read demands
                if (readingDemands)
                {
                    var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        int id = int.Parse(parts[0]);
                        double demand = double.Parse(parts[1]);

                        var customer = customers.FirstOrDefault(c => c.Id == id);
                        if (customer != null)
                        {
                            customer.Demand = demand;
                        }
                    }
                }

                // Read depot information
                if (readingDepot)
                {
                    if (int.TryParse(trimmedLine, out int depotId) && depotId != -1)
                    {
                        var depotCustomer = customers.FirstOrDefault(c => c.Id == depotId);
                        if (depotCustomer != null)
                        {
                            depot = new Depot(depotCustomer.X, depotCustomer.Y);
                            customers.Remove(depotCustomer); // Remove depot from customer list
                        }
                    }
                }
            }

            // Initialize vehicles with the parsed capacity
            int numberOfVehicles = (int)Math.Ceiling(customers.Sum(c => c.Demand) / capacity);
            for (int i = 1; i <= numberOfVehicles; i++)
            {
                vehicles.Add(new Vehicle(i, capacity));
            }

            return (customers, vehicles, depot);
        }
    }
}
