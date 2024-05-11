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

public class DataVisualizerViewModelTests
{
    private readonly Mock<IResultDataManager> _mockResultDataManager = new();
    private readonly List<OptimizationResult> _results = [new(
            "Gas Boiler",
            new DateTime(2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
            new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified),
            5, 0, 5.05, 2500, 1075)];
    private const string _filePath = "testFile.csv";
    private readonly DataVisualizerViewModel _viewModel;

    public DataVisualizerViewModelTests() => _viewModel = new DataVisualizerViewModel(_mockResultDataManager.Object, _results);

    [Fact]
    public void Constructor_InitializesProperties()
    {
        Assert.Equal("Winter", _viewModel.SelectedPeriod);
        Assert.NotNull(_viewModel.Results);
        Assert.Equal(_results, _viewModel.Results);
    }

    [Fact]
    public void SelectedPeriod_UpdatesProperties()
    {
        // Arrange
        var initialPeriod = _viewModel.SelectedPeriod;

        // Act
        _viewModel.SelectedPeriod = "Summer";

        // Assert
        Assert.NotEqual(initialPeriod, _viewModel.SelectedPeriod);
        Assert.NotEmpty(_viewModel.Costs);
        Assert.NotEmpty(_viewModel.CO2Emissions);
    }

    [Fact]
    public async Task ImportResults_UpdatesResultsSuccessfully()
    {
        // Arrange
        _ = _mockResultDataManager.Setup(resultDataManager => resultDataManager.LoadResultData(_filePath)).Returns(_results);

        // Act
        await _viewModel.ImportResults(_filePath);

        // Assert
        Assert.Equal(_results.Count, _viewModel.Results.Count);
        _mockResultDataManager.Verify(resultDataManager => resultDataManager.LoadResultData(_filePath), Times.Once);
    }

    [Fact]
    public async Task ExportResults_SucceedsWhenResultsArePresent()
    {
        // Arrange
        _ = _mockResultDataManager.Setup(resultDataManager => resultDataManager.SaveResultData(_viewModel.Results, _filePath));

        // Act
        await _viewModel.ExportResults(_filePath);

        // Assert
        _mockResultDataManager.Verify(resultDataManager => resultDataManager.SaveResultData(_viewModel.Results, _filePath), Times.Once);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void CanExport_ReflectsResultsCount(int count, bool expectedCanExport)
    {
        // Arrange
        _viewModel.Results = [];
        for (int i = 0; i < count; i++)
        {
            _viewModel.Results.Add(new());
        }

        // Act
        bool canExport = _viewModel.ExportResultsCommand.CanExecute(null);

        // Assert
        Assert.Equal(expectedCanExport, canExport);
    }
}
