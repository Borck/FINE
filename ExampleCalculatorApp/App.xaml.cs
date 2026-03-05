namespace ExampleCalculatorApp;

using System.Windows;
using FINE;
using ReactiveUI;
using ReactiveUI.Builder;

public partial class App : Application {
  protected override void OnStartup(StartupEventArgs e) {
    base.OnStartup(e);

    RxAppBuilder.CreateReactiveUIBuilder()
        .WithWpf()
        .BuildApp();

    NNViewRegistrar.RegisterSplat();
  }
}
