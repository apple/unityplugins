using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Apple.Core.Runtime
{
    public static class Time
    {
        private const string _ntpServer = "time.apple.com";
        private const int _ntpPort = 123;
        private const byte _serverReplyTimeOffset = 40;
        
        /// <summary>
        /// The timeout amount for the request and response to the
        /// NTP network server.
        /// </summary>
        public static int NTPTimeoutMS { get; set; } = 3000;
        
        public static async Task<DateTime> GetNetworkTime()
        {
            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
            
            using var udpClient = new UdpClient();

            udpClient.Connect(_ntpServer, _ntpPort);

            UdpReceiveResult result = default;

            var delayTask = Task.Delay(NTPTimeoutMS);

            if (await Task.WhenAny(
                Task.Run(async () =>
                {
                    await udpClient.SendAsync(ntpData, ntpData.Length);
                    result = await udpClient.ReceiveAsync();
                }),
                delayTask) == delayTask)
            {
                throw new Exception("Time.GetNetworkTime() has timed out.");
            }

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
