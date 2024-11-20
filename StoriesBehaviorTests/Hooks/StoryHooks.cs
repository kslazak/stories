using System.Diagnostics;

namespace StoriesBehaviorTests.Hooks;

[Binding]
public sealed class StoryHooks
{
    private static Process? _hostProcess;

    [BeforeTestRun]
    public static void RunApi()
    {
        _hostProcess = Process.Start(@"..\..\..\..\StoriesAPI\bin\Debug\net8.0\StoriesAPI.exe", "-UseMock");
    }

    [AfterTestRun]
    public static void StopApi()
    {
        _hostProcess?.Kill();
        _hostProcess?.Dispose();
    }
}
