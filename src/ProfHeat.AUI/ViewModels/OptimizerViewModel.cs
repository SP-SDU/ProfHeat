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
using System.Reactive;
using Avalonia.Controls;
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

namespace ProfHeat.AUI.ViewModels;

public class OptimizerViewModel : BaseViewModel
{
    private IAssetManager _assetManager { get; }
    private ISourceDataManager _sourceDataManager { get; }
    private readonly HeatingGrid _grid;                                     // Static grid (Display grid image in the UI)
    private List<MarketCondition> _marketConditions = [];                   // Dynamic data

    public ObservableCollection<CheckBoxItem> CheckBoxItems { get; set; }   // Note: Add the pictures from PU and display in the UI (And make the pictures)
    public ObservableCollection<OptimizationResult> Results { get; set; } = [];

    public Interaction<Unit, string[]> ShowOpenFileDialog { get; }
    public ReactiveCommand<Unit, Unit> ImportDataCommand { get; }
    public ReactiveCommand<Unit, Unit> OptimizeCommand { get;  }
    public OptimizerViewModel()
    {
        _sourceDataManager = new SourceDataManager();
        _assetManager = new AssetManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/HeatingGrid.xml"));
        _grid = _assetManager.LoadAssets();
        CheckBoxItems = new ObservableCollection<CheckBoxItem>(_grid.ProductionUnits.Select(pu => new CheckBoxItem(pu.Name)));

        ShowOpenFileDialog = new Interaction<Unit, string[]>();

        ImportDataCommand = ReactiveCommand.CreateFromTask(ImportDataAsync);
        OptimizeCommand = ReactiveCommand.Create(Optimize);
    }

    private async Task ImportDataAsync()
    {
        // Trigger the interaction
        var filePath = await ShowOpenFileDialog.Handle(Unit.Default);

        // Load the source data
        _marketConditions = _sourceDataManager.LoadSourceData(filePath[0]);
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
        Results = new ObservableCollection<OptimizationResult>(optimizationResults);
        // Need to prove that the optimization is working by displaying the results
        // Turn them into a graph in DV (which is why it is observable collection)
        // And export the results to a file in DV (OpenFolderDialog)
    }
}
