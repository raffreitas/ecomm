using Shouldly;

namespace Ecomm.Products.WebApi.UnitTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Assert.True(true);
        true.ShouldBeTrue();
    }
}
