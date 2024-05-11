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

public class AssetManagerTests
{
    private readonly Mock<IRepository> _mockRepository = new();
    private readonly IAssetManager _assetManager;
    private readonly string _filePath = Path.Combine("Data", "testFile.config");
    private readonly HeatingGrid _grid = new("test", "/test.svg", 1000, [new("Boiler", "/test2.svg", 5.00, 500, 215, 1.01, 0)]);

    public AssetManagerTests() => _assetManager = new AssetManager(_mockRepository.Object, _filePath);

    [Fact]
    public void LoadAssets_Test()
    {
        // Arrange
        _ = _mockRepository.Setup(repository => repository.Load<HeatingGrid>(_filePath)).Returns(_grid);

        // Act
        var result = _assetManager.LoadAssets();

        // Assert
        Assert.Equal(_grid.Name, result.Name);
        _mockRepository.Verify(repository => repository.Load<HeatingGrid>(_filePath), Times.Once);
    }

    [Fact]
    public void SaveAssets_Test()
    {
        // Arrange
        _ = _mockRepository.Setup(repository => repository.Save(It.IsAny<HeatingGrid>(), _filePath));

        // Act
        _assetManager.SaveAssets(_grid);

        // Assert
        _mockRepository.Verify(repository => repository.Save(_grid, _filePath), Times.Once);
    }
}
