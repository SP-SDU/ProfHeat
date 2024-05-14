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

using Moq;
using ProfHeat.AUI.ViewModels;
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;

namespace ProfHeat.AUI.Tests.ViewModels;

public class OptimizerViewModelTests
{
    private readonly Mock<IAssetManager> _mockAssetManager = new();
    private readonly Mock<ISourceDataManager> _mockSourceDataManager = new();
    private readonly Mock<IOptimizer> _mockOptimizer = new();
    private readonly Mock<Action<int>> _mockChangeTab = new();
    private readonly List<MarketCondition> _marketConditions = [new(
        new DateTime(2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
        new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified),
        10.111, 10.111)];
    private readonly HeatingGrid _grid = new ("test", "/test.svg", 1000, [new ("Gas Boiler", "/Assets/Images/GasBoiler.svg", 5, 500, 215, 1.01, 0)]);
    private readonly List<OptimizationResult> _results = [];
    private readonly OptimizerViewModel _viewModel;
    private const string _filePath = "testFile.csv";

    public OptimizerViewModelTests()
    {
        _ = _mockAssetManager.Setup(x => x.LoadAssets()).Returns(_grid);
        _viewModel = new OptimizerViewModel(_mockAssetManager.Object, _mockSourceDataManager.Object, _mockOptimizer.Object, _results, _mockChangeTab.Object);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        Assert.NotNull(_viewModel.CheckBoxItems);
        Assert.Equal(_grid.Name, _viewModel.GridName);
        Assert.Equal(_grid.ImagePath, _viewModel.GridImagePath);
        Assert.Equal(_grid.ProductionUnits.Count, _viewModel.CheckBoxItems.Count);
    }

    [Fact]
    public async Task ImportData_LoadsData()
    {
        // Arrange
        _ = _mockSourceDataManager.Setup(sourceDataManager => sourceDataManager.LoadSourceData(_filePath)).Returns(_marketConditions);
        _viewModel.CheckBoxItems[0].IsChecked = true;

        // Act
        await _viewModel.ImportData(_filePath);

        // Assert
        Assert.True(_viewModel.OptimizeCommand.CanExecute(null));
    }

    [Fact]
    public void OptimizeCommand_Executes()
    {
        // Arrange
        _viewModel.CheckBoxItems[0].IsChecked = true;
        var selectedUnits = new List<ProductionUnit> { _grid.ProductionUnits[0] };
        var optimizationResults = new List<OptimizationResult> { new(
            "Gas Boiler",
            new DateTime(2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified),
            5, 0, 5.05, 2500, 1075)
        };
        _ = _mockOptimizer.Setup(sourceDataManager => sourceDataManager.Optimize(It.IsAny<List<ProductionUnit>>(), It.IsAny<List<MarketCondition>>())).Returns(optimizationResults);
        _ = _viewModel.ImportData(_filePath);

        // Act
        _viewModel.OptimizeCommand.Execute(null);

        // Assert
        _mockOptimizer.Verify(sourceDataManager => sourceDataManager.Optimize(selectedUnits, It.IsAny<List<MarketCondition>>()), Times.Once());
        Assert.NotEmpty(_results);
        _mockChangeTab.Verify(changeTab => changeTab.Invoke(1), Times.Once());
    }
}
