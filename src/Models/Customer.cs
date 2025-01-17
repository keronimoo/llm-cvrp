namespace CapacitatedVehicleRoutingProblem.Models
{
    /// <summary>
    /// Represents a customer node in the CVRP problem.
    /// Contains customer location coordinates, demand requirements, and unique identifier.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Unique identifier for the customer
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// X-coordinate of customer location
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y-coordinate of customer location
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Customer's demand quantity that must be satisfied
        /// </summary>
        public double Demand { get; set; }

        /// <summary>
        /// Initializes a new customer with specified location and demand.
        /// </summary>
        /// <param name="id">Unique identifier</param>
        /// <param name="x">X-coordinate location</param>
        /// <param name="y">Y-coordinate location</param>
        /// <param name="demand">Required demand quantity</param>
        public Customer(int id, double x, double y, double demand)
        {
            Id = id;
            X = x;
            Y = y;
            Demand = demand;
        }

        /// <summary>
        /// Provides string representation of customer for debugging and logging.
        /// </summary>
        /// <returns>String containing customer ID, location, and demand</returns>
        public override string ToString()
        {
            return $"Customer[Id={Id}, X={X}, Y={Y}, Demand={Demand}]";
        }
    }
}
