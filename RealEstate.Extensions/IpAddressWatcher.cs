using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RealEstate.Extensions
{
    public static class IpAddressWatcher
    {
        public static string GetIpAddress
        {
            get
            {
                var finalIp = string.Empty;
                var firstUpInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .OrderByDescending(c => c.Speed)
                    .FirstOrDefault(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up);

                if (firstUpInterface == null)
                    return finalIp;

                var props = firstUpInterface.GetIPProperties();
                var ip = props.UnicastAddresses
                    .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select(c => c.Address)
                    .FirstOrDefault();
                if (ip != null)
                    finalIp = ip.ToString();

                return finalIp;
            }
        }
    }
}