using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using ReactiveUI.Builder;

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
