using Soenneker.Blazor.Dayjs.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Blazor.Dayjs.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class TomSelectInteropTests : HostedUnitTest
{
    private readonly IDayJsInterop _util;

    public TomSelectInteropTests(Host host) : base(host)
    {
        _util = Resolve<IDayJsInterop>(true);
    }

    [Test]
    public void Default()
    {

    }
}
