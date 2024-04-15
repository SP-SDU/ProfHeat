// Copyright 2024 SoftFuzz
//
// Licensed under the Apache License, Version 2.0 (the "License"):
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProfHeat.Core.Models;
using ProfHeat.DAL.Interfaces;
using ProfHeat.DAL.Repositories;
using System;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Windows.Input;
using ProfHeat.AUI.ViewModels;

namespace ProfHeat.AUI.ViewModels;

public class OptimizerViewModel : BaseViewModel
{
    private IAssetManager _assetManager { get; }
    private ISourceDataManager _sourceDataManager { get; }
    private readonly HeatingGrid _grid;                                     // Static grid (Display grid image in the UI)
    private List<MarketCondition> _marketConditions = [];                   // Dynamic data

    public ObservableCollection<CheckBoxItem> CheckBoxItems { get; set; }   // Note: Add the pictures from PU and display in the UI (And make the pictures)
    public ObservableCollection<OptimizationResult> Results { get; set; } = [];

    public ICommand ImportDataCommand { get; }
    public ICommand OptimizeCommand { get; }
    public OptimizerViewModel()
    {
        _sourceDataManager = new SourceDataManager();
        _assetManager = new AssetManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HeatingGrid.config"));
        _grid = _assetManager.LoadAssets();
        CheckBoxItems = new ObservableCollection<CheckBoxItem>(_grid.ProductionUnits.Select(pu => new CheckBoxItem(pu.Name)));

        ImportDataCommand = ReactiveCommand.CreateFromTask(ImportDataAsync);
        OptimizeCommand = ReactiveCommand.Create(Optimize);
    }

    private async Task ImportDataAsync()
    {
        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [
                    new("XML Files")
                    {
                        Patterns = ["*.xml"],
                        AppleUniformTypeIdentifiers = ["public.xml"],
                        MimeTypes = ["application/xml", "text/xml"]
                    }]
        };

        // Trigger the interaction
        var filePaths = (await GetMainWindow().StorageProvider.OpenFilePickerAsync(options))
            .Select(file => file.TryGetLocalPath())
            .ToArray();

        if (filePaths == null || filePaths.Length == 0)
        {
            return;
        }

        // Load the source data
        _marketConditions = _sourceDataManager.LoadSourceData(filePaths[0]!);
    }

    private void Optimize()
    {
        // User selected units
        var selectedUnits = CheckBoxItems.Where(cbi => cbi.IsChecked).Select(cbi => cbi.Name).ToList();
        var newUnits = _grid.ProductionUnits.Where(pu => selectedUnits.Contains(pu.Name)).ToList();
        var newGrid = new HeatingGrid
        {
            Buildings = _grid.Buildings,
            ProductionUnits = newUnits
        };

        var optimizationResults = Optimizer.Optimize(newGrid, _marketConditions);

        // For testing, needs to be removed
        ResultsText = string.Join("\n", optimizationResults.Select(result =>
    $"Period: {result.TimeFrom} to {result.TimeTo}, " +
    $"Produced Heat: {result.ProducedHeat} MWh, " +
    $"Electricity Produced: {result.ElectricityProduced} MWh, " +
    $"Primary Energy Consumption: {result.PrimaryEnergyConsumption} MWh, " +
    $"Costs: {result.Costs} DKK, " +
    $"CO2 Emissions: {result.CO2Emissions} kg")); // Update this when you have new results

        if (optimizationResults == null || !optimizationResults.Any())
        {
            throw new InvalidOperationException("Optimization failed to produce any results.");
        }

        Results = new ObservableCollection<OptimizationResult>(optimizationResults);

    }

    private string _resultsText;
    public string ResultsText
    {
        get => _resultsText;
        set => this.RaiseAndSetIfChanged(ref _resultsText, value);
    }
}
