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
using ReactiveUI;

namespace ProfHeat.AUI.Views;

using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ProfHeat.AUI.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

public partial class OptimizerView : ReactiveUserControl<OptimizerViewModel>
{
    [Obsolete]
    public OptimizerView()
    {
        InitializeComponent();
        ViewModel = new OptimizerViewModel();

        _ = this.WhenActivated(disposables => ViewModel?.ShowOpenFileDialog.RegisterHandler(async interaction =>
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Import Data File",
                    AllowMultiple = false,
                    Filters =
                    [
                        new() { Name = "XML Files", Extensions = { "xml" } }
                    ]
                };
                var result = await dialog.ShowAsync(GetWindow());
                interaction.SetOutput(result!);
            }).DisposeWith(disposables));
    }

    private Window GetWindow() => (Window) VisualRoot!;
}
