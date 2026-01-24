using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Blazor.Dayjs.Tests;

[Collection("Collection")]
public sealed class DayJsNowTests : FixturedUnitTest
{
    public DayJsNowTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
