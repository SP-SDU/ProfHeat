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
using ProfHeat.Core.Models;
using ProfHeat.DAL.Interfaces;
using ProfHeat.DAL.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace ProfHeat.AUI.ViewModels;

public partial class DataVisualizerViewModel : BaseViewModel
{
    private readonly IResultDataManager _ResultDataManager = new ResultDataManager(new CsvRepository());
    private readonly ObservableCollection<OptimizationResult> _results;
    private readonly FilePickerSaveOptions _saveCsvFileOptions = new()
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

    public DataVisualizerViewModel(ObservableCollection<OptimizationResult> results)
    {
        _results = results;
    }

    [RelayCommand]
    public async Task ExportResultsCommand()
    {
        var results = new List<OptimizationResult>(_results);
        var filePicker = await GetMainWindow().StorageProvider.SaveFilePickerAsync(_saveCsvFileOptions);
        var filePath = filePicker!.TryGetLocalPath();

        if (filePath != null)
        {
            _ResultDataManager.SaveResultData(results, filePath!);
        }
    }
}
