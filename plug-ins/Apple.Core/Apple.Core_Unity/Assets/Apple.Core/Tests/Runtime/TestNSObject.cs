using NUnit.Framework;

using Apple.Core.Runtime;
using System;

public class TestNSObject
{
    [Test]
    public void TestBasicOperations()
    {
        NSString empty = NSString.Empty;
        NSString string1 = "string1";
        NSString string2 = "string2";
        NSString anotherString1 = "string1";

        Assert.IsTrue(string1.Equals(string1));
        Assert.IsFalse(string1.Equals(string2));
        Assert.IsTrue(string1.Equals(anotherString1));
        Assert.IsFalse(string1.Equals(null));
    }
}
