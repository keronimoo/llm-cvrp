namespace CapacitatedVehicleRoutingProblem.Models
{
    /// <summary>
    /// Represents a vehicle in the CVRP problem, maintaining its route and capacity constraints.
    /// Each vehicle has a unique ID, maximum capacity, current load, and ordered list of customers to visit.
    /// </summary>
    public class Vehicle
    {
        public int Id { get; }
        public double Capacity { get; }
        public double Load { get; set; }
        public List<Customer> Route { get; }

        /// <summary>
        /// Initializes a new vehicle with specified capacity and optional initial route.
        /// </summary>
        /// <param name="id">Unique identifier for the vehicle</param>
        /// <param name="capacity">Maximum carrying capacity</param>
        /// <param name="route">Optional initial route of customers</param>
        public Vehicle(int id, double capacity, List<Customer> route = null)
        {
            Id = id;
            Capacity = capacity;
            Route = route ?? new List<Customer>();
            UpdateLoad();
        }

        /// <summary>
        /// Attempts to add a customer to the vehicle's route while respecting capacity constraints.
        /// </summary>
        /// <param name="customer">Customer to be added to the route</param>
        /// <returns>True if customer was successfully added, false if duplicate or capacity exceeded</returns>
        public bool AddCustomer(Customer customer)
        {
            if (Route.Contains(customer))
                return false; // Still prevent duplicates
            
            Route.Add(customer);
            Load += customer.Demand;
            return true;
        }

        /// <summary>
        /// Removes all customers from the route and resets the load to zero.
        /// Useful for reinitializing routes during solution modification.
        /// </summary>
        public void ClearRoute()
        {
            Route.Clear();
            Load = 0;
        }

        public override string ToString()
        {
            return $"Vehicle[Id={Id}, Capacity={Capacity}, Load={Load}, Route=[{string.Join(", ", Route)}]]";
        }

        /// <summary>
        /// Updates the current load based on the demands of all customers in the route.
        /// This method should be called after any route modifications to maintain consistency.
        /// </summary>
        public void UpdateLoad()
        {
            Load = Route.Sum(c => c.Demand);
        }
    }
}
