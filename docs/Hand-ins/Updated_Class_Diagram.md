Models:
- HeatingGrid
  + Name: string
  + ImagePath: string
  + Buildings: int
  + ProductionUnits: List<ProductionUnit>
  --> XML Annotated for serialization
  --> Contains ProductionUnit (Aggregation)

- ProductionUnit
  + Name: string
  + ImagePath: string
  + MaxHeat: double
  + ProductionCost: double
  + CO2Emissions: double
  + GasConsumption: double
  + MaxElectricity: double

- MarketCondition
  + TimeFrom: DateTime
  + TimeTo: DateTime
  + HeatDemand: double
  + ElectricityPrice: double

- OptimizationResult
  + UnitName: string
  + TimeFrom: DateTime
  + TimeTo: DateTime
  + ProducedHeat: double
  + ElectricityProduced: double
  + GasConsumption: double
  + Costs: double
  + CO2Emissions: double

Repositories:
- AssetManager (implements IAssetManager)
  + LoadAssets(): HeatingGrid
  + SaveAssets(HeatingGrid grid): void
  --> Uses HeatingGrid (Association)

- SourceDataManager (implements ISourceDataManager)
  + LoadSourceData(string filePath): List<MarketCondition>
  + SaveSourceData(List<MarketCondition> data, string filePath): void
  --> Uses MarketCondition (Association)

- ResultDataManager (implements IResultDataManager)
  + LoadResultData(string filePath): List<OptimizationResult>
  + SaveResultData(List<OptimizationResult> data, string filePath): void
  --> Uses OptimizationResult (Association)

Optimization Logic:
- Optimizer (implements IOptimizer)
  + Optimize(productionUnits: List<ProductionUnit>, marketConditions: List<MarketCondition>): List<OptimizationResult>
  --> Uses: ProductionUnit, MarketCondition (Dependency)
  --> Outputs: OptimizationResult (Dependency)

ViewModels:
- OptimizerViewModel
  + CheckBoxItems: ObservableCollection<CheckBoxItem>
  + GridName: string
  + GridImagePath: string
  + ImportData(filePath: string): Task
  + Optimize(): void
  --> Uses: IAssetManager, ISourceDataManager, IOptimizer (Dependency)
  --> Manages: HeatingGrid, OptimizationResult (Association)

- DataVisualizerViewModel
  + Results: List<OptimizationResult>
  + SelectedPeriod: string
  + Periods: ObservableCollection<string>
  + Costs: ObservableCollection<ISeries>
  + CO2Emissions: ObservableCollection<ISeries>
  + ProducedHeat: ObservableCollection<ISeries>
  + GasConsumption: ObservableCollection<ISeries>
  + ElectricityProduced: ObservableCollection<ISeries>
  + ImportResults(filePath: string): Task
  + ExportResults(filePath: string): Task
  --> Uses: IResultDataManager (Dependency)
  --> Displays: OptimizationResult (Association)

Views:
- OptimizerView
  + CheckBoxItems for selecting production units
  + Buttons for Importing data and Optimizing
  + Display fields for GridName and GridImage
  --> Binds to: OptimizerViewModel (Association)

- DataVisualizerView
  + TabControl containing charts
  + ComboBox for selecting Periods
  + Buttons for Importing and Exporting results
  + Visual representations of data through charts
  --> Binds to: DataVisualizerViewModel (Association)
