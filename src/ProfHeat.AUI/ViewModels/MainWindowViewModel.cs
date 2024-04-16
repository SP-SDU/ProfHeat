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

using Avalonia.Controls;
using ProfHeat.Core.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace ProfHeat.AUI.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    public OptimizerViewModel Optimizer { get; }
    public DataVisualizerViewModel DataVisualizer { get; }

    public ObservableCollection<OptimizationResult> Results { get; set; } = [];

    public MainWindowViewModel()
    {
        Optimizer = new OptimizerViewModel(Results);
        DataVisualizer = new DataVisualizerViewModel(Results);
    }

    [RelayCommand]
    public void MinimizeCommand() => GetMainWindow().WindowState = WindowState.Minimized;

    [RelayCommand]
    public void MaximizeCommand() => GetMainWindow().WindowState = GetMainWindow().WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    [RelayCommand]
    public void CloseCommand() => GetMainWindow().Close();
}
