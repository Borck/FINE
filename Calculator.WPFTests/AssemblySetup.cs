using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using ReactiveUI.Reactive.Builder;

namespace CalculatorTests;

[TestClass]
public class AssemblySetup
{
    [AssemblyInitialize]
    public static void Initialize(TestContext testContext)
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .BuildApp();
    }
}
