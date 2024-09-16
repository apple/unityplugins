using NUnit.Framework;

using Apple.Core.Runtime;
using System;

public class TestNSData
{
    [Test]
    public void TestBasicOperations()
    {
        NSData emptyData = NSString.Empty.Utf8Data;
        Assert.AreEqual(emptyData.Length, 0);

        NSString emptyString = new NSString(emptyData);
        Assert.AreEqual(string.Empty, emptyString.ToString());

        NSData testData = new NSString("test").Utf8Data;
        Assert.AreEqual(testData.Length, 4);
        
        NSString testString = new NSString(testData);
        Assert.AreEqual("test", testString.ToString());

        var testBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        NSData testData2 = new NSData(testBytes);
        Assert.AreEqual(testData2.Length, testBytes.Length);
        Assert.AreEqual(testBytes, testData2.Bytes);
    }
}
