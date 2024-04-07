  Models:
- HeatingUnit
  + Id: Guid
  + Name: string
  + Type: Enum (Gas, Oil, Electric)
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
  + Optimize(assets: List<HeatingUnit>, demands: List<HeatDemand>, prices: List<ElectricityPrice>): List<OptimizationResult>

Services: (Are named managers to keep consistency with the case)
- AssetManager
  --> Interacts with: CSVDataImportExportService
  + GetAllHeatingUnits(): List<HeatingUnit>
  + SaveHeatingUnit(unit: HeatingUnit): void

- SourceDataManager
  --> Interacts with: CSVDataImportExportService
  + GetHeatDemand(): List<HeatDemand>
  + GetElectricityPrice(): List<ElectricityPrice>

- ResultDataManager
  --> Interacts with: CSVDataImportExportService
  + SaveOptimizationResults(results: List<OptimizationResult>): void
  + GetOptimizationResults(): List<OptimizationResult>

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