using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        }

        private bool CheckFirewall(string appName, ushort port)
        {
            var firewall = Firewall.Instance.Rules;
            if (firewall.Count == 0) return false;

            var outboundConnections = firewall.Where(x => x.Direction == FirewallDirection.Outbound).ToList();
            if (outboundConnections.Count == 0) return false;

            var rule = outboundConnections.Find(x => x.Name.Equals(appName));
            if (rule != null)
                return true;

            var newPort = Firewall.Instance.CreatePortRule(FirewallProfiles.Private, appName, port);
            newPort.Protocol = FirewallProtocol.TCP;
            newPort.Direction = FirewallDirection.Outbound;

            Firewall.Instance.Rules.Add(newPort);
            return CheckFirewall(appName, port);
        }

        private void AddInfo(string text)
        {
            textBox1.Text += $"[{DateTime.Now}] {text}\r\n";
        }

        private static bool FindMainProcess(string appName)
        {
            var dir = Directory.GetCurrentDirectory();
            var path = Path.Combine(dir, $"{appName}.dll");
            var check = File.Exists(path);
            return check;
        }

        private static string CheckIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().StartsWith("192"))
                    return ip.ToString();

            return string.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const string appName = "RealEstate.Web";
            const ushort port = 5566;
            var url = string.Empty;

            var checkFile = FindMainProcess(appName);
            if (!checkFile)
            {
                AddInfo("Main dll file not found.");
                return;
            }
            else
            {
                AddInfo("Main dll found.");
            }

            var firewallActivation = CheckFirewall(appName, port);
            if (!firewallActivation)
            {
                AddInfo("Unable to open needed port.");
                return;
            }
            else
            {
                AddInfo("Firewall is already activated.");
            }

            var ipCheck = CheckIp();
            if (!string.IsNullOrEmpty(ipCheck))
            {
                url = $"http://{ipCheck}:{port}";
                AddInfo("Server IP: " + url);
            }
            else
            {
                AddInfo("Unable to find server's ip.");
            }

            var runApp = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"{appName}.dll",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };
            var proc = Process.Start(runApp);
            if (proc == null)
            {
                AddInfo("Unable to start app.");
                return;
            }
            else
            {
                AddInfo("Process running successfully.");
            }

            var sInfo = new ProcessStartInfo(url);
            Process.Start(sInfo);
            //var output = proc.StandardOutput.ReadToEnd();
            //AddInfo(output);
        }
    }
}