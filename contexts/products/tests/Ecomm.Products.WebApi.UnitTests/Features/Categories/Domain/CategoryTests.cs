using Ecomm.Products.WebApi.Features.Categories.Domain;

using Shouldly;

namespace Ecomm.Products.WebApi.UnitTests.Features.Categories.Domain;

[Trait("UnitTests", "Categories - Domain")]
public sealed class CategoryTests
{
    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNullOrEmpty()
    {
        // Arrange
        string? name = null;
        // Act & Assert
        Should.Throw<ArgumentException>(() => Category.Create(name!));
    }

    [Fact]
    public void Create_ShouldCreateCategory_WhenNameIsValid()
    {
        // Arrange
        var name = "Electronics";
        // Act
        var category = Category.Create(name);
        // Assert
        category.ShouldNotBeNull();
        category.Name.ShouldBe(name);
        category.ParentCategoryId.ShouldBeNull();
        category.Children.ShouldBeEmpty();
    }

    [Fact]
    public void Update_ShouldThrowArgumentException_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var category = Category.Create("Electronics");
        // Act & Assert
        Should.Throw<ArgumentException>(() => category.Update(null!));
    }

    [Fact]
    public void Update_ShouldUpdateCategory_WhenNameIsValid()
    {
        // Arrange
        var category = Category.Create("Electronics");
        var newName = "Updated Electronics";
        // Act
        category.Update(newName);
        // Assert
        category.Name.ShouldBe(newName);
        category.ParentCategoryId.ShouldBeNull();
        category.Children.ShouldBeEmpty();
    }

    [Fact]
    public void Update_ShouldUpdateParent_WhenParentIsProvided()
    {
        // Arrange
        var parentCategory = Category.Create("Parent Category");
        var category = Category.Create("Child Category", parentCategory);
        var newParentCategory = Category.Create("New Parent Category");
        // Act
        category.Update("Updated Child Category", newParentCategory);
        // Assert
        category.Name.ShouldBe("Updated Child Category");
        category.Parent.ShouldBe(newParentCategory);
        category.ParentCategoryId.ShouldBe(newParentCategory.Id);
    }

    [Fact]
    public void Update_ShouldNotChangeParent_WhenParentIsNull()
    {
        // Arrange
        var parentCategory = Category.Create("Parent Category");
        var category = Category.Create("Child Category", parentCategory);
        // Act
        category.Update("Updated Child Category", null);
        // Assert
        category.Name.ShouldBe("Updated Child Category");
        category.Parent.ShouldBe(parentCategory);
        category.ParentCategoryId.ShouldBe(parentCategory.Id);
    }

    [Fact]
    public void Create_ShouldAddChildToParent_WhenParentIsProvided()
    {
        // Arrange
        var parentCategory = Category.Create("Parent Category");
        // Act
        var childCategory = Category.Create("Child Category", parentCategory);
        // Assert
        parentCategory.Children.ShouldContain(childCategory);
        childCategory.Parent.ShouldBe(parentCategory);
        childCategory.ParentCategoryId.ShouldBe(parentCategory.Id);
    }
}
