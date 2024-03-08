using NUnit.Framework;

using Apple.Core.Runtime;
using System;

public class TestNSString
{
    [Test]
    public void TestBasicOperations()
    {
        NSString empty = NSString.Empty;
        Assert.AreEqual(empty.ToString(), string.Empty);
        Assert.AreEqual((string)empty, string.Empty);
        
        NSString test = new NSString("test");
        Assert.AreEqual(test.ToString(), "test");
        Assert.AreEqual((string)test, "test");
    }
}