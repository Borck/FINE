using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using ReactiveUI.Reactive.Builder;

namespace FINETests;

[TestClass]
public class FINEAssemblySetup
{
    [AssemblyInitialize]
    public static void Initialize(TestContext testContext)
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .BuildApp();
    }
}
