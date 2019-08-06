using System;
using System.Diagnostics;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using RealEstate.App.Library;

namespace RealEstate.App
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      CefSharpSettings.ShutdownOnExit = true;
      CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

      var settings = new CefSettings
      {
        CachePath = "cache",
        UserAgent = "RealEstate.App",
        BackgroundColor = 0x00,
        LogFile = "Debug.log", //You can customise this path
        LogSeverity = LogSeverity.Default, // You can change the log level
      };

      settings.CefCommandLineArgs.Add("enable-media-stream", "1");
      settings.CefCommandLineArgs.Add("disable-gpu", "1"); // Disable GPU acceleration
      settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1"); //Disable GPU vsync
      settings.DisableGpuAcceleration();

      Cef.EnableHighDPISupport();
      Cef.Initialize(settings, true, browserProcessHandler: null);
    }

    public static void KillProcesses()
    {
      try
      {
        foreach (var process in Process.GetProcesses())
        {
          if (!process.ProcessName.Contains("dotnet"))
            continue;

          process.Kill();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      new ProcManager().KillByPort(8080);
    }

    protected override void OnExit(ExitEventArgs e)
    {
      KillProcesses();
//      Cef.Shutdown();
      base.OnExit(e);
    }
  }
}