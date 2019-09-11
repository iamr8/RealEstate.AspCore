using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
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
      //Add Custom assembly resolver
      AppDomain.CurrentDomain.AssemblyResolve += Resolver;

      //Any CefSharp references have to be in another method with NonInlining
      // attribute so the assembly rolver has time to do it's thing.
      InitializeCefSharp();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitializeCefSharp()
    {
      CefSharpSettings.ShutdownOnExit = true;
      CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

      var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        Environment.Is64BitProcess ? "x64" : "x86",
        "CefSharp.BrowserSubprocess.exe");
      var settings = new CefSettings
      {
        CachePath = "cache",
        UserAgent = "RealEstate.App",
        BackgroundColor = 0x00,
        LogFile = "Debug.log", //You can customise this path
        LogSeverity = LogSeverity.Default, // You can change the log level
        BrowserSubprocessPath = path
      };

      settings.CefCommandLineArgs.Add("enable-media-stream", "1");
      settings.CefCommandLineArgs.Add("disable-gpu", "1"); // Disable GPU acceleration
      settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1"); //Disable GPU vsync
      settings.DisableGpuAcceleration();

      Cef.EnableHighDPISupport();
      Cef.Initialize(settings, true, browserProcessHandler: null);
    }

    // Will attempt to load missing assembly from either x86 or x64 subdir
    // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
    private static Assembly Resolver(object sender, ResolveEventArgs args)
    {
      if (!args.Name.StartsWith("CefSharp"))
        return null;

      var assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
      var archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        Environment.Is64BitProcess ? "x64" : "x86",
        assemblyName);

      return File.Exists(archSpecificPath)
        ? Assembly.LoadFile(archSpecificPath)
        : null;
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