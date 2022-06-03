using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Apple.Core.Runtime
{
    public static class Network
    {
        /// <summary>
        /// Determines if the internet is reachable through:
        /// 1. Checking the Application.internetReachability
        /// 2. Fetching the NetworkTime via Time.GetNetworkTime()
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsInternetReachable()
        {
            try
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                    return false;
                
                await Time.GetNetworkTime();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
