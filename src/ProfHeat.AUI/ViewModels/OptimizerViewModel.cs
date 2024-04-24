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
using ProfHeat.Core.Repositories;

namespace ProfHeat.AUI.ViewModels;

public partial class OptimizerViewModel : BaseViewModel
{
    #region Fields
    // Instances of managers.
    private readonly IAssetManager _assetManager = new AssetManager(
        new XmlRepository(),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HeatingGrid.config")   // Path to HeatingGrid.config.
        );
    private readonly ISourceDataManager _sourceDataManager = new SourceDataManager(new CsvRepository());
    private readonly IOptimizer _optimizer = new Optimizer();

    // Fields for holding data.
    private readonly HeatingGrid _grid;
    private readonly List<MarketCondition> _marketConditions = [];
    private readonly List<OptimizationResult> _results;

    // Options for opening CSV files.
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

    public ObservableCollection<CheckBoxItem> CheckBoxItems { get; }
    #endregion

    #region Constructor
    public OptimizerViewModel(List<OptimizationResult> results)
    {
        _results = results;

        // Load assets and initialize CheckBoxItems for UI.
        _grid = _assetManager.LoadAssets();
        CheckBoxItems = new(
            _grid.ProductionUnits
            .Select(unit =>
            new CheckBoxItem(
                unit.Name,
                unit.ImagePath,
                () => OptimizeCommand.NotifyCanExecuteChanged()
                ))
        );
    }
    #endregion

    #region Commands
    /// <summary> Command to import data from a CSV file. </summary>
    [RelayCommand]
    public async Task ImportData()
    {
        var filePicker = await GetMainWindow().StorageProvider
            .OpenFilePickerAsync(_openCsvFileOptions);    // Select file in File Explorer.
        var filePaths = filePicker
            .Select(file => file
            .TryGetLocalPath())
            .ToList();

        if (filePaths.Count != 0)
        {
            _marketConditions.Clear();
            _marketConditions
                .AddRange(_sourceDataManager
                .LoadSourceData(filePaths[0]!));   // Load the source data.

            OptimizeCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary> Command to perform optimization based on the selected production units and imported conditions. </summary>
    [RelayCommand(CanExecute = nameof(CanOptimize))]
    public void Optimize()
    {
        // Getting check marked Production Units
        var selectedUnits = CheckBoxItems
            .Where(item => item.IsChecked)
            .Select(item => item.Name)
            .ToList();
        var newUnits = _grid.ProductionUnits
            .Where(unit => selectedUnits
            .Contains(unit.Name))
            .ToList();
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
    #endregion

    #region Helper Methods
    private bool CanOptimize() => _marketConditions.Count > 0 && CheckBoxItems.Any(cbi => cbi.IsChecked);
    #endregion
}
