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
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using ProfHeat.DAL.Interfaces;
using ProfHeat.DAL.Repositories;

namespace ProfHeat.AUI.ViewModels;

public partial class OptimizerViewModel : BaseViewModel
{
    private readonly IAssetManager _assetManager = new AssetManager(new XmlRepository(),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HeatingGrid.config")); // Sets the path to the HeatingGrid.config file
    private readonly ISourceDataManager _sourceDataManager = new SourceDataManager(new CsvRepository());
    private readonly IOptimizer _optimizer = new Optimizer();
    private readonly HeatingGrid _grid;                             // Static grid
    private readonly List<MarketCondition> _marketConditions = [];  // Dynamically loaded Market data
    private readonly List<OptimizationResult> _results;
    private readonly FilePickerOpenOptions _openCsvFileOptions = new()
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
    private bool CanOptimize() => _marketConditions.Count > 0 && CheckBoxItems.Any(cbi => cbi.IsChecked);

    public ObservableCollection<CheckBoxItem> CheckBoxItems { get; }

    public OptimizerViewModel(List<OptimizationResult> results)
    {
        // Initialize fields
        _results = results;
        _grid = _assetManager.LoadAssets();
        CheckBoxItems = new ObservableCollection<CheckBoxItem>(_assetManager.LoadAssets()
            .ProductionUnits.Select(pu => new CheckBoxItem(pu.Name, () => OptimizeCommand.NotifyCanExecuteChanged())));  // Populate CheckBoxItems with ProductionUnit names
    }

    [RelayCommand]
    public async Task ImportData()
    {
        var filePicker = await GetMainWindow().StorageProvider.OpenFilePickerAsync(_openCsvFileOptions);    // Select file in File Explorer
        var filePaths = filePicker.Select(file => file.TryGetLocalPath()).ToList();

        if (filePaths.Count != 0)
        {
            _marketConditions.Clear();
            _marketConditions.AddRange(_sourceDataManager.LoadSourceData(filePaths[0]!));   // Load the source data
            OptimizeCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanOptimize))]
    public void Optimize()
    {
        // Getting check marked Production Units
        var selectedUnits = CheckBoxItems.Where(cbi => cbi.IsChecked).Select(cbi => cbi.Name).ToList();
        var newUnits = _grid.ProductionUnits.Where(pu => selectedUnits.Contains(pu.Name)).ToList();
        var newGrid = new HeatingGrid(
            _grid.Name,
            _grid.ImagePath,
            _grid.Buildings,
            newUnits
            );

        var optimizationResults = _optimizer.Optimize(newGrid, _marketConditions);

        _results.Clear();
        _results.AddRange(optimizationResults);
    }
}
