Models:
- ProductionUnit
  + Id: Guid
  + Name: string
  + MaxHeat: double
  + ProductionCost: double
  + MaxElectricity: double
  + CO2Emission: double
  + GasConsumption: double

- HeatDemand
  + Time: DateTime
  + DemandValue: double

- ElectricityPrice
  + Time: DateTime
  + Price: double

- OptimizationResult
  + Time: DateTime
  + OptimizedHeat: double
  + OptimizedCosts: double
  + CO2Emissions: double

- Optimizer (Business Logic/Model)
  + Optimize(assets: List<ProductionUnit>, demands: List<HeatDemand>, prices: List<ElectricityPrice>): List<OptimizationResult>
  --> Interacts with: ProductionUnit, HeatDemand, ElectricityPrice, OptimizationResult

Services: (Are named managers to keep consistency with the case)
- AssetManager
  + LoadAssets(): List<ProductionUnit>
  + SaveAssets(List<ProductionUnit> units): void
  --> Uses: ProductionUnit

- SourceDataManager
  + LoadSourceData(): List<(HeatDemand, ElectricityPrice)>
  + SaveSourceData(List<(HeatDemand demand, ElectricityPrice price)> data): void
  --> Uses: HeatDemand, ElectricityPrice

- ResultDataManager
  + SaveOptimizationResults(results: List<OptimizationResult>): void
  + LoadOptimizationResults(): List<OptimizationResult>
  --> Uses: OptimizationResult

ViewModels:
- OptimizerViewModel
  --> Uses: AssetManager, SourceDataManager, ResultDataManager, Optimizer
  + OptimizationResults: ObservableCollection<OptimizationResult>
  + OptimizeCommand(): void
  --> Binds to: OptimizationResult

- DataVisualizerViewModel
  --> Uses: SourceDataManager
  Inherits: ViewModelBase
  + HeatDemands: ObservableCollection<HeatDemand>
  + ElectricityPrices: ObservableCollection<ElectricityPrice>
  + LoadDataCommand(): void
  --> Binds to: HeatDemand, ElectricityPrice

Views:
- OptimizerView
  --> Binds to: OptimizerViewModel

- DataVisualizerView
  --> Binds to: DataVisualizerViewModel
