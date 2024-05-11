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
using ProfHeat.Core.Interfaces;
using ProfHeat.Core.Models;
using ProfHeat.Core.Repositories;

namespace ProfHeat.Core.Tests.RepositoryTests;

public class ResultDataManagerTests
{
    private readonly Mock<IRepository> _mockRepository = new();
    private readonly IResultDataManager _resultDataManager;
    private readonly string _filePath = Path.Combine("Data", "testFile.csv");
    private readonly List<OptimizationResult> _OptimizationResults = [new(
        "Gas",
        new DateTime(2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
        new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified),
        10.11, 10.11, 102.23, 0, 11.23)];

    public ResultDataManagerTests() => _resultDataManager = new ResultDataManager(_mockRepository.Object);

    [Fact]
    public void LoadResultData_Test()
    {
        // Arrange
        _ = _mockRepository.Setup(repository => repository.Load<List<OptimizationResult>>(_filePath)).Returns(_OptimizationResults);

        // Act
        var result = _resultDataManager.LoadResultData(_filePath);

        // Assert
        Assert.Equal(_OptimizationResults, result);
        _mockRepository.Verify(repository => repository.Load<List<OptimizationResult>>(_filePath), Times.Once);
    }

    [Fact]
    public void SaveResultData_Test()
    {
        // Arrange
        _ = _mockRepository.Setup(repository => repository.Save(It.IsAny<List<OptimizationResult>>(), _filePath));

        // Act
        _resultDataManager.SaveResultData(_OptimizationResults, _filePath);

        // Assert
        _mockRepository.Verify(repository => repository.Save(_OptimizationResults, _filePath), Times.Once);
    }
}
