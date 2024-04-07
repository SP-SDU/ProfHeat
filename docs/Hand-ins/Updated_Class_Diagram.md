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

Services: (Are named managers to keep consistency with the case)
- AssetManager
  + LoadAssets(): List<ProductionUnit>
  + SaveAssets(List<ProductionUnit> units): void

- SourceDataManager
  + LoadSourceData(): List<(HeatDemand, ElectricityPrice)>
  + SaveSourceData(List<(HeatDemand demand, ElectricityPrice price)> data): void

- ResultDataManager
  + SaveOptimizationResults(results: List<OptimizationResult>): void
  + LoadOptimizationResults(): List<OptimizationResult>

ViewModels:
- OptimizerViewModel
  --> Uses: AssetService, SourceDataService, ResultDataService, Optimizer
  + OptimizationResults: ObservableCollection<OptimizationResult>
  + OptimizeCommand(): void

- DataVisualizerViewModel
  --> Uses: SourceDataService
  Inherits: ViewModelBase
  + HeatDemands: ObservableCollection<HeatDemand>
  + ElectricityPrices: ObservableCollection<ElectricityPrice>
  + LoadDataCommand(): void

Views:
- OptimizerView
  --> Binds to: OptimizerViewModel

- DataVisualizerView
  --> Binds to: DataVisualizerViewModel