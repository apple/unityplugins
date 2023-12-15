using NUnit.Framework;

using Apple.Core.Runtime;
using System;

public class TestNSArray
{
    [Test]
    // Tests: Add (via initializer list), operator[] (get/set), Count, GetEnumerator, IndexOf, Clear
    public void TestBasicOperations()
    {
        var mutableArray = new NSMutableArray<int> { 0, 1, 2, 3, -4, -5, -6 };

        Assert.AreEqual(7, mutableArray.Count);

        for (int i = 0; i < mutableArray.Count; i++)
        {
            mutableArray[i] = Math.Abs(mutableArray[i]);
        }

        NSArray<int> immutableArray = mutableArray;

        Assert.AreEqual(mutableArray.Count, immutableArray.Count);

        for (int i = 0; i < immutableArray.Count; i++)
        {
            Assert.AreEqual(i, mutableArray[i]);
            Assert.AreEqual(i, immutableArray[i]);
        }

        foreach (var value in mutableArray)
        {
            Assert.AreEqual(mutableArray.IndexOf(value), value);
        }

        mutableArray.Clear();
        Assert.AreEqual(0, mutableArray.Count);
    }

    [Test]
    public void TestTryGetValueAt()
    {
        // string
        {
            NSArray<string> array = new NSMutableArray<string> { "Some", "test", "strings" };
            Assert.IsFalse(array.TryGetValueAt(-1, out var _));
            Assert.IsTrue(array.TryGetValueAt(0, out var value0) && value0.Equals("Some"));
            Assert.IsTrue(array.TryGetValueAt(1, out var value1) && value1.Equals("test"));
            Assert.IsTrue(array.TryGetValueAt(2, out var value2) && value2.Equals("strings"));
            Assert.IsFalse(array.TryGetValueAt(3, out var _));
        }


        // NSString
        {
            NSArray<NSString> array = new NSMutableArray<NSString> { new NSString("More"), new NSString("test"), new NSString("NSStrings") };
            Assert.IsFalse(array.TryGetValueAt(-1, out var _));
            Assert.IsTrue(array.TryGetValueAt(0, out var value0) && value0.Equals(new NSString("More")));
            Assert.IsTrue(array.TryGetValueAt(1, out var value1) && value1.Equals(new NSString("test")));
            Assert.IsTrue(array.TryGetValueAt(2, out var value2) && value2.Equals(new NSString("NSStrings")));
            Assert.IsFalse(array.TryGetValueAt(3, out var _));
        }
    }

    [Test]
    public void TestIndexOfContainsInsertRemove()
    {
        // float
        {
            NSMutableArray<float> array = new NSMutableArray<float> { 1f, 2f, 3.14159f };

            var one = 1f;
            var two = 2f;
            var pi = 3.14159f;
            var negativeOne = -1f;
            var nines = 999;
            var zero = 0;

            Assert.AreEqual(3, array.Count);

            Assert.IsTrue(array.Contains(one));
            Assert.IsTrue(array.Contains(two));
            Assert.IsTrue(array.Contains(pi));
            Assert.IsFalse(array.Contains(negativeOne));
            Assert.IsFalse(array.Contains(nines));
            Assert.IsFalse(array.Contains(zero));

            Assert.AreEqual(0, array.IndexOf(one));
            Assert.AreEqual(1, array.IndexOf(two));
            Assert.AreEqual(2, array.IndexOf(pi));
            Assert.AreEqual(-1, array.IndexOf(negativeOne));
            Assert.AreEqual(-1, array.IndexOf(nines));
            Assert.AreEqual(-1, array.IndexOf(zero));

            Assert.IsTrue(array.Remove(two));
            Assert.IsFalse(array.Remove(two));

            Assert.AreEqual(2, array.Count);

            Assert.IsTrue(array.Contains(one));
            Assert.IsFalse(array.Contains(two));
            Assert.IsTrue(array.Contains(pi));

            Assert.AreEqual(0, array.IndexOf(one));
            Assert.AreEqual(-1, array.IndexOf(two));
            Assert.AreEqual(1, array.IndexOf(pi));

            array.RemoveAt(0);

            Assert.AreEqual(1, array.Count);

            Assert.IsFalse(array.Contains(one));
            Assert.IsFalse(array.Contains(two));
            Assert.IsTrue(array.Contains(pi));

            Assert.AreEqual(-1, array.IndexOf(one));
            Assert.AreEqual(-1, array.IndexOf(two));
            Assert.AreEqual(0, array.IndexOf(pi));
        }

        // NSNumber
        {
            NSMutableArray<NSNumber> array = new NSMutableArray<NSNumber> { new NSNumber(1f), new NSNumber(2f), new NSNumber(3.14159f) };

            var one = new NSNumber(1f);
            var two = new NSNumber(2f);
            var pi = new NSNumber(3.14159f);
            var negativeOne = new NSNumber(-1f);
            var nines = new NSNumber(999);
            var zero = new NSNumber(0);

            Assert.AreEqual(3, array.Count);

            Assert.IsTrue(array.Contains(one));
            Assert.IsTrue(array.Contains(two));
            Assert.IsTrue(array.Contains(pi));
            Assert.IsFalse(array.Contains(negativeOne));
            Assert.IsFalse(array.Contains(nines));
            Assert.IsFalse(array.Contains(zero));

            Assert.AreEqual(0, array.IndexOf(one));
            Assert.AreEqual(1, array.IndexOf(two));
            Assert.AreEqual(2, array.IndexOf(pi));
            Assert.AreEqual(-1, array.IndexOf(negativeOne));
            Assert.AreEqual(-1, array.IndexOf(nines));
            Assert.AreEqual(-1, array.IndexOf(zero));

            Assert.IsTrue(array.Remove(two));
            Assert.IsFalse(array.Remove(two));

            Assert.AreEqual(2, array.Count);

            Assert.IsTrue(array.Contains(one));
            Assert.IsFalse(array.Contains(two));
            Assert.IsTrue(array.Contains(pi));

            Assert.AreEqual(0, array.IndexOf(one));
            Assert.AreEqual(-1, array.IndexOf(two));
            Assert.AreEqual(1, array.IndexOf(pi));

            array.RemoveAt(0);

            Assert.AreEqual(1, array.Count);

            Assert.IsFalse(array.Contains(one));
            Assert.IsFalse(array.Contains(two));
            Assert.IsTrue(array.Contains(pi));

            Assert.AreEqual(-1, array.IndexOf(one));
            Assert.AreEqual(-1, array.IndexOf(two));
            Assert.AreEqual(0, array.IndexOf(pi));
        }
    }

    [Test]
    public void TestKnownValueTypes()
    {
        Assert.DoesNotThrow(() =>
        {
            _ = new NSMutableArray<NSObject>();
            _ = new NSMutableArray<NSString>();
            _ = new NSMutableArray<NSNumber>();
            _ = new NSMutableArray<NSMutableArray<NSObject>>();
            _ = new NSMutableArray<NSNull>();
            _ = new NSMutableArray<string>();
            _ = new NSMutableArray<bool>();
            _ = new NSMutableArray<Byte>();
            _ = new NSMutableArray<SByte>();
            _ = new NSMutableArray<Int16>();
            _ = new NSMutableArray<Int32>();
            _ = new NSMutableArray<Int64>();
            _ = new NSMutableArray<UInt16>();
            _ = new NSMutableArray<UInt32>();
            _ = new NSMutableArray<UInt64>();
            _ = new NSMutableArray<Single>();
            _ = new NSMutableArray<Double>();
        }, "Failed to create NSArray<T> for one of the supported types.");
    }

    [Test]
    public void TestInvalidArrayType()
    {
        // attempt to create an array containing an unsupported type
        Assert.Throws<NotImplementedException>(() => new NSMutableArray<Exception>());
    }

    [Test]
    public void TestNullValues()
    {
        // NSArray uses NSNull to represent null values.
        // On the C# side, NSNull is converted to/from null.
        // Check this logic.

        var stringArray = new NSMutableArray<string>() { "NotNull", null, "AlsoNotNull" };
        Assert.AreEqual(stringArray[0], "NotNull");
        Assert.AreEqual(stringArray[1], null);
        Assert.AreEqual(stringArray[2], "AlsoNotNull");

        var objArray = new NSMutableArray<NSObject>() { new NSString("NotNull"), null, new NSString("AlsoNotNull") };
        Assert.AreEqual(objArray[0]?.As<NSString>()?.ToString(), "NotNull");
        Assert.AreEqual(objArray[1]?.As<NSString>()?.ToString(), null);
        Assert.AreEqual(objArray[2]?.As<NSString>()?.ToString(), "AlsoNotNull");

        // If we explicitly add NSNull to the array, it will get unboxed as a C# null value.
        objArray.Add(new NSNull());
        Assert.AreEqual(objArray[3], null);
    }

    [Test]
    public void TestJson()
    {
        var nsArray1 = new NSMutableArray<string>() { "one", "two" };
        string json = nsArray1.ToJson();

        var nsArray2 = NSArray<string>.FromJson(json);

        Assert.AreEqual(nsArray2[0], "one");
        Assert.AreEqual(nsArray2[1], "two");
    }
}
