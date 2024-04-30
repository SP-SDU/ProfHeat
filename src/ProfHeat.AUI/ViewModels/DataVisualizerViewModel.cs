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

#region Using
using Avalonia.Platform.Storage;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using ProfHeat.Core.Repositories;
#endregion

namespace ProfHeat.AUI.ViewModels;

public partial class DataVisualizerViewModel : BaseViewModel
{
    #region Fields
    // Instances of managers.
    private readonly IResultDataManager _ResultDataManager = new ResultDataManager(new CsvRepository());

    // Options for CSV files.
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

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ExportResultsCommand))]
    private List<OptimizationResult> _results;
    #endregion

    #region Constructor
    public DataVisualizerViewModel(List<OptimizationResult> results)
    {
        Results = results;
    }
    #endregion

    #region Commands
    /// <summary> Command to import results from a CSV file. </summary>
    [RelayCommand]
    public async Task ImportResults()
    {
        var filePicker = await GetMainWindow().StorageProvider
            .OpenFilePickerAsync(_openCsvFileOptions);    // Select file in File Explorer.
        var filePaths = filePicker
            .Select(file => file
            .TryGetLocalPath())
            .ToList();

        if (filePaths.Count != 0)
        {
            Results.Clear();
            Results
                .AddRange(_ResultDataManager
                .LoadResultData(filePaths[0]!));

            ExportResultsCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary> Command to export results to a CSV file. </summary>
    [RelayCommand(CanExecute = nameof(CanExport))]
    public async Task ExportResults()
    {
        var filePicker = await GetMainWindow().StorageProvider
            .SaveFilePickerAsync(_saveCsvFileOptions);

        if (filePicker == null)
        {
            return;
        }

        var filePath = filePicker!.TryGetLocalPath();

        if (filePath != null)
        {
            _ResultDataManager.SaveResultData(Results, filePath!);
        }
    }
    #endregion

    #region Helper Methods
    private bool CanExport() => Results.Count > 0;
    #endregion
}
