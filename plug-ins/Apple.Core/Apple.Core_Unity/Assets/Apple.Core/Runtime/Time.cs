using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Apple.Core.Runtime
{
    public static class Time
    {
        private const string _ntpServer = "time.apple.com";
        private const byte _serverReplyTimeOffset = 40;
        private static UdpClient _udpClient;
        
        /// <summary>
        /// The timeout amount for doing Dns reverse lookup for the
        /// NTP network server.
        /// </summary>
        public static int DnsLookupTimeoutMS { get; set; } = 3000;

        /// <summary>
        /// The timeout amount for the request and response to the
        /// NTP network server.
        /// </summary>
        public static int NTPTimeoutMS { get; set; } = 3000;
        
        public static async Task<DateTime> GetNetworkTime()
        {
            var dnsLookupTask = Dns.GetHostEntryAsync(_ntpServer);
            
            if (await Task.WhenAny(dnsLookupTask, Task.Delay(DnsLookupTimeoutMS)) == dnsLookupTask)
            {
                var ipEndPoint = new IPEndPoint(dnsLookupTask.Result.AddressList[0], 123);
                return await GetNetworkTime(ipEndPoint);
            }
            else
            {
                throw new Exception("Time.GetNetworkTime() DNS lookup has timed out.");
            }
        }
        
        private static async Task<DateTime> GetNetworkTime(IPEndPoint endPoint)
        {
            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
            
            _udpClient = new UdpClient();
            _udpClient.Client.ReceiveTimeout = NTPTimeoutMS;
            _udpClient.Client.SendTimeout = NTPTimeoutMS;
            
            _udpClient.Connect(endPoint);
            await _udpClient.SendAsync(ntpData, ntpData.Length);
            var result = await _udpClient.ReceiveAsync();

            ulong intPart = BitConverter.ToUInt32(result.Buffer, _serverReplyTimeOffset);
            ulong fractPart = BitConverter.ToUInt32(result.Buffer, _serverReplyTimeOffset + 4);

            // Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(milliseconds);

            return networkDateTime;
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint) (((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }


    }
}
