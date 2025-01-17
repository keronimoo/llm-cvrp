# Capacitated Vehicle Routing Problem (CVRP) Solver

This project implements a genetic algorithm-based solver for the Capacitated Vehicle Routing Problem (CVRP). The solver uses various genetic operators and configurations to find efficient routing solutions while respecting vehicle capacity constraints.

## Overview

The CVRP solver implements:
- Multiple crossover operators (Edge Recombination, Order, Partially Mapped)
- Multiple mutation operators (Scramble, Swap, Insert) 
- Tournament selection with elitism
- Capacity-constrained route optimization
- Visualization of routes and convergence

## Results

### Configuration Performance Statistics

The following table shows the performance statistics for different genetic algorithm configurations:

|PopulationSize|CrossoverOperator|MutationOperator|SelectionOperator  |ReplacementOperator|TournamentSize|EliteCount|CrossoverRate|MutationRate|MaxGenerations|FitnessFunction              |Best    |Worst   |Mean    |StdDev |
|--------------|-----------------|----------------|-------------------|-------------------|--------------|----------|-------------|------------|--------------|-----------------------------|--------|--------|--------|-------|
|100           |EdgeRecombination|Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|21098.87|25679.16|23174.59|1258.24|
|100           |EdgeRecombination|Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|19781.61|23990.55|21761.39|1230.03|
|100           |EdgeRecombination|Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|20046.56|23480.25|21873.29|1098.72|
|100           |Order            |Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|26305.64|30897.69|29730.66|1434.40|
|100           |Order            |Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|27838.28|30806.70|29025.44|909.01 |
|100           |Order            |Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|23353.16|26183.29|24763.18|904.62 |
|100           |PartiallyMapped  |Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|23970.59|28991.24|26426.97|1734.06|
|100           |PartiallyMapped  |Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|22760.58|29240.60|26430.59|1715.13|
|100           |PartiallyMapped  |Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|22464.32|25609.67|24148.83|911.61 |
|150           |EdgeRecombination|Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|18831.01|23348.96|21228.96|1442.26|
|150           |EdgeRecombination|Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|17920.49|23030.84|20605.05|1684.30|
|150           |EdgeRecombination|Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|18572.06|21352.68|19813.74|917.76 |
|150           |Order            |Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|27896.91|31166.03|29831.13|1187.27|
|150           |Order            |Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|28172.73|30356.20|29071.78|756.47 |
|150           |Order            |Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|23436.51|25617.02|24192.58|585.87 |
|150           |PartiallyMapped  |Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|23730.81|28638.17|25954.90|1377.05|
|150           |PartiallyMapped  |Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|23269.25|27640.34|25359.87|1227.16|
|150           |PartiallyMapped  |Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|21557.40|25361.99|23500.95|1089.35|
|200           |EdgeRecombination|Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|16006.74|23403.14|19621.99|1939.36|
|200           |EdgeRecombination|Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|17521.36|20677.69|19036.30|872.65 |
|200           |EdgeRecombination|Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|17939.85|20376.80|18965.13|726.17 |
|200           |Order            |Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|27914.40|31025.97|29372.86|860.48 |
|200           |Order            |Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|26032.30|29900.63|28440.38|1009.67|
|200           |Order            |Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|22258.64|24885.24|23325.29|784.40 |
|200           |PartiallyMapped  |Scramble        |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|22975.93|27272.93|24684.03|1352.51|
|200           |PartiallyMapped  |Swap            |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|22456.93|27119.00|24685.77|1420.70|
|200           |PartiallyMapped  |Insert          |TournamentSelection|ElitistReplacement |2             |10        |0.85         |0.5         |320           |CalculateDistanceWithCapacity|22154.96|25253.60|23571.94|839.15 |


### Best Configuration Performance

The best performing configuration achieved the following results:
- Population Size: 200
- Crossover: Edge Recombination
- Mutation: Swap
- Best Solution Cost: 17521.36

#### Convergence Plot

![Convergence Plot](docs/images/convergence_plot.png)

*Convergence plot showing best and average solution costs over generations*

#### Initial Solution Routes (Generation 0)

![Initial Routes](docs/images/generation_0000.png)

*Vehicle routes from first generation of best configuration*

#### Final Solution Routes 

![Final Routes](docs/images/generation_0300.png)

*Optimized vehicle routes from final generation*

## Implementation Details

The solver implements the following key components:

### Benchmark Dataset

This implementation uses the XML100_2144_06 instance from the CVRPLIB benchmark library:
- Generated by Queiroga, Sadykov, Uchoa, and Vidal (2021)
- Problem Type: CVRP (Capacitated Vehicle Routing Problem)
- Dimension: 101 nodes (100 customers + 1 depot)
- Distance Calculation: Euclidean 2D
- Vehicle Capacity: 702 units

### Population Initialization

Initial population is generated through a randomized construction heuristic:
- Creates specified number of solutions with randomly ordered customers
- Assigns customers to vehicles while respecting capacity constraints
- Uses greedy approach for remaining assignments when needed

### Genetic Operators

**Crossover Methods:**
- Edge Recombination Crossover: Preserves edge relationships between customers
- Order Crossover: Maintains relative ordering of customer visits
- Partially Mapped Crossover: Preserves vehicle assignments while maintaining feasibility

**Mutation Methods:**
- Scramble Mutation: Randomly reorders subsequences of customers
- Swap Mutation: Exchanges positions of two random customers
- Insert Mutation: Moves a customer to a new position in the route

### Selection and Replacement

- Tournament Selection: Randomly samples solutions and selects the best
- Elitist Replacement: Preserves best solutions across generations

### Constraint Handling

- Vehicle capacity constraints enforced during solution construction
- Infeasible solutions penalized in fitness evaluation
- Load balancing between vehicles optimized

## Project Structure

The project is organized into the following structure:

```
src/
├── Core/                   # Core genetic algorithm components
│   ├── Crossover.cs       # Crossover operators
│   ├── Mutation.cs        # Mutation operators
│   ├── Population.cs      # Population management
│   ├── Selection.cs       # Selection operators
│   ├── Fitness.cs         # Fitness evaluation
│   ├── GeneticAlgorithm.cs # Main GA implementation
│   ├── DistanceMatrix.cs  # Distance calculations
│   └── Replacement.cs     # Replacement strategies
├── Models/                 # Problem domain models
│   ├── Customer.cs        # Customer representation
│   ├── Vehicle.cs         # Vehicle representation
│   ├── Depot.cs           # Depot representation
│   ├── Configurations/    # Configuration models
│   │   └── GeneticAlgorithmConfig.cs
│   └── Statistics/        # Statistics models
│       ├── RunStatistics.cs
│       └── ConfigurationStatistics.cs
└── Utils/                 # Utility classes
    ├── Visualization.cs   # Solution visualization
    ├── StatisticsHelper.cs # Statistical calculations
    ├── StatisticsWriter.cs # Statistics output
    ├── DirectoryManager.cs # Directory management
    └── DatasetParser.cs   # Dataset parsing
```
