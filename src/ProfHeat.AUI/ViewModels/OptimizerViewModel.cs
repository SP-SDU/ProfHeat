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

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using ProfHeat.DAL.Interfaces;
using ProfHeat.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProfHeat.AUI.ViewModels;

public partial class OptimizerViewModel : BaseViewModel
{
    private readonly IAssetManager _assetManager = new AssetManager(new XmlRepository(),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HeatingGrid.config")); // Sets the path to the HeatingGrid.config file
    private readonly ISourceDataManager _sourceDataManager = new SourceDataManager(new CsvRepository());
    private readonly IOptimizer _optimizer = new Optimizer();
    private readonly ObservableCollection<OptimizationResult> _results;
    private readonly HeatingGrid _grid;                             // Static grid
    private readonly List<MarketCondition> _marketConditions = [];  // Dynamically loaded Market data

    public ObservableCollection<CheckBoxItem> CheckBoxItems { get; }

    public OptimizerViewModel(ObservableCollection<OptimizationResult> results)
    {
        // Initialize fields
        _results = results;
        _grid = _assetManager.LoadAssets();
        CheckBoxItems = new ObservableCollection<CheckBoxItem>(_assetManager.LoadAssets()
            .ProductionUnits.Select(pu => new CheckBoxItem(pu.Name)));  // Populate CheckBoxItems with ProductionUnit names
    }

    [RelayCommand]
    public async Task ImportDataCommand()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Open CSV File",
            AllowMultiple = false,
            FileTypeFilter = [
                new("CSV Files (Invariant Culture)")
                {
                    Patterns = ["*.csv"],
                    AppleUniformTypeIdentifiers = ["public.comma-separated-values-text"],
                    MimeTypes = ["text/csv"]
                }]
        };

        // Trigger file picker interaction
        var filePaths = (await GetMainWindow().StorageProvider.OpenFilePickerAsync(options))
            .Select(file => file.TryGetLocalPath())
            .ToArray();

        if (filePaths.Length == 0)
        {
            return;
        }

        // Load the source data
        _marketConditions.Clear();
        _marketConditions.AddRange(_sourceDataManager.LoadSourceData(filePaths[0]!));
    }

    [RelayCommand]
    public void OptimizeCommand()
    {
        // User selected units
        var selectedUnits = CheckBoxItems.Where(cbi => cbi.IsChecked).Select(cbi => cbi.Name).ToList();
        var newUnits = _grid.ProductionUnits.Where(pu => selectedUnits.Contains(pu.Name)).ToList();
        var newGrid = new HeatingGrid(
            Name: _grid.Name,
            ImagePath: _grid.ImagePath,
            Buildings: _grid.Buildings,
            ProductionUnits: newUnits
            );


        var optimizationResults = _optimizer.Optimize(newGrid, _marketConditions);

        if (optimizationResults.Count == 0)
        {
            throw new InvalidOperationException("Optimization failed to produce any results.");
        }

        _results.Clear();
        optimizationResults.ForEach(_results.Add);
    }
}
