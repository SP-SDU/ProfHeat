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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Windows.Input;

namespace ProfHeat.AUI.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    public OptimizerViewModel Optimizer { get; } = new OptimizerViewModel();

    public DataVisualizerViewModel DataVisualizer { get; } = new DataVisualizerViewModel();

    public ICommand MinimizeCommand { get; }
    public ICommand MaximizeRestoreCommand { get; }
    public ICommand CloseCommand { get; }

    public MainWindowViewModel()
    {
        MinimizeCommand = ReactiveCommand.Create(MinimizeWindow);
        MaximizeRestoreCommand = ReactiveCommand.Create(MaximizeRestoreWindow);
        CloseCommand = ReactiveCommand.Create(CloseWindow);

        // Assign the Results from Optimizer to DataVisualizer
        DataVisualizer.Results = Optimizer.Results;
    }

    private void MinimizeWindow() => GetMainWindow().WindowState = WindowState.Minimized;

    private void MaximizeRestoreWindow() => GetMainWindow().WindowState = GetMainWindow().WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void CloseWindow() => GetMainWindow().Close();
}
