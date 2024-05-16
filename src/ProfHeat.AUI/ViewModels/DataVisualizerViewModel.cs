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

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using SkiaSharp;

namespace ProfHeat.AUI.ViewModels;

public partial class DataVisualizerViewModel : BaseViewModel
{
    #region Fields
    // Instances of managers.
    private readonly IResultDataManager _resultDataManager;

    // Observable properties.
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ExportResultsCommand))]
    private List<OptimizationResult> _results;

    [ObservableProperty]
    private string _selectedPeriod = String.Empty;

    public ObservableCollection<string> Periods { get; } = ["Winter", "Summer"];
    public ObservableCollection<ISeries> Costs => GetLineSeries(result => result.Costs);
    public ObservableCollection<ISeries> CO2Emissions => GetLineSeries(result => result.CO2Emissions);
    public ObservableCollection<ISeries> ProducedHeat => GetLineSeries(result => result.ProducedHeat);
    public ObservableCollection<ISeries> GasConsumption => GetLineSeries(result => result.GasConsumption);
    public ObservableCollection<ISeries> ElectricityProduced => GetLineSeries(result => result.ElectricityProduced);

    public static DrawMarginFrame DrawMarginFrame => new() { Stroke = new SolidColorPaint(SKColors.White, 1) };

    public static Axis[] XAxes =>
        [new DateTimeAxis(TimeSpan.FromHours(1), date => date.ToString("yy MMM dd',' HH'h'"))
        { LabelsPaint = new SolidColorPaint(SKColors.White) }];

    public static Axis[] CostsYAxis { get; } = GetYAxis("DKK");
    public static Axis[] CO2EmissionsYAxis { get; } = GetYAxis("kg");
    public static Axis[] ProducedHeatYAxis { get; } = GetYAxis("MWh");
    public static Axis[] GasConsumptionYAxis { get; } = GetYAxis("MWh");
    public static Axis[] ElectricityProducedYAxis { get; } = GetYAxis("MW");

    public SolidColorPaint LegendTextPaint { get; } = new() { Color = SKColors.White };

    #endregion

    #region Constructor
    public DataVisualizerViewModel(IResultDataManager resultDataManager, List<OptimizationResult> results)
    {
        _resultDataManager = resultDataManager;
        SelectedPeriod = Periods[0];
        Results = results;
    }
    #endregion

    #region Commands
    /// <summary> Command to import results from a CSV file. </summary>
    [RelayCommand]
    public async Task ImportResults(string filePath = null!)
    {
        try
        {
            filePath ??= await GetLoadFilePathAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                Results.Clear();
                Results.AddRange(_resultDataManager.LoadResultData(filePath));

                NotifyPropertiesChanged();
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
    public async Task ExportResults(string filePath = null!)
    {
        try
        {
            filePath ??= await GetSaveFilePathAsync();

            if (!string.IsNullOrEmpty(filePath))
            {
                _resultDataManager.SaveResultData(Results, filePath!);
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

    private ObservableCollection<ISeries> GetLineSeries(Func<OptimizationResult, double> selector) =>
        new(Results
            .GroupBy(r => r.UnitName)
            .Select(group => new LineSeries<DateTimePoint>
            {
                Values = group
                .Where(result => IsInSelectedPeriod(result.TimeFrom.Month))
                .Select(result => new DateTimePoint(result.TimeFrom, selector(result))),
                Name = group.Key,
                DataPadding = new LvcPoint(0.5f, 0),
                LineSmoothness = 0,
                GeometryStroke = null,
                GeometryFill = null,
                Fill = null
            }));

    private bool IsInSelectedPeriod(int month) =>
        SelectedPeriod == "Winter" ? month is >= 10 or <= 3 : month is >= 4 and <= 9;

    private static Axis[] GetYAxis(string metric) =>
        [new Axis
        {
            Name = metric,
            NamePaint = new SolidColorPaint(SKColors.White),
            LabelsPaint = new SolidColorPaint(SKColors.White)
        }];

    partial void OnSelectedPeriodChanged(string value) => NotifyPropertiesChanged();

    private void NotifyPropertiesChanged()
    {
        OnPropertyChanged(nameof(Results));
        OnPropertyChanged(nameof(Costs));
        OnPropertyChanged(nameof(CO2Emissions));
        OnPropertyChanged(nameof(ProducedHeat));
        OnPropertyChanged(nameof(GasConsumption));
        OnPropertyChanged(nameof(ElectricityProduced));
        OnPropertyChanged(nameof(XAxes));
    }
    #endregion
}
