using NUnit.Framework;

using Apple.Core;
using System;
using System.Reflection;

public class TestAvailability
{
    // various runtime environments
    readonly RuntimeEnvironment iOS_0_0 = new (RuntimeOperatingSystem.iOS, 0, 0);
    readonly RuntimeEnvironment macOS_0_0 = new (RuntimeOperatingSystem.macOS, 0, 0);
    readonly RuntimeEnvironment tvOS_0_0 = new (RuntimeOperatingSystem.tvOS, 0, 0);
    readonly RuntimeEnvironment visionOS_0_0 = new (RuntimeOperatingSystem.visionOS, 0, 0);

    readonly RuntimeEnvironment iOS_1_0 = new (RuntimeOperatingSystem.iOS, 1, 0);
    readonly RuntimeEnvironment macOS_1_0 = new (RuntimeOperatingSystem.macOS, 1, 0);
    readonly RuntimeEnvironment tvOS_1_0 = new (RuntimeOperatingSystem.tvOS, 1, 0);
    readonly RuntimeEnvironment visionOS_1_0 = new (RuntimeOperatingSystem.visionOS, 1, 0);

    readonly RuntimeEnvironment iOS_1_5 = new (RuntimeOperatingSystem.iOS, 1, 5);
    readonly RuntimeEnvironment macOS_1_5 = new (RuntimeOperatingSystem.macOS, 1, 5);
    readonly RuntimeEnvironment tvOS_1_5 = new (RuntimeOperatingSystem.tvOS, 1, 5);
    readonly RuntimeEnvironment visionOS_1_5 = new (RuntimeOperatingSystem.visionOS, 1, 5);

    readonly RuntimeEnvironment iOS_2_0 = new (RuntimeOperatingSystem.iOS, 2, 0);
    readonly RuntimeEnvironment macOS_2_0 = new (RuntimeOperatingSystem.macOS, 2, 0);
    readonly RuntimeEnvironment tvOS_3_0 = new (RuntimeOperatingSystem.tvOS, 3, 0);
    readonly RuntimeEnvironment visionOS_4_0 = new (RuntimeOperatingSystem.visionOS, 4, 0);

    readonly RuntimeEnvironment iOS_9_9 = new (RuntimeOperatingSystem.iOS, 9, 9);
    readonly RuntimeEnvironment macOS_9_9 = new (RuntimeOperatingSystem.macOS, 9, 9);
    readonly RuntimeEnvironment tvOS_9_9 = new (RuntimeOperatingSystem.tvOS, 9, 9);
    readonly RuntimeEnvironment visionOS_9_9 = new (RuntimeOperatingSystem.visionOS, 9, 9);

    readonly RuntimeEnvironment iOS_1_2 = new (RuntimeOperatingSystem.iOS, 1, 2);

    readonly RuntimeEnvironment macOS_10_15_5 = new (RuntimeOperatingSystem.macOS, 10, 15, 5);

    // class and methods with no attributes
    public class TestClass0
    {
        public TestClass0() {}
        public void TestMethod1() {}
        public bool TestProperty1 { get; set; }
        public bool TestField1;
        public event EventHandler TestEvent1;
        public enum TestEnum1 { A, B };
        public delegate void TestDelegate1();
    }

    [Test]
    public void TestNoAttributes()
    {
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(iOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(macOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(tvOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(visionOS_0_0));

        Assert.IsTrue(Availability.IsMethodAvailable<TestClass0>(nameof(TestClass0.TestMethod1), env: iOS_0_0));
        Assert.IsTrue(Availability.IsPropertyAvailable<TestClass0>(nameof(TestClass0.TestProperty1), macOS_0_0));
        Assert.IsTrue(Availability.IsFieldAvailable<TestClass0>(nameof(TestClass0.TestField1), tvOS_0_0));
        Assert.IsTrue(Availability.IsEventAvailable<TestClass0>(nameof(TestClass0.TestEvent1), tvOS_3_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0.TestEnum1>(visionOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0.TestDelegate1>(visionOS_0_0));

        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(iOS_1_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(macOS_2_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(tvOS_3_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(visionOS_4_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass0>(macOS_10_15_5));
    }

    // class and methods with Introduced attributes
    [Introduced(iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
    public class TestClass1
    {
        public TestClass1() {}
        public void TestMethod1() {}
        public bool TestProperty1 { get; set; }
        public bool TestField1;
        public event EventHandler TestEvent1;
        public enum TestEnum1 { A, B };
        public void TestOverload() {}

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
        public TestClass1(int foo) {}

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
        public void TestMethod2() {}

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
        public bool TestProperty2 { get; set; }

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
        public bool TestField2;

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
        public event EventHandler TestEvent2;

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
        public enum TestEnum2 { A, B };

        [Introduced(iOS: "1.2", macOS: "10.15.5", tvOS: "3.4", visionOS: "4.5")]
        public void TestOverload(int foo) {}
    }

    [Test]
    public void TestIntroduced()
    {
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(iOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(macOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(tvOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(visionOS_0_0));

        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1>(iOS_1_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1>(macOS_2_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1>(tvOS_3_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1>(visionOS_4_0));

        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1>(iOS_1_5));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(macOS_1_5));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(tvOS_1_5));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1>(visionOS_1_5));

        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1.TestEnum1>(iOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1.TestEnum1>(macOS_9_9));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass1.TestEnum2>(iOS_1_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass1.TestEnum2>(iOS_1_5));

        Assert.IsFalse(Availability.IsEventAvailable<TestClass1>(nameof(TestClass1.TestEvent1), iOS_0_0));
        Assert.IsTrue(Availability.IsEventAvailable<TestClass1>(nameof(TestClass1.TestEvent1), iOS_1_0));

        Assert.IsFalse(Availability.IsEventAvailable<TestClass1>(nameof(TestClass1.TestEvent2), iOS_1_0));
        Assert.IsTrue(Availability.IsEventAvailable<TestClass1>(nameof(TestClass1.TestEvent2), iOS_1_5));
    }

    [Test]
    public void TestOverloads()
    {
        // possible ambiguous cases that don't specify a list of parameters
        
        // no overloads available
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), env: iOS_0_0));

        // all overloads available
        Assert.IsTrue(Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), env: iOS_2_0));

        // mix of availability: ambiguous case throws an exception
        Assert.Throws<AmbiguousMatchException>(() => Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), env: iOS_1_0));

        // non ambiguous cases
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), Type.EmptyTypes, iOS_0_0));
        Assert.IsTrue(Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), Type.EmptyTypes, iOS_1_0));
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), new Type[] { typeof(int) }, iOS_1_0));
        Assert.IsTrue(Availability.IsMethodAvailable<TestClass1>(nameof(TestClass1.TestOverload), new Type[] { typeof(int) }, macOS_10_15_5));
    }

    // class and methods with Unavailable attributes
    [Unavailable(RuntimeOperatingSystem.visionOS)]
    public class TestClass2
    {
        public TestClass2() {}
        public void TestMethod1() {}
        public bool TestProperty1 { get; set; }
        public bool TestField1;
        public event EventHandler TestEvent1;
        public enum TestEnum1 { A, B };

        [Unavailable(RuntimeOperatingSystem.iOS)]
        public TestClass2(int foo) {}

        [Unavailable(RuntimeOperatingSystem.macOS)]
        public void TestMethod2() {}

        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public bool TestProperty2 { get; set; }

        [Unavailable(RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.macOS)]
        public bool TestField2;

        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.macOS)]
        public event EventHandler TestEvent2;

        [Unavailable(RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
        public enum TestEnum2 { A, B };
    }

    [Test]
    public void TestUnavailable()
    {
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2>(iOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2>(macOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2>(tvOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass2>(visionOS_0_0));

        Assert.IsTrue(Availability.IsConstructorAvailable<TestClass2>(Type.EmptyTypes, iOS_0_0));
        Assert.IsTrue(Availability.IsConstructorAvailable<TestClass2>(Type.EmptyTypes, macOS_0_0));
        Assert.IsTrue(Availability.IsConstructorAvailable<TestClass2>(Type.EmptyTypes, tvOS_0_0));
        Assert.IsFalse(Availability.IsConstructorAvailable<TestClass2>(Type.EmptyTypes, visionOS_0_0));

        Assert.IsTrue(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod1), env: iOS_0_0));
        Assert.IsTrue(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod1), env: macOS_0_0));
        Assert.IsTrue(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod1), env: tvOS_0_0));
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod1), env: visionOS_0_0));

        Assert.IsTrue(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty1), iOS_0_0));
        Assert.IsTrue(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty1), macOS_0_0));
        Assert.IsTrue(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty1), tvOS_0_0));
        Assert.IsFalse(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty1), visionOS_0_0));

        Assert.IsTrue(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField1), iOS_0_0));
        Assert.IsTrue(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField1), macOS_0_0));
        Assert.IsTrue(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField1), tvOS_0_0));
        Assert.IsFalse(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField1), visionOS_0_0));

        Assert.IsTrue(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent1), iOS_0_0));
        Assert.IsTrue(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent1), macOS_0_0));
        Assert.IsTrue(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent1), tvOS_0_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent1), visionOS_0_0));

        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2.TestEnum1>(iOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2.TestEnum1>(macOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2.TestEnum1>(tvOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass2.TestEnum1>(visionOS_0_0));


        Assert.IsFalse(Availability.IsConstructorAvailable<TestClass2>(new Type[] { typeof(int) }, iOS_0_0));
        Assert.IsTrue(Availability.IsConstructorAvailable<TestClass2>(new Type[] { typeof(int) }, macOS_0_0));
        Assert.IsTrue(Availability.IsConstructorAvailable<TestClass2>(new Type[] { typeof(int) }, tvOS_0_0));
        Assert.IsFalse(Availability.IsConstructorAvailable<TestClass2>(new Type[] { typeof(int) }, visionOS_0_0));

        Assert.IsTrue(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod2), env: iOS_0_0));
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod2), env: macOS_0_0));
        Assert.IsTrue(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod2), env: tvOS_0_0));
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass2>(nameof(TestClass2.TestMethod2), env: visionOS_0_0));

        Assert.IsTrue(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty2), iOS_0_0));
        Assert.IsTrue(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty2), macOS_0_0));
        Assert.IsFalse(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty2), tvOS_0_0));
        Assert.IsFalse(Availability.IsPropertyAvailable<TestClass2>(nameof(TestClass2.TestProperty2), visionOS_0_0));

        Assert.IsFalse(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField2), iOS_0_0));
        Assert.IsFalse(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField2), macOS_0_0));
        Assert.IsTrue(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField2), tvOS_0_0));
        Assert.IsFalse(Availability.IsFieldAvailable<TestClass2>(nameof(TestClass2.TestField2), visionOS_0_0));

        Assert.IsTrue(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent2), iOS_0_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent2), macOS_0_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent2), tvOS_0_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass2>(nameof(TestClass2.TestEvent2), visionOS_0_0));

        Assert.IsFalse(Availability.IsTypeAvailable<TestClass2.TestEnum2>(iOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass2.TestEnum2>(macOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass2.TestEnum2>(tvOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass2.TestEnum2>(visionOS_0_0));
    }

    [Obsoleted("This class is obsolete!", iOS: "1.2", macOS: "2.3", tvOS: "3.4", visionOS: "4.5")]
    public class TestClass3
    {
        public TestClass3() {}
        public void TestMethod1() {}
        public bool TestProperty1 { get; set; }
        public bool TestField1;
        public event EventHandler TestEvent1;
        public enum TestEnum1 { A, B };

        [Obsoleted("This constructor is obsolete!", iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
        public TestClass3(int foo) {}

        [Obsoleted("This method is obsolete!", iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
        public void TestMethod2() {}

        [Obsoleted("This property is obsolete!", iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
        public bool TestProperty2 { get; set; }

        [Obsoleted("This field is obsolete!", iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
        public bool TestField2;

        [Obsoleted("This event is obsolete!", iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
        public event EventHandler TestEvent2;

        [Obsoleted("This enum is obsolete!", iOS: "1", macOS: "2", tvOS: "3", visionOS: "4")]
        public enum TestEnum2 { A, B };
    }

    [Test]
    public void TestObsoleted()
    {
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass3>(iOS_1_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass3>(iOS_1_5));

        Assert.IsTrue(Availability.IsMethodAvailable<TestClass3>(nameof(TestClass3.TestMethod1), env: visionOS_1_0));
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass3>(nameof(TestClass3.TestMethod1), env: visionOS_9_9));

        Assert.IsTrue(Availability.IsMethodAvailable<TestClass3>(nameof(TestClass3.TestMethod2), env: macOS_1_0));
        Assert.IsFalse(Availability.IsMethodAvailable<TestClass3>(nameof(TestClass3.TestMethod2), env: macOS_2_0));

        Assert.IsTrue(Availability.IsEventAvailable<TestClass3>(nameof(TestClass3.TestEvent1), env: iOS_1_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass3>(nameof(TestClass3.TestEvent1), env: iOS_1_5));

        Assert.IsTrue(Availability.IsEventAvailable<TestClass3>(nameof(TestClass3.TestEvent2), env: iOS_0_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass3>(nameof(TestClass3.TestEvent2), env: iOS_1_0));
    }

    [Introduced(iOS: "1", macOS: "2", tvOS: "3")]
    [Unavailable(RuntimeOperatingSystem.visionOS)]
    [Obsoleted("This class is obsolete!", iOS: "2", macOS: "3", tvOS: "4")]
    public class TestClass4
    {
        public TestClass4() {}
        public void TestMethod1() {}
        public bool TestProperty1 { get; set; }
        public bool TestField1;
        public event EventHandler TestEvent1;
        public enum TestEnum1 { A, B };

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4")]
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        [Obsoleted("This constructor is obsolete!", iOS: "1.3", macOS: "2.4", tvOS: "3.6")]
        public TestClass4(int foo) {}

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4")]
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        [Obsoleted("This method is obsolete!", iOS: "1.3", macOS: "2.4", tvOS: "3.6")]
        public void TestMethod2() {}

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4")]
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        [Obsoleted("This property is obsolete!", iOS: "1.3", macOS: "2.4", tvOS: "3.6")]
        public bool TestProperty2 { get; set; }

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4")]
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        [Obsoleted("This field is obsolete!", iOS: "1.3", macOS: "2.4", tvOS: "3.6")]
        public bool TestField2;

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4")]
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        [Obsoleted("This event is obsolete!", iOS: "1.3", macOS: "2.4", tvOS: "3.6")]
        public event EventHandler TestEvent2;

        [Introduced(iOS: "1.2", macOS: "2.3", tvOS: "3.4")]
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        [Obsoleted("This enum is obsolete!", iOS: "1.3", macOS: "2.4", tvOS: "3.6")]
        public enum TestEnum2 { A, B };
    }

    [Test]
    public void TestCombo()
    {
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(iOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass4>(iOS_1_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass4>(iOS_1_5));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(iOS_2_0));

        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(macOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass4>(macOS_2_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(macOS_9_9));

        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(tvOS_0_0));
        Assert.IsTrue(Availability.IsTypeAvailable<TestClass4>(tvOS_3_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(tvOS_9_9));

        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(visionOS_0_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(visionOS_4_0));
        Assert.IsFalse(Availability.IsTypeAvailable<TestClass4>(visionOS_9_9));

        Assert.IsTrue(Availability.IsEventAvailable<TestClass4>(nameof(TestClass4.TestEvent1), iOS_1_0));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass4>(nameof(TestClass4.TestEvent1), iOS_2_0));

        Assert.IsTrue(Availability.IsEventAvailable<TestClass4>(nameof(TestClass4.TestEvent2), iOS_1_2));
        Assert.IsFalse(Availability.IsEventAvailable<TestClass4>(nameof(TestClass4.TestEvent2), iOS_1_5));
    }
}