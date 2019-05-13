﻿using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using WindowsFirewallHelper;
using WindowsFirewallHelper.FirewallAPIv2;

namespace RealEstate.Runner
{
    public static class Modules
    {
        public enum Mode
        {
            Debug,
            Release,
        }

        public static string CheckDotNetCoreRuntimesInstalled()
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
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
            {
                output += process.StandardOutput.ReadToEnd();
            }

            if (string.IsNullOrEmpty(output))
                return "DotNet Sdk not installed.";

            return $"DotNet Sdk {output.Replace("\r\n", "")} is installed.";
            // 2.2.105
        }

        public static bool IsPortReady(string port)
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

            if (string.IsNullOrEmpty(output))
                return true;

            return false;
        }

        public static bool DbInstance(string command, string validityCondition)
        {
            const string instance = "MSSQLLocalDB";
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

        public static bool CheckLocalDbInstalled()
        {
            var localDb = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\13.0", false);
            return localDb?.ValueCount > 0;
        }

        // Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\13.0
        public static bool CheckFirewall(string appName, ushort port)
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

        public static string CheckIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var result = (from ip in host.AddressList
                          where ip.AddressFamily == AddressFamily.InterNetwork
                                 && ip.ToString().StartsWith("192")
                          select ip.ToString()).FirstOrDefault();
            return result;
        }

        public static string CreateSynchronizer(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var synchronizer = Properties.Resources.Synchronizer;
            synchronizer = synchronizer.Replace("{{url}}", url);
            var fileName = Path.ChangeExtension("Synchronizer", ".html");

            var dir = Directory.GetCurrentDirectory();
            var path = $"{dir}\\{fileName}";

            //var exist = File.Exists(path);
            var bytes = Encoding.UTF8.GetBytes(synchronizer);
            using (var fs = File.Open(path, FileMode.OpenOrCreate))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
            //            }
            //            else
            //            {
            //                using (var fs = File.CreateText(path))
            //                {
            //                    fs.Write(synchronizer);
            //                    fs.Flush();
            //                }
            //            }

            return path;
        }

        private static string GetAssemblyVersion(string filePath)
        {
            var assembly = Assembly.LoadFrom(filePath);
            var ver = assembly.GetName().Version;
            return ver.ToString();
        }

        public static Dictionary<string, bool> CheckAssembliesVersion()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            const string prefix = "RealEstate";
            var @base = $"{prefix}.Base.dll";
            var resources = $"{prefix}.Resources.dll"; ;
            var services = $"{prefix}.Services.dll";
            var web = $"{prefix}.Web.dll";
            var configuration = $"{prefix}.Configuration.dll";

            var result = new Dictionary<string, bool>();
            var list = new List<string>
            {
                configuration,
                @base,
                resources,
                services,
                web
            };
            if (list.Count == 0)
                return new Dictionary<string, bool>();

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
            return result;
        }
    }
}