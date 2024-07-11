using NUnit.Framework;

using Apple.Core.Runtime;
using System;
using System.Collections.Generic;

public class TestNSDictionary
{
    [Test]
    // Tests: Add (via initializer list), operator[] (get/set), Count, GetEnumerator, ContainsKey, Contains, Remove, Clear
    public void TestBasicOperations()
    {
        const string zero = "zero";
        const string one = "one";
        const string two = "two";
        const string three = "three";
        const string onePlusTwo = "one+two";

        var mutableDictionary = new NSMutableDictionary<int, string> { { 0, zero }, { 1, one }, { 2, two } };

        Assert.AreEqual(3, mutableDictionary.Count);

        Assert.AreEqual(mutableDictionary[0], zero);
        Assert.AreEqual(mutableDictionary[1], one);
        Assert.AreEqual(mutableDictionary[2], two);

        mutableDictionary.Add(3, three);

        Assert.AreEqual(4, mutableDictionary.Count);
        Assert.AreEqual(mutableDictionary[3], three);

        NSDictionary<int, string> immutableDictionary = mutableDictionary;

        Assert.AreEqual(mutableDictionary.Count, immutableDictionary.Count);

        mutableDictionary[3] = onePlusTwo;

        Assert.AreEqual(mutableDictionary[3], onePlusTwo);
        Assert.AreEqual(immutableDictionary[3], onePlusTwo);

        Assert.AreEqual(4, mutableDictionary.Count);
        Assert.AreEqual(4, immutableDictionary.Count);

        Assert.IsTrue(mutableDictionary.ContainsKey(0));
        Assert.IsTrue(mutableDictionary.ContainsKey(1));
        Assert.IsTrue(mutableDictionary.ContainsKey(2));
        Assert.IsTrue(mutableDictionary.ContainsKey(3));

        Assert.IsTrue(immutableDictionary.ContainsKey(0));
        Assert.IsTrue(immutableDictionary.ContainsKey(1));
        Assert.IsTrue(immutableDictionary.ContainsKey(2));
        Assert.IsTrue(immutableDictionary.ContainsKey(3));

        Assert.IsFalse(mutableDictionary.ContainsKey(-1));
        Assert.IsFalse(mutableDictionary.ContainsKey(4));

        Assert.IsFalse(immutableDictionary.ContainsKey(-1));
        Assert.IsFalse(immutableDictionary.ContainsKey(4));

        int count = 0;
        foreach (var kvp in immutableDictionary)
        {
            Assert.IsTrue(mutableDictionary.Contains(kvp));
            count++;
        }
        Assert.AreEqual(count, mutableDictionary.Count);
        Assert.AreEqual(count, immutableDictionary.Count);

        Assert.IsTrue(mutableDictionary.Remove(0));
        Assert.IsFalse(mutableDictionary.ContainsKey(0));
        Assert.IsFalse(immutableDictionary.ContainsKey(0));
        Assert.AreEqual(count - 1, mutableDictionary.Count);
        Assert.AreEqual(count - 1, immutableDictionary.Count);

        Assert.IsTrue(mutableDictionary.Remove(new KeyValuePair<int, string>(2, two)));
        Assert.IsFalse(mutableDictionary.ContainsKey(2));
        Assert.IsFalse(immutableDictionary.ContainsKey(2));
        Assert.AreEqual(count - 2, mutableDictionary.Count);
        Assert.AreEqual(count - 2, immutableDictionary.Count);

        mutableDictionary.Clear();
        Assert.AreEqual(0, mutableDictionary.Count);
        Assert.AreEqual(0, immutableDictionary.Count);

        // Test attempt to remove item that does not exist. Should return false.
        Assert.IsFalse(mutableDictionary.Remove(0));
    }

    [Test]
    public void TestInvalidKeyType()
    {
        // attempt to create a dictionary with an unsupported key type
        Assert.Throws<NotSupportedException>(() => new NSMutableDictionary<NSObject, int>());
    }

    [Test]
    public void TestBackCompat()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var mutableDictionary = new NSMutableDictionary<string, NSObject>()
        {
            { "NSString", new NSString("string value") },
            { "bool", new NSNumber(true) },
            { "Int64", new NSNumber(64L) },
            { "Double", new NSNumber(3.14159) },
            { "NSDictionary", new NSMutableDictionary<string, NSObject>() }
        };

        var dictionary = mutableDictionary.As<NSDictionary>();

        Assert.IsTrue(dictionary != null);

        Assert.AreEqual(dictionary.GetString("NSString"), "string value");
        Assert.AreEqual(dictionary.GetBoolean("bool"), true);
        Assert.AreEqual(dictionary.GetInt64("Int64"), 64L);
        Assert.AreEqual(dictionary.GetDouble("Double"), 3.14159);
        Assert.IsTrue(dictionary.GetNSDictionary("NSDictionary") != null);

        // failure cases
        Assert.AreEqual(dictionary.GetString("nope"), default(string));
        Assert.AreEqual(dictionary.GetBoolean("nope"), default(bool));
        Assert.AreEqual(dictionary.GetInt64("nope"), default(Int64));
        Assert.AreEqual(dictionary.GetDouble("nope"), default(Double));
        Assert.AreEqual(dictionary.GetNSDictionary("nope"), default(NSDictionary));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Test]
    public void TestNullValues()
    {
        // NSDictionary uses NSNull to represent null values.
        // On the C# side, NSNull is converted to/from null.
        // Check this logic.

        var stringDictionary = new NSMutableDictionary<int, string>() { { 0, "NotNull" }, { 1, null }, { 2, "AlsoNotNull" } };
        Assert.AreEqual(stringDictionary[0], "NotNull");
        Assert.AreEqual(stringDictionary[1], null);
        Assert.AreEqual(stringDictionary[2], "AlsoNotNull");

        var objDictionary = new NSMutableDictionary<int, NSObject>() { { 0, new NSString("NotNull") }, { 1, null }, { 2, new NSString("AlsoNotNull") } };
        Assert.AreEqual(objDictionary[0]?.As<NSString>()?.ToString(), "NotNull");
        Assert.AreEqual(objDictionary[1]?.As<NSString>()?.ToString(), null);
        Assert.AreEqual(objDictionary[2]?.As<NSString>()?.ToString(), "AlsoNotNull");

        // If we explicitly add NSNull to the array, it will get unboxed as a C# null value.
        objDictionary.Add(3, new NSNull());
        Assert.AreEqual(objDictionary[3], null);
    }

    [Test]
    public void TestDuplicateKeys()
    {
        // Duplicate keys are not allowed by NSDictionary.
        // Values added later replace values added previously when the keys are the same.
        var dupDictionary = new NSMutableDictionary<int, int>() { { 0, 0 }, { 0, 1 }, { 0, 2 } };

        // All three values have the same key so only the last value should remain.
        Assert.AreEqual(1, dupDictionary.Count);
        Assert.AreEqual(2, dupDictionary[0]);
    }

    [Test]
    public void TestJson()
    {
        var nsDictionary1 = new NSMutableDictionary<string, string>() { { "1", "one" }, { "2", "two" } };
        string json = nsDictionary1.ToJson();

        var nsDictionary2 = NSDictionary<string, string>.FromJson(json);

        Assert.AreEqual(nsDictionary2["1"], "one");
        Assert.AreEqual(nsDictionary2["2"], "two");
    }
}
