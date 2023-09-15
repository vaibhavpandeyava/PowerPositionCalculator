using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services;
using PowerPositionCalculator;
using App.WindowsService;

namespace PowerPositionCalculator.Tests;

[TestClass]
public class PowerPositionCalculatorTest
{
    [TestMethod]
    public void TestFetchAsync()
    {

        var mockDependency = new Mock<IPowerService>();
        mockDependency.Setup(x => x.GetTradesAsync(DateTime.Now)).ReturnsAsync(new List<PowerTrade>());
        var list = new List<PowerTrade>();
        var test = new CommonHelper();
        var result = test.AggregateTrade(list,5);
        Assert.IsNotNull(result);

    }
}