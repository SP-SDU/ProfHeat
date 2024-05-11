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

public class SourceDataManagerTests
{
    private readonly Mock<IRepository> _mockRepository = new();
    private readonly ISourceDataManager _sourceDataManager;
    private readonly string _filePath = Path.Combine("Data", "testFile.csv");
    private readonly List<MarketCondition> _marketConditions = [new(
        new DateTime(2023, 02, 08, 0, 0, 0, DateTimeKind.Unspecified),
        new DateTime(2023, 02, 08, 1, 0, 0, DateTimeKind.Unspecified),
        10.111, 10.111)];

    public SourceDataManagerTests() => _sourceDataManager = new SourceDataManager(_mockRepository.Object);

    [Fact]
    public void LoadSourceData_Test()
    {
        // Arrange
        _ = _mockRepository.Setup(repository => repository.Load<List<MarketCondition>>(_filePath)).Returns(_marketConditions);

        // Act
        var result = _sourceDataManager.LoadSourceData(_filePath);

        // Assert
        Assert.Equal(_marketConditions, result);
        _mockRepository.Verify(repository => repository.Load<List<MarketCondition>>(_filePath), Times.Once);
    }

    [Fact]
    public void SaveSourceData_Test()
    {
        // Arrange
        _ = _mockRepository.Setup(repository => repository.Save(It.IsAny<List<MarketCondition>>(), _filePath));

        // Act
        _sourceDataManager.SaveSourceData(_marketConditions, _filePath);

        // Assert
        _mockRepository.Verify(repository => repository.Save(_marketConditions, _filePath), Times.Once);
    }
}
