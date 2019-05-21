using Microsoft.Win32;
using RealEstate.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using WindowsFirewallHelper;
using WindowsFirewallHelper.FirewallAPIv2;

namespace RealEstate.Runner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _admin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private delegate void SetTextCallback(string text);

        private static Mode RunMode => (Mode)Enum.Parse(typeof(Mode), Config.RunMode, true);
        private readonly bool _admin;
        private static R8Config Config => Assembly.GetEntryAssembly().ReadConfiguration();
        private Process Process { get; set; }
        private string FullUrl { get; set; }
        private static ushort Port => ushort.Parse(Config.Port);
        private string Ip { get; set; }
        private static string MainAssembly => $"{Config.Prefix}.{Config.Startup}";

        private void StopApplication()
        {
            Program.KillDotNet();
            Log("Application stopped.");
            btnFunc.Text = "Start";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogToFile();
        }

        private enum Mode
        {
            Debug,
            Release,
        }

        private bool CheckDotNetCoreRuntimesInstalled()
        {
            var process = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C dotnet --version",
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
                Log("DotNet Sdk not installed.");
                return false;
            }

            Log($"DotNet Sdk {output.Replace("\r\n", "")} is installed.");
            return true;
        }

        private bool IsPortReady(string port)
        {
            // netstat -o
            // 192.168.1.34:5566
            var process = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "netstat",
                    Arguments = $"-na | find \"{port}\"",
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
                Log("Port is ready to use.");
                Stop(false);
                return true;
            }

            Log($"Port {Port} is in use. strongly recommended to reboot Machine");
            Stop(true);
            return false;
        }

        private bool DbInstanceRunner(string command, string validityCondition)
        {
            var instance = Config.Instance;
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

        private bool CheckLocalDbInstalled()
        {
            var localDb = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\13.0", false);
            var success = localDb?.ValueCount > 0;

            if (success)
            {
                // msiexec /i SqlLocalDB.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES
                Log("Microsoft SQL Server LocalDB 13.0 is installed");
                return true;
            }
            else
            {
                Log("Microsoft SQL Server LocalDB 13.0 not installed");
                return false;
            }
        }

        private bool CheckFirewall()
        {
            var outBound = CheckFirewallPort(MainAssembly, Port, FirewallDirection.Outbound);
            var inBound = CheckFirewallPort(MainAssembly, Port, FirewallDirection.Inbound);

            var success = outBound && inBound;
            if (success)
            {
                Log("Port is already opened.");
                return true;
            }

            Log("Unable to open needed port.");
            return false;
        }

        private static bool CheckFirewallPort(string appName, ushort port, FirewallDirection direction)
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

        public bool CheckIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = (from ip in host.AddressList
                             where ip.AddressFamily == AddressFamily.InterNetwork
                                    && ip.ToString().StartsWith("192")
                             select ip.ToString()).FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                Ip = ipAddress;
                FullUrl = $"http://{Ip}:{Port}";
                Log($"Server IP: {FullUrl}");
                return true;
            }

            Log("Unable to find server's ip.");
            return false;
        }

        private bool CreateSynchronizer()
        {
            if (string.IsNullOrEmpty(FullUrl))
                return false;

            var synchronizer = Properties.Resources.Synchronizer;
            synchronizer = synchronizer.Replace("{{url}}", FullUrl);
            var fileName = Path.ChangeExtension("Synchronizer", ".html");

            var dir = Directory.GetCurrentDirectory();
            var path = $"{dir}\\{fileName}";

            var bytes = Encoding.UTF8.GetBytes(synchronizer);
            using (var fs = File.Open(path, FileMode.OpenOrCreate))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }

            if (string.IsNullOrEmpty(path))
            {
                Log("Unable to Create Synchronizer.");
                Stop(true);
                return false;
            }

            Stop(false);
            Log($"Synchronizer created in : {path}");
            return true;
        }

        private static string GetAssemblyVersion(string filePath)
        {
            var assembly = Assembly.LoadFrom(filePath);
            var ver = assembly.GetName().Version;
            return ver.ToString();
        }

        private bool CheckAssembliesVersion()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var result = new Dictionary<string, bool>();

            var list = Config.Assemblies.Split('|').Select(x => $"{Config.Prefix}.{x}.dll").ToList();
            if (list.Count == 0)
                return false;

            foreach (var assembly in list)
            {
                var dllPath = $"{path}\\{assembly}";
                var fileAvailability = File.Exists(dllPath);

                if (fileAvailability)
                {
                    result.Add($"{assembly} {GetAssemblyVersion(dllPath)} found.", true);
                }
                else
                {
                    result.Add($"{assembly} not found.", false);
                }
            }

            var success = false;
            foreach (var ass in result)
            {
                if (ass.Value)
                {
                    Log(ass.Key);
                    success = true;
                }
                else
                {
                    Log(ass.Key);
                    success = false;
                }
            }
            Stop(!success);
            return success;
        }

        private void LogToFile()
        {
            var logs = txtLog.Text;
            if (string.IsNullOrEmpty(logs))
                return;

            var path = Path.GetDirectoryName($"{Assembly.GetEntryAssembly().Location}");

            var logFolderPath = $"{path}\\logs";
            var isFolderExists = Directory.Exists(logFolderPath);
            if (isFolderExists)
            {
                var fileName = $"[{DateTime.Now.Date:yy-MM-dd} {DateTime.Now.ToString("t").Replace(":", "-")}] {Guid.NewGuid()}.txt";
                var currentLogFile = $"{logFolderPath}\\{fileName}";

                using (var fs = new FileStream(currentLogFile, FileMode.CreateNew))
                {
                    var info = new UTF8Encoding(true).GetBytes(logs);
                    fs.Write(info, 0, info.Length);
                }
            }
            else
            {
                var createDir = Directory.CreateDirectory(logFolderPath);
                if (createDir.Exists)
                {
                    LogToFile();
                }
            }
        }

        private void Log(string text)
        {
            if (this.txtConsole.InvokeRequired)
            {
                var d = new SetTextCallback(Log);
                Invoke(d, text);
            }
            else
            {
                txtConsole.Text += $"[{DateTime.Now}] {text}\r\n";
            }
        }

        private void UpdateHosts(string[] hosts, string hostPath)
        {
            var newHosts = string.Join("\r\n", hosts);
            if (!string.IsNullOrEmpty(newHosts))
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(newHosts);
                    using (var fs = File.Open(hostPath, FileMode.OpenOrCreate))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                        Log($"Forwarding URL filled correctly to \" {Config.Domain}");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log("Need administration previllege to edit hosts file.");
                    RunAsAdministrator();
                    return;
                }
            }
        }

        private bool CheckDatabasePath()
        {
            if (!string.IsNullOrEmpty(Config.DbPath))
            {
                if (File.Exists(Config.DbPath))
                {
                    Log($"Database path: {Config.DbPath}");
                    return true;
                }
            }

            Log("Unable to find database file path.");
            return true;
        }

        private bool CheckHostsFile()
        {
            const string hostPath = "C:\\Windows\\System32\\drivers\\etc\\hosts";
            var hosts = File.ReadLines(hostPath).ToArray();
            if (hosts.Any())
            {
                var forwardingUrl = hosts.FirstOrDefault(x => x.Contains(Ip));
                var currentDefinition = $"{Ip} {Config.Domain}";
                if (!string.IsNullOrEmpty(forwardingUrl))
                {
                    var forwardingUrls = forwardingUrl.Split(' ');
                    if (!forwardingUrls[0].Equals(Config.Domain))
                    {
                        Log($"Forwarding URL is set to: \" {Config.Domain} \"");
                        hosts[Array.IndexOf(hosts, forwardingUrl)] = currentDefinition;
                        UpdateHosts(hosts, hostPath);
                    }
                    else
                    {
                        Log($"Forwarding URL is the same as current definition: \" {Config.Domain} \"");
                    }
                }
                else
                {
                    var tempHost = hosts.ToList();
                    tempHost.Add(currentDefinition);
                    hosts = tempHost.ToArray();
                    UpdateHosts(hosts, hostPath);
                    Log($"Forwarding URL added as: \" {Config.Domain} \"");
                }

                return true;
            }

            Log("Unable to find Hosts file.");
            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log(RunMode.ToString());
            Log(_admin ? "Running as Administrator" : "Not running as Administrator");

            var assemblies = CheckAssembliesVersion();
            if (!assemblies)
                return;

            var dbPath = CheckDatabasePath();
            if (!dbPath)
                return;

            var firewallActivation = CheckFirewall();
            if (!firewallActivation)
                return;

            var ipCheck = CheckIpAddress();
            if (!ipCheck)
                return;

            var localDbCheck = CheckLocalDbInstalled();
            if (!localDbCheck)
                return;

            var dotNetCheck = CheckDotNetCoreRuntimesInstalled();
            if (!dotNetCheck)
                return;

            //var checkHosts = CheckHostsFile();
            //if (!checkHosts)
            //    return;

            var checkInstance = CheckDbInstance();
            if (!checkInstance)
                return;
        }

        private bool CheckDbInstance()
        {
            var instanceFound = DbInstanceRunner("info", "Name:");
            if (instanceFound)
            {
                Log("Instance found.");
                return true;
            }

            Log("Instance not found or not valid.");

            var instanceDeleted = DbInstanceRunner("d", "LocalDB instance \"MSSQLLocalDB\" deleted.");
            Log(instanceDeleted ? "Tried to delete former instance." : "Unable to delete former instance");

            var instanceCreated = DbInstanceRunner("c", "LocalDB instance \"MSSQLLocalDB\" created with");
            if (instanceCreated)
            {
                Log("Instance created successfully.");
            }
            else
            {
                Log("Unable to start created instance");
                return false;
            }

            var instanceStarted = DbInstanceRunner("start", "LocalDB instance \"MSSQLLocalDB\" started.");
            if (instanceStarted)
            {
                Log("Instance started successfully.");
                return true;
            }

            Log("Unable to start instance");
            return false;
        }

        private void RunAsAdministrator()
        {
            var processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase)
            {
                UseShellExecute = true,
                Verb = "runas"
            };
            Process.Start(processStartInfo);
            Application.Exit();
        }

        private void StartApplication()
        {
            var portCheck = IsPortReady(Port.ToString());
            if (!portCheck)
                return;

            var synchronizerCheck = CreateSynchronizer();
            if (!synchronizerCheck)
                return;

            var start = Start();
            if (!start)
                return;

            btnFunc.Text = "Stop";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var text = btnFunc.Text;
            switch (text)
            {
                case "Start":
                    StartApplication();
                    break;

                case "Stop":
                default:
                    StopApplication();

                    break;
            }
        }

        private bool Start()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (!process.ProcessName.Contains("dotnet"))
                    continue;

                process.Kill();
                Log("DOTNET process closed.");
            }

            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = "dotnet",
                    Arguments = $"{MainAssembly}.dll",
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
            if (!startState)
            {
                Log("Unable to start app.");
                Stop(true);
                return false;
            }

            Log("Process is preparing to run.");
            Stop(false);
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
            return true;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Log($"Application exited.");
            Program.KillDotNet();
            Stop(true);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (RunMode == Mode.Debug)
            {
                Log(e.Data);
            }
            else
            {
                LogDotnet(e.Data);
                Log($"Application got error.");
            }
        }

        private void Stop(bool state)
        {
            SetText(state ? "Stop" : "Start");
        }

        private void SetText(string text)
        {
            if (this.btnFunc.InvokeRequired)
            {
                var d = new SetTextCallback(SetText);
                Invoke(d, text);
            }
            else
            {
                btnFunc.Text = text;
            }
        }

        private void LogDotnet(string text)
        {
            if (this.txtLog.InvokeRequired)
            {
                var d = new SetTextCallback(LogDotnet);
                Invoke(d, text);
            }
            else
            {
                txtLog.Text += $"[{DateTime.Now}] {text}\r\n";
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (RunMode == Mode.Debug)
            {
                Log(e.Data);
                if (e.Data?.Contains("Unable to start Kestrel.") == true)
                {
                    Log("Unable to start Application.");
                    Stop(true);
                    return;
                }

                if (e.Data?.Contains("Application started. Press Ctrl+C to shut down.") == true)
                {
                    Log("Application running successfully.");
                    if (string.IsNullOrEmpty(FullUrl))
                        return;

                    Process.Start(new ProcessStartInfo(FullUrl));
                }
            }
            else
            {
                LogDotnet(e.Data);
                if (e.Data?.Contains("Unable to start Kestrel.") == true)
                {
                    Log("Unable to start Background process.");
                }
                else if (e.Data?.Contains("http://*:8080 is already in use.") == true)
                {
                    Log("Failed to bind to address http://[::]:8080: address already in use.");
                }
                else if (e.Data?.Contains("Application started. Press Ctrl+C to shut down.") == true)
                {
                    Log("Application running successfully.");
                    if (string.IsNullOrEmpty(FullUrl))
                        return;

                    Process.Start(new ProcessStartInfo(FullUrl));
                }
            }
        }
    }
}