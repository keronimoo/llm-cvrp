/// <summary>
/// Represents the central depot in the CVRP problem.
/// All vehicle routes must start and end at this location.
/// </summary>
namespace CapacitatedVehicleRoutingProblem.Models
{
    public class Depot
    {
        /// <summary>
        /// X-coordinate of depot location
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y-coordinate of depot location
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Initializes a new depot at specified coordinates.
        /// </summary>
        /// <param name="x">X-coordinate location</param>
        /// <param name="y">Y-coordinate location</param>
        public Depot(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Provides string representation of depot for debugging and logging.
        /// </summary>
        /// <returns>String containing depot coordinates</returns>
        public override string ToString()
        {
            return $"Depot[X={X}, Y={Y}]";
        }
    }
}
