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

using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;

namespace ProfHeat.AUI.ViewModels;

public partial class OptimizerViewModel : BaseViewModel
{
    #region Fields
    // Instances of managers.
    private readonly ISourceDataManager _sourceDataManager;
    private readonly IOptimizer _optimizer;

    // Fields for holding data.
    private readonly HeatingGrid _grid;
    private readonly List<MarketCondition> _marketConditions = [];
    private readonly List<OptimizationResult> _results;
    private readonly Action<int> _changeTab;

    // Observable properties.
    public ObservableCollection<CheckBoxItem> CheckBoxItems { get; }
    public string GridName { get; }
    public string GridImagePath { get; }
    #endregion

    #region Constructor
    public OptimizerViewModel(IAssetManager assetManager, ISourceDataManager sourceDataManager, IOptimizer optimizer, List<OptimizationResult> results, Action<int> changeTab)
    {
        _sourceDataManager = sourceDataManager;
        _optimizer = optimizer;
        _results = results;
        _changeTab = changeTab;
        _grid = assetManager.LoadAssets();

        // initialize CheckBoxItems for UI.
        CheckBoxItems = new(
            _grid.ProductionUnits
            .Select(unit =>
            new CheckBoxItem(
                unit.Name,
                unit.ImagePath,
                () => OptimizeCommand.NotifyCanExecuteChanged()
                ))
        );
        GridName = _grid.Name;
        GridImagePath = _grid.ImagePath;
    }
    #endregion

    #region Commands
    /// <summary> Command to import data from a CSV file. </summary>
    [RelayCommand]
    public async Task ImportData(string filePath = null!)
    {
        try
        {
            filePath ??= await GetLoadFilePathAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                _marketConditions.Clear();
                _marketConditions.AddRange(_sourceDataManager.LoadSourceData(filePath));   // Load the source data.

                OptimizeCommand.NotifyCanExecuteChanged();
            }
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception, "Error importing data");
        }
    }

    /// <summary> Command to perform optimization based on the selected production units and imported conditions. </summary>
    [RelayCommand(CanExecute = nameof(CanOptimize))]
    public void Optimize()
    {
        try
        {
            // Getting check marked Production Units
            var selectedUnits = CheckBoxItems
                .Where(item => item.IsChecked)
                .Select(item => _grid.ProductionUnits.Find(unit => unit.Name == item.Name))
                .Where(unit => unit != default)
                .ToList();

            var optimizationResults = _optimizer.Optimize(selectedUnits, _marketConditions);

            if (optimizationResults.Count != 0)
            {
                _results.Clear();
                _results.AddRange(optimizationResults);
                _changeTab?.Invoke(1);  // Changes tab to Data Visualizer.
            }
        }
        catch (Exception exception)
        {
            _ = HandleErrorAsync(exception, "Error while optimizing");
        }
    }
    #endregion

    #region Helper Methods
    private bool CanOptimize() => _marketConditions.Count > 0 && CheckBoxItems.Any(checkBoxItem => checkBoxItem.IsChecked);
    #endregion
}
