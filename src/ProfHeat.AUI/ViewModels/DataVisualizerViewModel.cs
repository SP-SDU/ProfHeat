using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ProfHeat.AUI.Views;

namespace ProfHeat.AUI.ViewModels
{
    public partial class DataVisualizerViewModel : BaseViewModel
    {
        private readonly IResultDataManager _ResultDataManager;
        private readonly FilePickerSaveOptions _saveCsvFileOptions = new FilePickerSaveOptions
        {
            Title = "Save CSV File",
            SuggestedFileName = $"Results_{Path.GetRandomFileName()}",
            DefaultExtension = "csv",
            FileTypeChoices = new[]
            {
                new FileTypeChoice("CSV Files (Invariant Culture)", new[] { "*.csv" })
            }
        };

        private List<OptimizationResult> _results;

        public ISeries[] Series { get; private set; }

        public DataVisualizerViewModel(List<OptimizationResult> results, IResultDataManager resultDataManager)
        {
            _ResultDataManager = resultDataManager;
            _results = results;
            InitializeChartData();
        }

        private void InitializeChartData()
        {
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _results.Select(r => r.Costs).ToArray(),
                    Name = "Optimized Costs"
                }
            };
        }

        private bool CanExport() => _results != null && _results.Count > 0;

        [RelayCommand(CanExecute = nameof(CanExport))]
        public async Task ExportResults()
        {
            try
            {
                var filePicker = await GetMainWindow().StorageProvider.SaveFilePickerAsync(_saveCsvFileOptions);
                if (filePicker == null)
                {
                    return;
                }

                var filePath = filePicker.TryGetLocalPath();
                if (filePath != null)
                {
                    _ResultDataManager.SaveResultData(_results, filePath);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or notify the user
                // For example: Log.Error("Failed to export results: " + ex.Message);
                Console.WriteLine("Error exporting results: " + ex.Message);
            }
        }

        // Helper method to get the main window, assuming it's implemented elsewhere in your codebase
        private MainWindow GetMainWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow as MainWindow;
            }
            return null;
        }
    }
}
