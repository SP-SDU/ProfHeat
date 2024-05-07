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
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using ProfHeat.Core.Repositories;
using SkiaSharp;

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

    // Observable properties. (UI)
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ExportResultsCommand))]
    private List<OptimizationResult> _results;

    private string _selectedPeriod = String.Empty;
    public string SelectedPeriod
    {
        get => _selectedPeriod;
        set
        {
            _selectedPeriod = value;
            OnPropertyChanged(nameof(Results));
            OnPropertyChanged(nameof(Costs));
            OnPropertyChanged(nameof(CO2Emissions));
            OnPropertyChanged(nameof(ProducedHeat));
            OnPropertyChanged(nameof(PrimaryEnergyConsumption));
            OnPropertyChanged(nameof(ElectricityProduced));
            OnPropertyChanged(nameof(XAxes));
        }
    }
    public ObservableCollection<string> Periods { get; } = ["Winter", "Summer"];
    public ObservableCollection<ISeries> Costs => GetLineSeries(r => r.Costs);
    public ObservableCollection<ISeries> CO2Emissions => GetLineSeries(r => r.CO2Emissions);
    public ObservableCollection<ISeries> ProducedHeat => GetLineSeries(r => r.ProducedHeat);
    public ObservableCollection<ISeries> PrimaryEnergyConsumption => GetLineSeries(r => r.PrimaryEnergyConsumption);
    public ObservableCollection<ISeries> ElectricityProduced => GetLineSeries(r => r.PrimaryEnergyConsumption);
    public static Axis[] XAxes => [new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("yy MMM dd',' HH'h'"))];
    #endregion

    #region Constructor
    public DataVisualizerViewModel(List<OptimizationResult> results)
    {
        SelectedPeriod = Periods[0];
        Results = results;
    }
    #endregion

    #region Commands
    /// <summary> Command to import results from a CSV file. </summary>
    [RelayCommand]
    public async Task ImportResults()
    {
        try
        {
            var filePicker = await App.TopLevel.StorageProvider.OpenFilePickerAsync(_openCsvFileOptions);    // Select file in File Explorer.
            var filePaths = filePicker
                .Select(file => file
                .TryGetLocalPath())
                .ToList();

            if (filePaths.Count != 0)
            {
                Results.Clear();
                Results.AddRange(
                    _ResultDataManager.LoadResultData(filePaths[0]!));

                OnPropertyChanged(nameof(Results));
                OnPropertyChanged(nameof(Costs));
                OnPropertyChanged(nameof(CO2Emissions));
                OnPropertyChanged(nameof(ProducedHeat));
                OnPropertyChanged(nameof(PrimaryEnergyConsumption));
                OnPropertyChanged(nameof(ElectricityProduced));
                OnPropertyChanged(nameof(XAxes));
                ExportResultsCommand.NotifyCanExecuteChanged();
            }
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception, "Error importing results");
        }
    }

    /// <summary> Command to export results to a CSV file. </summary>
    [RelayCommand(CanExecute = nameof(CanExport))]
    public async Task ExportResults()
    {
        try
        {
            var filePicker = await App.TopLevel.StorageProvider.SaveFilePickerAsync(_saveCsvFileOptions);

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
        catch (Exception exception)
        {
            await HandleErrorAsync(exception, "Error exporting results");
        }
    }
    #endregion

    #region Helper Methods
    private bool CanExport() => Results.Count > 0;

    private ObservableCollection<ISeries> GetLineSeries(Func<OptimizationResult, double> selector) => new(Results
        .GroupBy(r => r.UnitName)
        .Select(group => new LineSeries<DateTimePoint>
        {
            DataPadding = new LvcPoint(0.5f, 0),
            Values = group
            .Where(r => (SelectedPeriod == "Winter") ? r.TimeFrom.Month >= 10 || r.TimeFrom.Month <= 3 : r.TimeFrom.Month >= 4 && r.TimeFrom.Month <= 9)
            .Select(r => new DateTimePoint(r.TimeFrom, selector(r))),
            Name = group.Key,
            GeometryStroke = null,
            GeometryFill = null,
            Fill = null
        }));
    #endregion
}
