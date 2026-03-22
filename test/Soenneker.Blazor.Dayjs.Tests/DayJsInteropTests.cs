using Soenneker.Blazor.Dayjs.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Blazor.Dayjs.Tests;

[Collection("Collection")]
public class TomSelectInteropTests : FixturedUnitTest
{
    private readonly IDayJsInterop _util;

    public TomSelectInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IDayJsInterop>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
