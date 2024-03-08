using NUnit.Framework;

using Apple.Core.Runtime;
using System;
using UnityEditorInternal;

public class TestInteropWeakMap
{
    [Test]
    public void TestBasicOperations()
    {
        var instanceMap = new InteropWeakMap<NSString>();

        NSString theString = new NSString("test");
        IntPtr pointer = theString.Pointer;

        instanceMap.Add(theString);
        Assert.AreEqual(instanceMap.Count, 1);

        // The string object should still exist and be in the map.
        Assert.IsTrue(instanceMap.TryGet(pointer, out NSString theString2));

        Assert.AreEqual(theString, theString2);
        Assert.AreEqual("test", theString2.ToString());

        // Trimming the map now shouldn't remove anything.
        instanceMap.Trim();
        Assert.AreEqual(instanceMap.Count, 1);

        instanceMap.Remove(theString);
        Assert.AreEqual(instanceMap.Count, 0);

        // The string object should no longer be in the map.
        Assert.IsFalse(instanceMap.TryGet(pointer, out _));

        instanceMap.Add(theString);
        Assert.AreEqual(instanceMap.Count, 1);

        // destroy the string
        theString2 = null;
        theString.Dispose();
        theString = null;

        // The weak reference is still in the map but TryGet should fail because the object has been disposed.
        Assert.IsFalse(instanceMap.TryGet(pointer, out _));

        // Remove the dangling reference from the map.
        instanceMap.Trim();
        Assert.AreEqual(instanceMap.Count, 0);
    }
}
