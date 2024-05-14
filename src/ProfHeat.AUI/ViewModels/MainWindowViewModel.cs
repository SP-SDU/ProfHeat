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
using ProfHeat.Core.Repositories;

namespace ProfHeat.AUI.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    #region Fields
    // Instances of managers.
    private readonly IAssetManager _assetManager = new AssetManager(
        new XmlRepository(),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HeatingGrid.config")   // Path to HeatingGrid.config.
        );
    private readonly ISourceDataManager _sourceDataManager = new SourceDataManager(new CsvRepository());
    private readonly IResultDataManager _ResultDataManager = new ResultDataManager(new CsvRepository());
    private readonly IOptimizer _optimizer = new Optimizer();

    // List of optimization results.
    private readonly List<OptimizationResult> _results = [];

    // Observable properties.
    [ObservableProperty]
    private int _selectedTabIndex = 0;  // Sets Optimizer tab as default.

    // ViewModels for the tabs.
    public OptimizerViewModel Optimizer { get; }
    public DataVisualizerViewModel DataVisualizer { get; }
    #endregion

    #region Constructor
    public MainWindowViewModel()
    {
        Optimizer = new OptimizerViewModel(_assetManager, _sourceDataManager, _optimizer, _results, ChangeTab);
        DataVisualizer = new DataVisualizerViewModel(_ResultDataManager, _results);
    }
    #endregion

    #region Helper Methods
    private void ChangeTab(int tabIndex) => SelectedTabIndex = tabIndex;
    #endregion
}
