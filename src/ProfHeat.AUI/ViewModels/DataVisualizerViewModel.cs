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
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfHeat.DAL.Interfaces;
using ProfHeat.DAL.Repositories;
using Avalonia.Platform.Storage;
using System.IO;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace ProfHeat.AUI.ViewModels;

public partial class DataVisualizerViewModel : BaseViewModel
{
    private readonly IResultDataManager _ResultDataManager = new ResultDataManager(new CsvRepository());
    private readonly ObservableCollection<OptimizationResult> _results;

    public DataVisualizerViewModel(ObservableCollection<OptimizationResult> results)
    {
        _results = results;
    }

    [RelayCommand]
    public async Task ExportResultsCommand()
    {
        var results = new List<OptimizationResult>(_results);
        var options = new FilePickerSaveOptions
        {
            Title = "Save CSV File",
            SuggestedFileName = $"Results_{Path.GetRandomFileName()}",
            DefaultExtension = "csv",
            FileTypeChoices = [
                new("CSV Files (Invariant Culture)")
                {
                    Patterns = ["*.csv"],
                    AppleUniformTypeIdentifiers = ["public.comma-separated-values-text"],
                    MimeTypes = ["text/csv"]
                }]
        };

        var files = await GetMainWindow().StorageProvider.SaveFilePickerAsync(options);

        if (files == null)
        {
            return;
        }

        var filePath = files.TryGetLocalPath();

        if (filePath == null)
        {
            return;
        }

        _ResultDataManager.SaveResultData(results, filePath!);
    }
}
