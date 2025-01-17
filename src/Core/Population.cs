namespace CapacitatedVehicleRoutingProblem.Core
{
    using CapacitatedVehicleRoutingProblem.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Population
    {
        private List<List<Vehicle>> Solutions { get; set; }
        public int Size { get; }
        private readonly Random _random;
        private readonly List<Customer> _customers;
        private readonly List<Vehicle> _vehicleTemplates;

        public Population(int size, List<Customer> customers, List<Vehicle> vehicleTemplates)
        {
            Size = size;
            Solutions = new List<List<Vehicle>>(size);
            _random = new Random();
            _customers = customers;
            _vehicleTemplates = vehicleTemplates;
        }

        /// <summary>
        /// Generates initial population by creating random feasible solutions.
        /// Each solution ensures all customers are assigned while respecting vehicle capacities.
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < Size; i++)
            {
                var solution = GenerateRandomSolution();
                Solutions.Add(solution);
            }
        }

        /// <summary>
        /// Creates a single random solution by assigning customers to vehicles.
        /// </summary>
        /// <returns>A new feasible solution</returns>
        private List<Vehicle> GenerateRandomSolution()
        {
            // Create deep copies of vehicle templates
            var vehicles = _vehicleTemplates.Select(t => 
                new Vehicle(t.Id, t.Capacity)).ToList();

            // Create queue of randomly ordered customers
            var unassignedCustomers = new Queue<Customer>(
                _customers.OrderBy(x => _random.Next()));

            // Keep track of failed assignment attempts
            int maxAttempts = _customers.Count * 2;
            int attempts = 0;

            while (unassignedCustomers.Count > 0 && attempts < maxAttempts)
            {
                var customer = unassignedCustomers.Dequeue();
                bool assigned = false;

                // Try to add customer to a random vehicle
                foreach (var vehicle in vehicles.OrderBy(v => _random.Next()))
                {
                    if (vehicle.Load + customer.Demand <= vehicle.Capacity)
                    {
                        vehicle.AddCustomer(customer);
                        assigned = true;
                        break;
                    }
                }

                // If customer couldn't be assigned, put back in queue
                if (!assigned)
                {
                    unassignedCustomers.Enqueue(customer);
                    attempts++;
                }
                else
                {
                    attempts = 0; // Reset attempts counter on successful assignment
                }
            }

            // If there are still unassigned customers, use greedy approach
            if (unassignedCustomers.Count > 0)
            {
                while (unassignedCustomers.Count > 0)
                {
                    var customer = unassignedCustomers.Dequeue();
                    // Find vehicle with most remaining capacity
                    var targetVehicle = vehicles
                        .OrderByDescending(v => v.Capacity - v.Load)
                        .First();
                    targetVehicle.AddCustomer(customer);
                }
            }

            return vehicles;
        }

        // Get solution at specific index
        public List<Vehicle> GetSolution(int index)
        {
            return Solutions[index];
        }

        // Replace solution at specific index
        public void ReplaceSolution(int index, List<Vehicle> newSolution)
        {
            Solutions[index] = newSolution;
        }

        // Get best solution based on fitness function
        public List<Vehicle> GetBestSolution(Func<List<Vehicle>, double> fitnessFunction)
        {
            return Solutions.OrderBy(fitnessFunction).First();
        }

        // Get all solutions
        public List<List<Vehicle>> GetAllSolutions()
        {
            return Solutions;
        }

        /// <summary>
        /// Updates the entire population with a new set of solutions.
        /// Validates that the new population maintains the required size.
        /// </summary>
        /// <param name="newSolutions">New solutions to replace current population</param>
        /// <exception cref="ArgumentException">Thrown when new solutions count doesn't match population size</exception>
        public void UpdatePopulation(List<List<Vehicle>> newSolutions)
        {
            if (newSolutions.Count != Size)
            {
                throw new ArgumentException($"New population size ({newSolutions.Count}) does not match current size ({Size})");
            }
            Solutions = newSolutions;
        }
    }
}