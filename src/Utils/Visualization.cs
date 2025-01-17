using ScottPlot;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CapacitatedVehicleRoutingProblem.Models;
using CapacitatedVehicleRoutingProblem.Core;
using System.Drawing;
using System.Text;
using CapacitatedVehicleRoutingProblem.Utils;
using CapacitatedVehicleRoutingProblem.Models.Configurations;

namespace CapacitatedVehicleRoutingProblem.Utils
{
    public class Visualization
    {
        private Plot _plot;
        private readonly List<Customer> _customers;
        private readonly Depot _depot;
        private readonly Random _random;
        private readonly string _configDir;
        private readonly string _runDir;
        private int _highlightedVehicleIndex = -1;
        private readonly GeneticAlgorithmConfig _config;

        public Visualization(
            Depot depot, 
            List<Customer> customers, 
            string configDir,
            string runDir)
        {
            _depot = depot;
            _customers = customers;
            _random = new Random();
            _configDir = configDir;
            _runDir = runDir;

            DirectoryManager.EnsureDirectoryExists(_configDir);
            DirectoryManager.EnsureDirectoryExists(_runDir);
        }

        private void DrawArrow(Plot plt, double x1, double y1, double x2, double y2, Color color, bool isHighlighted)
        {
            // Calculate arrow parameters
            double dx = x2 - x1;
            double dy = y2 - y1;
            double angle = Math.Atan2(dy, dx);
            double length = Math.Sqrt(dx * dx + dy * dy);

            // Skip very short segments
            if (length < 10) return;

            // Draw main line
            var line = plt.AddLine(x1, y1, x2, y2);
            line.Color = color;
            line.LineWidth = isHighlighted ? 2 : 1;

            // Arrow head parameters
            double arrowSize = isHighlighted ? 15 : 10;
            double arrowAngle = Math.PI / 6; // 30 degrees

            // Calculate arrow head points
            double x3 = x2 - arrowSize * Math.Cos(angle - arrowAngle);
            double y3 = y2 - arrowSize * Math.Sin(angle - arrowAngle);
            double x4 = x2 - arrowSize * Math.Cos(angle + arrowAngle);
            double y4 = y2 - arrowSize * Math.Sin(angle + arrowAngle);

            // Draw arrow head
            var arrowLine1 = plt.AddLine(x2, y2, x3, y3);
            var arrowLine2 = plt.AddLine(x2, y2, x4, y4);
            arrowLine1.Color = color;
            arrowLine2.Color = color;
            arrowLine1.LineWidth = isHighlighted ? 2 : 1;
            arrowLine2.LineWidth = isHighlighted ? 2 : 1;
        }

        public void DisplaySolution(List<Vehicle> solution, int generation, double cost)
        {
            var plt = new Plot();
            plt.Title($"Vehicle Routes (Generation {generation}, Cost: {cost:F2})");

            // Plot depot
            var depotScatter = plt.AddScatter(
                new[] { _depot.X },
                new[] { _depot.Y });
            depotScatter.MarkerSize = 10;
            depotScatter.MarkerShape = MarkerShape.filledSquare;
            depotScatter.Color = Color.Black;
            depotScatter.Label = "Depot";

            // Plot customers
            var customerScatter = plt.AddScatter(
                _customers.Select(c => (double)c.X).ToArray(),
                _customers.Select(c => (double)c.Y).ToArray());
            customerScatter.MarkerSize = 5;
            customerScatter.MarkerShape = MarkerShape.filledCircle;
            customerScatter.Color = Color.Gray;
            customerScatter.Label = "Customers";

            // Generate distinct colors for routes
            var colors = GenerateDistinctColors(solution.Count);

            // Plot routes for each vehicle
            for (int i = 0; i < solution.Count; i++)
            {
                var vehicle = solution[i];
                bool isHighlighted = (i == _highlightedVehicleIndex);
                Color routeColor = isHighlighted ? Color.Red : colors[i];

                if (vehicle.Route.Count > 0)
                {
                    // Draw route segments with arrows
                    var points = new List<(double X, double Y)>();
                    points.Add((_depot.X, _depot.Y)); // Start at depot
                    points.AddRange(vehicle.Route.Select(c => ((double)c.X, (double)c.Y)));
                    points.Add((_depot.X, _depot.Y)); // Return to depot

                    // Draw arrows between consecutive points
                    for (int j = 0; j < points.Count - 1; j++)
                    {
                        DrawArrow(plt,
                            points[j].X, points[j].Y,
                            points[j + 1].X, points[j + 1].Y,
                            routeColor, isHighlighted);
                    }

                    // Add route label
                    var routeLabel = plt.AddScatter(
                        new[] { points[0].X },
                        new[] { points[0].Y });
                    routeLabel.Color = routeColor;
                    routeLabel.Label = $"Vehicle {vehicle.Id} (Load: {vehicle.Load:F1}/{vehicle.Capacity:F1})";
                    routeLabel.MarkerSize = 0; 
                }
            }

            plt.Legend();

            // Save to run-specific directory
            string filePath = Path.Combine(_runDir, $"generation_{generation:D4}.png");
            plt.SaveFig(filePath);
        }

        private Color[] GenerateDistinctColors(int count)
        {
            var colors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                // Use HSV color space for better distinction
                double hue = i * (360.0 / count);
                double saturation = 0.7;
                double value = 0.9;

                // Convert HSV to RGB
                colors[i] = HsvToRgb(hue, saturation, value);
            }
            return colors;
        }

        private Color HsvToRgb(double hue, double saturation, double value)
        {
            int hi = (int)(hue / 60) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            double v = value;
            double p = value * (1 - saturation);
            double q = value * (1 - f * saturation);
            double t = value * (1 - (1 - f) * saturation);

            if (hi == 0)
                return Color.FromArgb((int)v, (int)t, (int)p);
            else if (hi == 1)
                return Color.FromArgb((int)q, (int)v, (int)p);
            else if (hi == 2)
                return Color.FromArgb((int)p, (int)v, (int)t);
            else if (hi == 3)
                return Color.FromArgb((int)p, (int)q, (int)v);
            else if (hi == 4)
                return Color.FromArgb((int)t, (int)p, (int)v);
            else
                return Color.FromArgb((int)v, (int)p, (int)q);
        }

        public void SetHighlightedVehicle(int vehicleIndex)
        {
            _highlightedVehicleIndex = vehicleIndex;
        }

        public static void CreateConvergencePlot(string configDir, List<List<double>> convergenceData)
        {
            DirectoryManager.EnsureDirectoryExists(configDir);

            var plt = new Plot();
            plt.Title("Convergence Over Generations");
            plt.XLabel("Generation");
            plt.YLabel("Best Solution Cost");

            for (int i = 0; i < convergenceData.Count; i++)
            {
                var run = convergenceData[i];
                var line = plt.AddScatter(
                    Enumerable.Range(0, run.Count).Select(x => (double)x).ToArray(),
                    run.ToArray());
                line.Label = $"Run {i + 1}";
            }

            plt.Legend();
            plt.SaveFig(Path.Combine(configDir, "convergence_plot.png"));
        }
    }
}
