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

namespace ProfHeat.AUI.ViewModels
{
    public class OptimizerViewModel : BaseViewModel
    {
        private IAssetManager _assetManager { get; }
        private ISourceDataManager _sourceDataManager { get; }
        public ObservableCollection<CheckBoxItem> CheckBoxItems { get; set; }
        public ObservableCollection<MarketCondition> MarketConditions { get; set; }

        // Define an Interaction for opening file dialogs
        public Interaction<Unit, string[]> ShowOpenFileDialog { get; }

        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        public OptimizerViewModel()
        {
            _sourceDataManager = new SourceDataManager();
            _assetManager = new AssetManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/HeatingGrid.xml"));
            var units = _assetManager.LoadAssets().ProductionUnits;
            CheckBoxItems = new ObservableCollection<CheckBoxItem>(units.Select(pu => new CheckBoxItem(pu.Name)));

            ShowOpenFileDialog = new Interaction<Unit, string[]>();

            // Setup the ImportCommand to show the file dialog
            ImportCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // Trigger the interaction
                var filePath = await ShowOpenFileDialog.Handle(Unit.Default);

                // Load the source data
                MarketConditions = new ObservableCollection<MarketCondition>(_sourceDataManager.LoadSourceData(filePath[0]));
            });
        }
    }
}
