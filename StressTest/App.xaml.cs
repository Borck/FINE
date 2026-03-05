namespace StressTest;

using System.Windows;
using FINE;
using ReactiveUI;
using ReactiveUI.Builder;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
  protected override void OnStartup(StartupEventArgs e) {
    base.OnStartup(e);

    RxAppBuilder.CreateReactiveUIBuilder()
        .WithWpf()
        .BuildApp();

    NNViewRegistrar.RegisterSplat();
  }
}
