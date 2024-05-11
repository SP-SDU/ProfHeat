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
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ProfHeat.AUI.ViewModels;

public class BaseViewModel : ObservableObject
{
    #region Fields
    // Options for CSV files.
    private static readonly FilePickerOpenOptions s_openCsvFileOptions = new()
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
    private static readonly FilePickerSaveOptions s_saveCsvFileOptions = new()
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
    #endregion

    #region Methods
    protected static async Task HandleErrorAsync(Exception exception, string errorMessage = "An error occurred")
    {
        // Log the error
        Console.WriteLine($"{errorMessage}: {exception.Message}");

        // Show the user an error message
        _ = await MessageBoxManager
            .GetMessageBoxStandard("Error", $"{errorMessage}: {exception.Message}", ButtonEnum.Ok)
            .ShowAsync();
    }

    protected static async Task<string> GetLoadFilePathAsync() =>
        (await App.TopLevel.StorageProvider.OpenFilePickerAsync(s_openCsvFileOptions))
        .Select(file => file.TryGetLocalPath())
        .FirstOrDefault()!;

    protected static async Task<string> GetSaveFilePathAsync() =>
        (await App.TopLevel.StorageProvider.SaveFilePickerAsync(s_saveCsvFileOptions))
        ?.TryGetLocalPath()!;
    #endregion
}
