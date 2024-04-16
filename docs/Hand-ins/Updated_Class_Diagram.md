Models:
- ProductionUnit
  + Id: Guid
  + Name: string
  + MaxHeat: double
  + ProductionCost: double
  + MaxElectricity: double
  + CO2Emission: double
  + GasConsumption: double

- SourceData
  + TimeFrom: DateTime
  + TimeTo: DateTime
  + DemandValue: double
  + Price: double

- ResultData
  + TimeFrom: DateTime
  + TimeTo: DateTime
  + OptimizedHeat: double
  + OptimizedCosts: double
  + CO2Emissions: double

- Optimizer (Business Logic/Model)
  + Optimize(assets: List<ProductionUnit>, demands and prices: List<SourceData>): List<ResultData>
  --> Interacts with: ProductionUnit, SourceData, ResultData

Services: (Are named managers to keep consistency with the case)
- AssetManager
  + LoadAssets(): List<ProductionUnit>
  + SaveAssets(List<ProductionUnit> units): void
  --> Uses: ProductionUnit

- SourceDataManager
  + LoadSourceData(): List<SourceData>
  + SaveSourceData(List<SourceData> data): void
  --> Uses: SourceData

- ResultDataManager
  + SaveOptimizationResults(results: List<ResultData>): void
  + LoadOptimizationResults(): List<ResultData>
  --> Uses: ResultData

ViewModels:
- OptimizerViewModel
  --> Uses: AssetManager, SourceDataManager, ResultDataManager, Optimizer
  + OptimizationResults: ObservableCollection<ResultData>
  + OptimizeCommand(): void
  --> Binds to: ResultData

- DataVisualizerViewModel
  --> Uses: SourceDataManager
  Inherits: ViewModelBase
  + SourceData: ObservableCollection<SourceData>
  + LoadDataCommand(): void
  --> Binds to: SourceData

Views:
- OptimizerView
  --> Binds to: OptimizerViewModel

- DataVisualizerView
  --> Binds to: DataVisualizerViewModel
