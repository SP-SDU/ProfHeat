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

using ProfHeat.Core.Models;

namespace ProfHeat.Core.Tests.ModelsTests;

public class CheckBoxItemTests
{
    private const string Name = "test";
    private const string ImagePath = "/test.svg";
    [Fact]
    public void IsCheckedChanged_TriggersAction()
    {
        // Arrange
        bool actionInvoked = false;
        var checkBoxItem = new CheckBoxItem(Name, ImagePath, () => actionInvoked = true);

        // Act
        checkBoxItem.IsChecked = !checkBoxItem.IsChecked;

        // Assert
        Assert.True(actionInvoked);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Act
        var checkBoxItem = new CheckBoxItem(Name, ImagePath, () => { });

        // Assert
        Assert.Equal(Name, checkBoxItem.Name);
        Assert.Equal(ImagePath, checkBoxItem.ImagePath);
        Assert.False(checkBoxItem.IsChecked);
    }
}

