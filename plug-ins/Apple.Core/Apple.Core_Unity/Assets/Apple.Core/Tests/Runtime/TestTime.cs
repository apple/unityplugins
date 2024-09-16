using NUnit.Framework;

using Apple.Core.Runtime;
using System;
using System.Threading.Tasks;

public class TestTime
{
    [Test]
    public async Task TestBasicOperations()
    {
        // Confirm that network and local time are within a 60 seconds of each other.
        var networkTime = await Time.GetNetworkTime();
        var localTime = DateTime.UtcNow;
        Assert.Less(Math.Abs((networkTime - localTime).TotalSeconds), 60.0);
    }
}