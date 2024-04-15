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

using ProfHeat.Core.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ProfHeat.DAL.Interfaces;
using ProfHeat.DAL.Repositories;
using Avalonia.Platform.Storage;

namespace ProfHeat.AUI.ViewModels;

public class DataVisualizerViewModel : BaseViewModel
{
    private readonly IResultDataManager _ResultDataManager;

    public ObservableCollection<OptimizationResult> Results { get; set; } = [];

    public ICommand ExportResultsCommand { get; }
    // Display data as graphs and charts
    // Maybe an Import results button

    public DataVisualizerViewModel()
    {
        _ResultDataManager = new ResultDataManager();

        ExportResultsCommand = ReactiveCommand.CreateFromTask(ExportResultsAsync);
    }

    private async Task ExportResultsAsync()
    {
        var options = new FilePickerSaveOptions
        {
            SuggestedFileName = "Results",
            DefaultExtension = "xml",
        };

        var filePath = (await GetMainWindow().StorageProvider.SaveFilePickerAsync(options))!.TryGetLocalPath();

        if (filePath == null)
        {
            return;
        }

        var results = new List<OptimizationResult>(Results);

        _ResultDataManager.SaveResultData(results, filePath!);
    }
}
