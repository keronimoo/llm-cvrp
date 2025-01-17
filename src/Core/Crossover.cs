using System;
using System.Collections.Generic;
using System.Linq;
using CapacitatedVehicleRoutingProblem.Models;

namespace CapacitatedVehicleRoutingProblem.Core
{
    /// <summary>
    /// Provides various crossover operators for the CVRP genetic algorithm.
    /// Each operator implements different strategies for combining parent solutions
    /// while maintaining solution feasibility.
    /// </summary>
    public static class Crossover
    {
        /// <summary>
        /// Random number generator used for crossover operations.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Implements Order Crossover (OX) operator which preserves relative order of customers
        /// while maintaining route feasibility and capacity constraints.
        /// </summary>
        /// <param name="parent1">First parent solution containing vehicle routes</param>
        /// <param name="parent2">Second parent solution containing vehicle routes</param>
        /// <returns>A new feasible offspring solution combining characteristics from both parents</returns>
        public static List<Vehicle> OrderCrossover(List<Vehicle> parent1, List<Vehicle> parent2)
        {
            var offspring = new List<Vehicle>();
            var unassignedCustomers = new HashSet<Customer>(
                parent1.SelectMany(v => v.Route));

            // Initialize empty vehicles
            foreach (var template in parent1)
            {
                offspring.Add(new Vehicle(template.Id, template.Capacity));
            }

            // First phase: Try to keep parent1's assignments where possible
            for (int i = 0; i < parent1.Count; i++)
            {
                foreach (var customer in parent1[i].Route)
                {
                    if (unassignedCustomers.Contains(customer) &&
                        offspring[i].Load + customer.Demand <= offspring[i].Capacity)
                    {
                        offspring[i].Route.Add(customer);
                        offspring[i].UpdateLoad();
                        unassignedCustomers.Remove(customer);
                    }
                }
            }

            // Second phase: Insert remaining customers following parent2's order
            var remainingCustomers = parent2.SelectMany(v => v.Route)
                                          .Where(c => unassignedCustomers.Contains(c))
                                          .ToList();

            foreach (var customer in remainingCustomers)
            {
                // Try all vehicles in order of available capacity
                var targetVehicle = offspring
                    .Where(v => v.Load + customer.Demand <= v.Capacity)
                    .OrderByDescending(v => v.Capacity - v.Load)
                    .FirstOrDefault();

                if (targetVehicle == null)
                {
                    // If no vehicle has capacity, find the one closest to depot
                    targetVehicle = offspring.OrderBy(v => v.Route.Count).First();
                }

                targetVehicle.Route.Add(customer);
                targetVehicle.UpdateLoad();
                unassignedCustomers.Remove(customer);
            }

            return offspring;
        }

        /// <summary>
        /// Implements Edge Recombination Crossover which attempts to preserve customer-to-customer
        /// connections (edges) from both parents while building new routes. This operator focuses on
        /// maintaining beneficial adjacency relationships between customers from both parent solutions.
        /// </summary>
        /// <param name="parent1">First parent solution containing vehicle routes</param>
        /// <param name="parent2">Second parent solution containing vehicle routes</param>
        /// <returns>A new feasible offspring solution that preserves edge relationships from parents</returns>
        public static List<Vehicle> EdgeRecombinationCrossover(List<Vehicle> parent1, List<Vehicle> parent2)
        {
            //Console.WriteLine("DEBUG:        Starting Edge Recombination Crossover");
            var offspring = new List<Vehicle>();
            var unassignedCustomers = new HashSet<Customer>(parent1.SelectMany(v => v.Route));
            //Console.WriteLine($"DEBUG:        Total customers to assign: {unassignedCustomers.Count}");

            // Initialize empty vehicles
            foreach (var template in parent1)
            {
                offspring.Add(new Vehicle(template.Id, template.Capacity));
            }
            //Console.WriteLine($"DEBUG:        Initialized {offspring.Count} empty vehicles");

            // Build edge map: For each customer, store all its neighbors from both parents
            var edgeMap = new Dictionary<Customer, HashSet<Customer>>();
            BuildEdgeMap(parent1, edgeMap, true);
            BuildEdgeMap(parent2, edgeMap, true);

            // Debug: Print edge map contents
            //Console.WriteLine("DEBUG:        Edge Map contents:");
            foreach (var kvp in edgeMap)
            {
                var customer = kvp.Key;
                var neighbors = kvp.Value;
                //Console.WriteLine($"DEBUG:        Customer {customer?.Id}: Connected to {string.Join(", ", neighbors.Select(n => n?.Id.ToString() ?? "Depot"))}");
            }

            // Set maximum attempts to avoid infinite loops
            int maxAttempts = unassignedCustomers.Count * 2;
            int attempts = 0;

            while (unassignedCustomers.Any() && attempts < maxAttempts)
            {
                attempts++;
                //Console.WriteLine($"\nDEBUG:        Attempt {attempts}/{maxAttempts}");
                
                // Find vehicle with most remaining capacity to maximize load balancing
                var currentVehicle = offspring
                    .OrderByDescending(v => v.Capacity - v.Load)
                    .First();

                //Console.WriteLine($"DEBUG:        Selected vehicle {currentVehicle.Id} (Load: {currentVehicle.Load}/{currentVehicle.Capacity})");

                // Skip if vehicle is full
                if (currentVehicle.Load >= currentVehicle.Capacity)
                {
                    Console.WriteLine("DEBUG:        Vehicle is full, skipping");
                    continue;
                }

                // Select customer with smallest demand as starting point for better capacity utilization
                var current = unassignedCustomers
                    .OrderBy(c => c.Demand)
                    .FirstOrDefault();

                if (current == null)
                {
                    //Console.WriteLine("DEBUG:        No more customers to assign");
                    break;
                }

                //Console.WriteLine($"DEBUG:        Starting with customer {current.Id} (Demand: {current.Demand})");

                // Build route for current vehicle
                while (current != null && currentVehicle.Load + current.Demand <= currentVehicle.Capacity)
                {
                    // Add customer to route
                    currentVehicle.Route.Add(current);
                    currentVehicle.UpdateLoad();
                    unassignedCustomers.Remove(current);
                    
                    //Console.WriteLine($"DEBUG:        Added customer {current.Id} to vehicle {currentVehicle.Id}");
                    //Console.WriteLine($"DEBUG:        Vehicle load now: {currentVehicle.Load}/{currentVehicle.Capacity}");
                    //Console.WriteLine($"DEBUG:        Remaining unassigned customers: {unassignedCustomers.Count}");

                    if (!unassignedCustomers.Any())
                    {
                        //Console.WriteLine("DEBUG:        All customers assigned");
                        break;
                    }

                    // Find next feasible customer:
                    // - Must be unassigned
                    // - Must be connected to current customer in edge map
                    // - Must fit in current vehicle's remaining capacity
                    var feasibleNeighbors = edgeMap[current]
                        .Where(n => unassignedCustomers.Contains(n) && 
                                   currentVehicle.Load + n.Demand <= currentVehicle.Capacity)
                        .ToList();

                    //Console.WriteLine($"DEBUG:        Found {feasibleNeighbors.Count} feasible neighbors for customer {current.Id}");
                    //Console.WriteLine($"DEBUG:        Feasible neighbors: {string.Join(", ", feasibleNeighbors.Select(n => n.Id))}");
                    
                    if (feasibleNeighbors.Any())
                    {
                        // Choose neighbor with fewest remaining connections to maintain edge preservation
                        var nextCustomer = feasibleNeighbors
                            .OrderBy(n => edgeMap[n].Count(x => unassignedCustomers.Contains(x)))
                            .First();
                        //Console.WriteLine($"DEBUG:        Selected next customer {nextCustomer.Id} with {edgeMap[nextCustomer].Count(x => unassignedCustomers.Contains(x))} remaining connections");
                        current = nextCustomer;
                    }
                    else
                    {
                        //Console.WriteLine("DEBUG:        No feasible neighbors found, ending current route");
                        current = null;
                    }
                }
            }

            // Handle any remaining customers using greedy assignment
            if (unassignedCustomers.Any())
            {
                //Console.WriteLine($"\nDEBUG:        Handling {unassignedCustomers.Count} remaining customers with greedy assignment");
                foreach (var customer in unassignedCustomers.OrderBy(c => c.Demand))
                {
                    var targetVehicle = offspring
                        .OrderByDescending(v => v.Capacity - v.Load)
                        .First();
                    targetVehicle.Route.Add(customer);
                    targetVehicle.UpdateLoad();
                    //Console.WriteLine($"DEBUG:        Assigned customer {customer.Id} to vehicle {targetVehicle.Id} (Load: {targetVehicle.Load}/{targetVehicle.Capacity})");
                }
            }

            // Console.WriteLine("\nDEBUG:        Final solution:");
            // foreach (var vehicle in offspring)
            // {
            //     Console.WriteLine($"DEBUG:        Vehicle {vehicle.Id}: {vehicle.Load}/{vehicle.Capacity} - Customers: {string.Join(", ", vehicle.Route.Select(c => c.Id))}");
            // }

            return offspring;
        }

        /// <summary>
        /// Builds a map of edges (connections) between customers from a given solution.
        /// Each customer is mapped to a set of its neighbors in the routes.
        /// </summary>
        /// <param name="solution">Source solution to extract edges from</param>
        /// <param name="edgeMap">Dictionary to store customer connections, where key is a customer and value is set of neighboring customers</param>
        /// <param name="includeDepot">If true, includes connections to depot (represented as null) in the edge map</param>
        private static void BuildEdgeMap(List<Vehicle> solution, Dictionary<Customer, HashSet<Customer>> edgeMap, bool includeDepot = false)
        {
            //Console.WriteLine($"DEBUG:        Building edge map for solution with {solution.Count} vehicles");
            
            // Process each vehicle's route
            foreach (var vehicle in solution)
            {
                if (vehicle.Route.Count == 0) continue;

                // Initialize sets for new customers
                for (int i = 0; i < vehicle.Route.Count; i++)
                {
                    var customer = vehicle.Route[i];
                    if (!edgeMap.ContainsKey(customer))
                    {
                        edgeMap[customer] = new HashSet<Customer>();
                        //Console.WriteLine($"DEBUG:        Initialized edge set for customer {customer.Id}");
                    }
                }

                // Add edges between consecutive customers in route
                for (int i = 0; i < vehicle.Route.Count - 1; i++)
                {
                    var current = vehicle.Route[i];
                    var next = vehicle.Route[i + 1];
                    
                    edgeMap[current].Add(next);
                    edgeMap[next].Add(current);
                    //Console.WriteLine($"DEBUG:        Added edge between customers {current.Id} and {next.Id}");
                }

                // Add depot connections if requested
                if (includeDepot && vehicle.Route.Count > 0)
                {
                    // Connect first customer to depot (null represents depot)
                    var firstCustomer = vehicle.Route[0];
                    edgeMap[firstCustomer].Add(null);
                    //Console.WriteLine($"DEBUG:        Connected customer {firstCustomer.Id} to depot");

                    // Connect last customer to depot
                    var lastCustomer = vehicle.Route[vehicle.Route.Count - 1];
                    edgeMap[lastCustomer].Add(null);
                    //Console.WriteLine($"DEBUG:        Connected customer {lastCustomer.Id} to depot");
                }
            }
        }

        /// <summary>
        /// Implements Partially Mapped Crossover (PMX) which creates offspring by preserving vehicle assignments
        /// and customer order segments from both parents while maintaining feasibility through a mapping mechanism.
        /// Includes a local optimization phase to balance vehicle loads.
        /// </summary>
        /// <param name="parent1">First parent solution containing vehicle routes</param>
        /// <param name="parent2">Second parent solution containing vehicle routes</param>
        /// <returns>A new feasible offspring solution that preserves ordered segments from both parents</returns>
        /// <remarks>
        /// The PMX operator works in several phases:
        /// 1. Creates a mapping between corresponding customers in selected route segments
        /// 2. Attempts to maintain original vehicle assignments while respecting capacity
        /// 3. Assigns remaining customers using best-fit approach
        /// </remarks>
        public static List<Vehicle> PartiallyMappedCrossover(List<Vehicle> parent1, List<Vehicle> parent2)
        {
            // Initialize offspring with empty vehicles
            var offspring = parent1.Select(v => new Vehicle(v.Id, v.Capacity)).ToList();
            var unassignedCustomers = new HashSet<Customer>(parent1.SelectMany(v => v.Route));
            
            // Create initial mapping between customers in corresponding positions
            var mapping = new Dictionary<Customer, Customer>();
            var vehicleAssignments = new Dictionary<Customer, int>();
            
            // Record original vehicle assignments from parent1
            for (int i = 0; i < parent1.Count; i++)
            {
                foreach (var customer in parent1[i].Route)
                {
                    vehicleAssignments[customer] = i;
                }
            }

            // Process each vehicle route
            for (int vehicleIdx = 0; vehicleIdx < parent1.Count; vehicleIdx++)
            {
                var route1 = parent1[vehicleIdx].Route;
                var route2 = parent2[vehicleIdx].Route;
                
                if (route1.Count == 0 || route2.Count == 0) continue;
                
                // Select crossover points
                int crossPoint1 = random.Next(route1.Count);
                int crossPoint2 = random.Next(crossPoint1, route1.Count);
                
                // Build mapping from crossover segment
                for (int j = crossPoint1; j <= crossPoint2 && j < Math.Min(route1.Count, route2.Count); j++)
                {
                    mapping[route1[j]] = route2[j];
                    mapping[route2[j]] = route1[j];
                }
            }

            // Helper function to resolve mapping chains
            Customer ResolveMappedCustomer(Customer customer)
            {
                var current = customer;
                var visited = new HashSet<Customer> { current };
                
                while (mapping.ContainsKey(current))
                {
                    var next = mapping[current];
                    if (visited.Contains(next)) break; // Prevent infinite loops
                    current = next;
                    visited.Add(current);
                }
                
                return current;
            }

            // First pass: Try to maintain original vehicle assignments while respecting capacity
            foreach (var vehicleIdx in Enumerable.Range(0, parent1.Count))
            {
                var originalRoute = parent1[vehicleIdx].Route;
                foreach (var customer in originalRoute)
                {
                    var mappedCustomer = ResolveMappedCustomer(customer);
                    
                    if (unassignedCustomers.Contains(mappedCustomer) && 
                        offspring[vehicleIdx].Load + mappedCustomer.Demand <= offspring[vehicleIdx].Capacity)
                    {
                        offspring[vehicleIdx].Route.Add(mappedCustomer);
                        offspring[vehicleIdx].UpdateLoad();
                        unassignedCustomers.Remove(mappedCustomer);
                    }
                }
            }

            // Second pass: Assign remaining customers using best-fit
            var remainingCustomers = unassignedCustomers.OrderByDescending(c => c.Demand).ToList();
            foreach (var customer in remainingCustomers)
            {
                // Find vehicle with sufficient remaining capacity
                var targetVehicle = offspring
                    .Where(v => v.Load + customer.Demand <= v.Capacity)
                    .OrderBy(v => v.Load)
                    .FirstOrDefault();

                if (targetVehicle != null)
                {
                    targetVehicle.Route.Add(customer);
                    targetVehicle.UpdateLoad();
                }
                else
                {
                    // If no vehicle has sufficient capacity, find the one closest to feasible
                    targetVehicle = offspring.OrderBy(v => v.Load).First();
                    targetVehicle.Route.Add(customer);
                    targetVehicle.UpdateLoad();
                }
            }

            // Try to balance loads between vehicles
            bool improved;
            do
            {
                improved = false;
                for (int i = 0; i < offspring.Count; i++)
                {
                    if (offspring[i].Load > offspring[i].Capacity)
                    {
                        // Try to move customers to less loaded vehicles
                        for (int j = offspring[i].Route.Count - 1; j >= 0; j--)
                        {
                            var customer = offspring[i].Route[j];
                            var targetVehicle = offspring
                                .Where(v => v != offspring[i] && v.Load + customer.Demand <= v.Capacity)
                                .OrderBy(v => v.Load)
                                .FirstOrDefault();

                            if (targetVehicle != null)
                            {
                                offspring[i].Route.RemoveAt(j);
                                offspring[i].UpdateLoad();
                                targetVehicle.Route.Add(customer);
                                targetVehicle.UpdateLoad();
                                improved = true;
                            }
                        }
                    }
                }
            } while (improved);

            return offspring;
        }
    }
}
