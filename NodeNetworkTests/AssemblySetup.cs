using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using ReactiveUI.Builder;

namespace NodeNetworkTests;

[TestClass]
public class NodeNetworkAssemblySetup
{
    [AssemblyInitialize]
    public static void Initialize(TestContext testContext)
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .BuildApp();
    }
}
