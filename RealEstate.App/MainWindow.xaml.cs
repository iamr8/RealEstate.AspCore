using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shell;
using CefSharp;
using Microsoft.Win32;
using RealEstate.App.Controls;
using RealEstate.App.Library;
using RealEstate.App.Library.Blur;
using RealEstate.Configuration;
using WindowsFirewallHelper;
using WindowsFirewallHelper.FirewallAPIv2;

namespace RealEstate.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            _config = Assembly.GetEntryAssembly().ReadConfiguration();
            if (_config != null)
            {
                _mainAssembly = $"{_config.Prefix}.{_config.Startup}";
                _port = ushort.Parse(_config.Port);
                _runMode = (Mode)Enum.Parse(typeof(Mode), _config.RunMode, true);
            }
        }

        private delegate void SetTextCallback(string text);

        private enum Mode
        {
            Debug,
            Release,
        }

        private string LastPath { get; set; }
        private readonly Mode _runMode;

        private bool HasInitError
        {
            get => _hasInitError;
            set
            {
                ButtonStart.SetCurrentValue(IsEnabledProperty, !value);
                _hasInitError = value;
            }
        }

        private bool IsStarted
        {
            get => _isStarted;
            set
            {
                if (!HasInitError)
                {
                    Dispatcher?.Invoke(() =>
                    {
                        ButtonStart.SetCurrentValue(IsEnabledProperty, true);
                        TaskBarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Normal);
                        SetCurrentValue(SizeToContentProperty, SizeToContent.WidthAndHeight);
                        SetCurrentValue(WindowStateProperty, WindowState.Normal);
                    });

                    //                    var primary = System.Windows.Forms.Screen.PrimaryScreen;
                    //                    var workingArea = primary.WorkingArea;
                    var screenWidth = SystemParameters.PrimaryScreenWidth;
                    var screenHeight = SystemParameters.PrimaryScreenHeight;

                    if (value)
                    {
                        ButtonStart.Dispatcher?.Invoke(() =>
                        {
                            ButtonStart.SetCurrentValue(IsEnabledProperty, false);
                            ButtonStart.SetCurrentValue(ContentProperty, "در حال اتصال");
                        });
                        Dispatcher?.Invoke(() =>
                        {
                            Browser.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                            InitCheckList.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                            ButtonStart.SetCurrentValue(ContentProperty, "قطع اتصال");

                            ContentBackdrop.SetCurrentValue(OpacityProperty, 0.9);
                            BrowserWrapper.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                            ContentWrapper.SetCurrentValue(VisibilityProperty, Visibility.Hidden);

                            ButtonMaximized.SetCurrentValue(IsEnabledProperty, true);

                            SetCurrentValue(ResizeModeProperty, ResizeMode.CanResizeWithGrip);
                            //                                SetCurrentValue(LeftProperty, (double)10);
                            //                                SetCurrentValue(TopProperty, (double)10);
                            //SetCurrentValue(WidthProperty, screenWidth - Left * 2);
                            //SetCurrentValue(HeightProperty, screenHeight - Top * 2);

                            NavButtons.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                            ButtonNavBack.SetCurrentValue(IsEnabledProperty, false);
                            ButtonNavForward.SetCurrentValue(IsEnabledProperty, false);
                            ButtonNavRefresh.SetCurrentValue(IsEnabledProperty, false);
                        });

                        Browser.Dispatcher.Invoke(() =>
                        {
                            if (Browser.IsBrowserInitialized && !Browser.IsDisposed)
                                Browser.SetCurrentValue(CefSharp.Wpf.ChromiumWebBrowser.AddressProperty, FullUrl);
                        });
                    }
                    else
                    {
                        Dispatcher?.Invoke(() =>
                        {
                            ButtonStart.SetCurrentValue(ContentProperty, "اتصال");
                            Browser.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                            InitCheckList.SetCurrentValue(VisibilityProperty, Visibility.Visible);

                            ContentBackdrop.SetCurrentValue(OpacityProperty, 0.7);
                            BrowserWrapper.SetCurrentValue(VisibilityProperty, Visibility.Hidden);
                            ContentWrapper.SetCurrentValue(VisibilityProperty, Visibility.Visible);

                            ButtonMaximized.SetCurrentValue(IsEnabledProperty, false);
                            NavButtons.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);

                            SetCurrentValue(ResizeModeProperty, ResizeMode.NoResize);
                            SetCurrentValue(WidthProperty, (double)800);
                            SetCurrentValue(HeightProperty, (double)430);
                            SetCurrentValue(LeftProperty, (screenWidth / 2) - (Width / 2));
                            SetCurrentValue(TopProperty, (screenHeight / 2) - (Height / 2));
                        });
                    }
                }
                else
                {
                    ButtonStart.SetCurrentValue(ContentProperty, "بررسی مجدد");
                }

                _isStarted = value;
            }
        }

        private readonly R8Config _config;
        private Process Process { get; set; }
        private string FullUrl { get; set; }
        private readonly ushort _port;
        private string Ip { get; set; }
        private readonly string _mainAssembly;
        private bool _isStarted;
        private bool _hasInitError;

        private void CheckIpAddress()
        {
            if (_port == 0)
                return;

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = (from ip in host.AddressList
                             where ip.AddressFamily == AddressFamily.InterNetwork
                                   && ip.ToString().StartsWith("192")
                             select ip.ToString()).FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                Ip = ipAddress;
                FullUrl = $"http://{Ip}:{_port}";
                ServerUrl.SetCurrentValue(System.Windows.Controls.TextBox.TextProperty, FullUrl);
                CheckIp.SetCurrentValue(CheckItem.ValueProperty, FullUrl);
                CheckIp.SetCurrentValue(CheckItem.SuccessProperty, true);
                return;
            }
            CheckIp.SetCurrentValue(CheckItem.SuccessProperty, false);
        }

        private void CheckAssembliesVersion()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (string.IsNullOrEmpty(path))
            {
                DllFiles.SetCurrentValue(CheckItem.SuccessProperty, false);
                return;
            }

            var list = _config?.Assemblies?.Split('|')?.Select(x => $"{_config.Prefix}.{x}.dll").ToList();
            if (list == null || !list.Any())
            {
                DllFiles.SetCurrentValue(CheckItem.SuccessProperty, false);
                return;
            }

            var successDll = list.Where(x => File.Exists($"{path}\\{x}")).ToList();
            DllFiles.SetCurrentValue(CheckItem.SuccessProperty, successDll.Count == list.Count);
            DllFiles.SetCurrentValue(CheckItem.ValueProperty, string.Join(", ", successDll.Select(x => x.Split('.')[1])));

            try
            {
                var mainDll = successDll.FirstOrDefault(x => x.Equals("RealEstate.Web.dll"));
                var mainDllPath = Path.Combine(path, mainDll ?? throw new InvalidOperationException());
                var assembly = Assembly.LoadFrom(mainDllPath);
                var version = assembly.GetName().Version;
                VersionName.SetCurrentValue(ContentProperty, $"نگارش: {version}");
            }
            catch
            {
                DllFiles.SetCurrentValue(CheckItem.SuccessProperty, false);
            }
        }

        private void CheckLocalDbInstalled()
        {
            var localDb = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\13.0", false);
            var success = localDb?.ValueCount > 0;

            if (success)
            {
                // msiexec /i SqlLocalDB.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES
                CheckSqlLocalDb.SetCurrentValue(CheckItem.ValueProperty, "Microsoft SQL Server LocalDB 13.0");
                CheckSqlLocalDb.SetCurrentValue(CheckItem.SuccessProperty, true);
                return;
            }

            CheckSqlLocalDb.SetCurrentValue(CheckItem.SuccessProperty, false);
        }

        private void CheckDatabasePath()
        {
            DbPath.SetCurrentValue(CheckItem.ValueProperty, _config?.DbPath);
            DbPath.SetCurrentValue(CheckItem.SuccessProperty, !string.IsNullOrEmpty(_config?.DbPath) && File.Exists(_config.DbPath));
        }

        private bool CheckFirewallPort(string appName, ushort port, FirewallDirection direction)
        {
            try
            {
                var firewall = Firewall.Instance.Rules;
                if (firewall.Count == 0) return false;

                var outboundConnection = firewall.FirstOrDefault(x => x.Direction == direction && x.Name.Equals(appName));
                if (outboundConnection == null)
                {
                    var addPort = Firewall.Instance.CreatePortRule(FirewallProfiles.Private, appName, port);
                    addPort.Protocol = FirewallProtocol.TCP;
                    addPort.Direction = direction;
                    Firewall.Instance.Rules.Add(addPort);
                    return true;
                }

                if (outboundConnection.LocalPorts?.Any() == true && outboundConnection.LocalPorts[0] == port)
                    return true;

                var updatePort = Firewall.Instance.CreatePortRule(FirewallProfiles.Private, appName, port);
                updatePort.Protocol = FirewallProtocol.TCP;
                updatePort.Direction = direction;
                Firewall.Instance.Rules.Add(updatePort);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckFirewall()
        {
            var outBound = CheckFirewallPort(_mainAssembly, _port, FirewallDirection.Outbound);
            var inBound = CheckFirewallPort(_mainAssembly, _port, FirewallDirection.Inbound);

            FirewallApproval.SetCurrentValue(CheckItem.SuccessProperty, outBound && inBound);
        }

        private void CheckDotNetCoreRuntimesInstalled()
        {
            var process = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C dotnet --list-sdks",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    Verb = "runas"
                }
            };
            process.Start();
            var output = "";
            while (!process.HasExited)
                output += process.StandardOutput.ReadToEnd();

            if (string.IsNullOrEmpty(output))
            {
                CheckNetCoreRuntime.SetCurrentValue(CheckItem.SuccessProperty, false);
                return;
            }

            var versions = new List<string>();
            foreach (var line in output.Split(new[] { "\r\n" }, StringSplitOptions.None))
            {
                try
                {
                    var version = line.Split(new[]
                    {
                        " ["
                    }, StringSplitOptions.None)[0];
                    if (version.StartsWith("2.2"))
                        versions.Add(version);
                }
                catch
                {
                    // ignored
                }
            }

            CheckNetCoreRuntime.SetCurrentValue(CheckItem.SuccessProperty, true);
            CheckNetCoreRuntime.SetCurrentValue(CheckItem.ValueProperty, $"Microsoft .Net Core {versions.LastOrDefault()}");
        }

        private void IsPortReady()
        {
            if (_port == 0)
                return;

            // netstat -o
            // 192.168.1.34:5566
            var process = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "netstat",
                    Arguments = $"-na | find \"{_port}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    Verb = "runas"
                }
            };
            process.Start();
            var output = "";
            while (!process.HasExited)
            {
                output += process.StandardOutput.ReadToEnd();
            }

            var success = string.IsNullOrEmpty(output);
            if (success)
            {
                PortCheck.SetCurrentValue(CheckItem.SuccessProperty, true);
                PortCheck.SetCurrentValue(CheckItem.ValueProperty, _port.ToString());
                return;
            }

            PortCheck.SetCurrentValue(CheckItem.SuccessProperty, false);
            PortCheck.SetCurrentValue(CheckItem.ValueProperty, "پورت 8080 در حال استفاده است");
        }

        private bool DbInstanceRunner(string command, string validityCondition)
        {
            try
            {
                var instance = _config.Instance;
                var process = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo =
                    {
                        FileName = "sqllocaldb",
                        Arguments = $"{command} {instance}",
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };
                var isStarted = process.Start();
                if (!isStarted)
                    return false;

                var output = process.StandardOutput.ReadToEnd();
                return output.Contains(validityCondition);
            }
            catch
            {
                return false;
            }
        }

        private void CheckDbInstance2()
        {
            const string instanceName = "MSSQLLocalDB";

            var instanceFound = DbInstanceRunner("info", "Name:");
            if (instanceFound)
            {
                CheckDbInstance.SetCurrentValue(CheckItem.SuccessProperty, true);
                CheckDbInstance.SetCurrentValue(CheckItem.ValueProperty, instanceName);
                return;
            }

            DbInstanceRunner("d", "LocalDB instance \"MSSQLLocalDB\" deleted.");
            var instanceCreated = DbInstanceRunner("c", "LocalDB instance \"MSSQLLocalDB\" created with");
            if (instanceCreated)
            {
                CheckDbInstance.SetCurrentValue(CheckItem.ValueProperty, instanceName);
                CheckDbInstance.SetCurrentValue(CheckItem.SuccessProperty, true);
            }
            else
            {
                CheckDbInstance.SetCurrentValue(CheckItem.SuccessProperty, false);
                return;
            }

            var instanceStarted = DbInstanceRunner("start", "LocalDB instance \"MSSQLLocalDB\" started.");
            CheckDbInstance.SetCurrentValue(CheckItem.SuccessProperty, instanceStarted);
            CheckDbInstance.SetCurrentValue(CheckItem.ValueProperty, instanceStarted ? instanceName : "");
        }

        private void Initialize()
        {
            CheckAssembliesVersion();
            CheckDatabasePath();
            CheckFirewall();
            CheckIpAddress();
            CheckLocalDbInstalled();
            CheckDotNetCoreRuntimesInstalled();
            CheckDbInstance2();
            IsPortReady();

            var error = 0;
            var controls = InitCheckList.Children;
            foreach (UIElement control in controls)
            {
                if (!(control is CheckItem check))
                    continue;

                if (!check.Success)
                    error++;
            }

            HasInitError = error > 0;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;
            this.EnableBlur();

            IsStarted = false;
            HasInitError = true;

            Browser.LifeSpanHandler = new LifeSpanHandler();
            ButtonStart.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void LabelTitle_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            IsStarted = false;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                MessageBox.Show(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Contains("Unable to start Kestrel.") == true)
            {
                IsStarted = false;
                MessageBox.Show(_runMode == Mode.Debug ? e.Data : "خطا در اجرای برنامه اصلی");
            }
            else if (e.Data?.Contains("http://*:8080 is already in use.") == true)
            {
                IsStarted = false;
                MessageBox.Show(_runMode == Mode.Debug ? e.Data : "پورت 8080 مشغول است.");
            }
            else if (e.Data?.Contains("Application started. Press Ctrl+C to shut down.") == true)
            {
                IsStarted = !string.IsNullOrEmpty(FullUrl);
            }
        }

        private void ButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            TaskBarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Indeterminate);
            ButtonStart.SetCurrentValue(IsEnabledProperty, false);
            if (HasInitError)
            {
                Initialize();
                TaskBarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Normal);
                ButtonStart.SetCurrentValue(IsEnabledProperty, true);
                return;
            }

            App.KillProcesses();
            if (!IsStarted)
            {
                Process = new Process
                {
                    EnableRaisingEvents = true,
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = $"{_mainAssembly}.dll",
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };

                Process.OutputDataReceived += Process_OutputDataReceived;
                Process.ErrorDataReceived += Process_ErrorDataReceived;
                Process.Exited += Process_Exited;

                var startState = Process.Start();
                if (startState)
                {
                    Process.BeginErrorReadLine();
                    Process.BeginOutputReadLine();
                }
            }
            else
            {
                IsStarted = false;
            }
            ButtonStart.SetCurrentValue(IsEnabledProperty, true);
            TaskBarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Normal);
        }

        private void Browser_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            const int adjust = 6;
            if (WindowState == WindowState.Maximized)
            {
                TitleWrapper.SetCurrentValue(PaddingProperty, new Thickness(30, 10, 30, 10));
                ContentBackdrop.SetCurrentValue(System.Windows.Controls.Border.PaddingProperty, new Thickness(adjust, 0, adjust, adjust));
            }
            else
            {
                TitleWrapper.SetCurrentValue(PaddingProperty, new Thickness(10));
                ContentBackdrop.SetCurrentValue(System.Windows.Controls.Border.PaddingProperty, new Thickness(0));
            }
        }

        private void ExitBtn_OnClick(object sender, RoutedEventArgs e)
        {
            App.KillProcesses();
            Environment.Exit(0);
        }

        private void ButtonMinimized_OnClick(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(WindowStateProperty, WindowState.Minimized);
        }

        private void ButtonMaximized_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsStarted)
                SetCurrentValue(WindowStateProperty, WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        }

        private void ButtonStart_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ButtonNavBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsStarted && Browser.CanGoBack)
                Browser.Back();
        }

        private void ButtonNavForward_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsStarted && Browser.CanGoForward)
                Browser.Forward();
        }

        private void ButtonNavRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsStarted)
                Browser.Reload(false);
        }

        private void Browser_OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Dispatcher?.Invoke(() =>
            {
                Bar.SetCurrentValue(System.Windows.Controls.ProgressBar.IsIndeterminateProperty, e.IsLoading);
                ButtonNavBack.SetCurrentValue(IsEnabledProperty, e.CanGoBack);
                ButtonNavForward.SetCurrentValue(IsEnabledProperty, e.CanGoForward);
                ButtonNavRefresh.SetCurrentValue(IsEnabledProperty, e.CanReload);
            });
        }

        private void ButtonDisconnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsStarted)
                return;

            App.KillProcesses();
            IsStarted = false;
        }

        private void ButtonDevTools_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.ShowDevTools();
        }

        private void ButtonElmah_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LastPath))
            {
                LastPath = Browser.Address;
                Browser.SetCurrentValue(CefSharp.Wpf.ChromiumWebBrowser.AddressProperty, $"{FullUrl}/elmah");
            }
            else
            {
                LastPath = string.Empty;
                Browser.SetCurrentValue(CefSharp.Wpf.ChromiumWebBrowser.AddressProperty, LastPath);
            }
        }
    }
}