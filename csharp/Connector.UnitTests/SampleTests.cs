// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
namespace RTI.Connext.Connector.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using NUnit.Framework;

    // Since we are sending and waiting for samples in the same domain and topic
    // we need to run these test sequentially.
    [TestFixture, SingleThreaded]
    public class SampleTests
    {
        Connector connector;
        Output output;
        Input input;
        SampleCollection samples;

        [SetUp]
        public void SetUp()
        {
            connector = TestResources.CreatePubSubConnector();
            output = connector.GetOutput(TestResources.OutputName);
            input = connector.GetInput(TestResources.InputName);
            samples = input.Samples;

            // Wait for discovery
            Thread.Sleep(100);
        }

        [TearDown]
        public void TearDown()
        {
            input.Dispose();
            output.Dispose();
            connector.Dispose();
        }

        [Test]
        public void GetNumberOfSamplesReturnsValidValue()
        {
            SendAndTakeOrReadStandardSample(true);
            Assert.AreEqual(1, samples.Count);
        }

        [Test]
        public void SampleIteratorContainsOneSample()
        {
            SendAndTakeOrReadStandardSample(true);
            int count = 0;
            foreach (var sample in samples) {
                count++;
            }

            Assert.AreEqual(1, count);
        }

        [Test]
        public void ListContainsJustOneSampleWithNormalEnumerator()
        {
            SendAndTakeOrReadStandardSample(true);
            var list = NonGenericSampleEnumerable(samples);
            Assert.AreEqual(1, list.Cast<Sample>().Count());
        }

        [Test]
        public void RecievedSampleIsValid()
        {
            SendAndTakeOrReadStandardSample(true);
            Assert.IsTrue(samples.Single().IsValid);
        }

        [Test]
        public void ReceivedSampleHasValidNumberFields()
        {
            output.ClearValues();
            output.Instance.SetValue("x", -1453873);
            output.Instance.SetValue("byte", 0x80);
            output.Instance.SetValue("ushort", 0xCAFE);
            output.Instance.SetValue("short", -20812);
            output.Instance.SetValue("uint", 0xCAFEBABE);
            output.Instance.SetValue("angle", -3.14);
            output.Instance.SetValue("double", -3.14);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            input.Take();
            Sample sample = samples.Single();

            Assert.AreEqual(-1453873, sample.GetValueInt32("x"));
            Assert.AreEqual(0x80, sample.GetValueByte("byte"));
            Assert.AreEqual(0xCAFE, sample.GetValueUInt16("ushort"));
            Assert.AreEqual(-20812, sample.GetValueInt16("short"));
            Assert.AreEqual(0xCAFEBABE, sample.GetValueUInt32("uint"));
            Assert.That(sample.GetValueFloat("angle"), Is.InRange(-3.140005, -3.140000));
            Assert.That(sample.GetValueDouble("double"), Is.InRange(-3.140005, -3.140000));
        }

        [Test]
        public void ReceivedSampleHasValidStringField()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.AreEqual("BLUE", sample.GetValueString("color"));
        }

        [Test]
        public void ReceivedSampleHasValidBoolField()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.AreEqual(true, sample.GetValueBool("hidden"));
        }

        [Test]
        public void GetNonExistingFieldsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetValueInt32("fakeInt"));
            Assert.DoesNotThrow(() => sample.GetValueString("fakeString"));
            Assert.DoesNotThrow(() => sample.GetValueBool("fakeBool"));
        }

        [Test]
        public void GetWrongVariableTypeForIntsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetValueInt32("color"));
            Assert.DoesNotThrow(() => sample.GetValueInt32("hidden"));
        }

        [Test]
        public void GetWrongVariableTypeForStringsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetValueString("x"));
            Assert.DoesNotThrow(() => sample.GetValueString("hidden"));
            Assert.DoesNotThrow(() => sample.GetValueString("angle"));
            Assert.DoesNotThrow(() => sample.GetValueString("fillKind"));
        }

        [Test]
        public void GetWrongVariableTypeForBoolsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetValueBool("x"));
            Assert.DoesNotThrow(() => sample.GetValueBool("color"));
            Assert.DoesNotThrow(() => sample.GetValueBool("angle"));
            Assert.DoesNotThrow(() => sample.GetValueBool("fillKind"));
        }

        [Test]
        public void TakeRemovesSamples()
        {
            SendAndTakeOrReadStandardSample(true);
            Assert.AreEqual(1, samples.Count);
            input.Take();
            Assert.AreEqual(0, samples.Count);
        }

        [Test]
        public void ReadDoesNotRemoveSamples()
        {
            SendAndTakeOrReadStandardSample(false);
            Assert.AreEqual(1, samples.Count);
            input.Read();
            Assert.AreEqual(1, samples.Count);
            Assert.AreEqual(4, samples.First().GetValueInt32("y"));
        }

        [Test]
        public void TakeAfterReadRemovesSample()
        {
            SendAndTakeOrReadStandardSample(false);
            Assert.AreEqual(1, samples.Count);
            input.Take();
            Assert.AreEqual(1, samples.Count);
            input.Take();
            Assert.AreEqual(0, samples.Count);
        }

        [Test]
        public void GetValidObjectSample()
        {
            MyClassType obj = new MyClassType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            MyClassType received = sample.GetSampleAs<MyClassType>();

            Assert.AreEqual("test", received.Color);
            Assert.AreEqual(3, received.X);
            Assert.AreEqual(true, received.Hidden);
        }

        [Test]
        public void GetValidStructSample()
        {
            MyStructType obj = new MyStructType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            MyStructType received = sample.GetSampleAs<MyStructType>();

            Assert.AreEqual("test", received.Color);
            Assert.AreEqual(3, received.X);
            Assert.AreEqual(true, received.Hidden);
        }

        [Test]
        public void SendStructAndReceiveClassSample()
        {
            MyStructType obj = new MyStructType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            MyClassType received = sample.GetSampleAs<MyClassType>();

            Assert.AreEqual("test", received.Color);
            Assert.AreEqual(3, received.X);
            Assert.AreEqual(true, received.Hidden);
        }

        [Test]
        public void SendAnonymousAndReceiveObjectSample()
        {
            var obj = new {
                color = "test",
                x = 3,
                hidden = true
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            dynamic received = sample.GetSampleAsObject();

            Assert.AreEqual("test", received.color.Value);
            Assert.AreEqual(3, received.x.Value);
            Assert.AreEqual(true, received.hidden.Value);
        }

        [Test]
        public void SendDictionaryAndReceiveSample()
        {
            var obj = new Dictionary<string, object> {
                { "color", "test" },
                { "x", 3 },
                { "hidden", true }
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            var received = sample.GetSampleAs<Dictionary<string, object>>();

            Assert.AreEqual("test", received["color"]);
            Assert.AreEqual(3, received["x"]);
            Assert.AreEqual(true, received["hidden"]);
        }

        [Test]
        public void GetCompleteClassSample()
        {
            ComplexType obj = new ComplexType {
                Color = "test",
                X = 3,
                Angle = 3.14f,
                Hidden = true,
                List = new[] { 0, 1, 2, 3, 4 },
                Inner = new ComplexType.InnerType { Z = -1 }
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            ComplexType received = sample.GetSampleAs<ComplexType>();

            Assert.AreEqual("test", received.Color);
            Assert.AreEqual(3, received.X);
            Assert.AreEqual(true, received.Hidden);
            Assert.AreEqual(3.14f, received.Angle);
            Assert.AreEqual(5, received.List.Length);
            Assert.AreEqual(3, received.List[3]);
            Assert.AreEqual(-1, received.Inner.Z);
        }

        [Test]
        public void GetClassWithInvalidFieldsThrowsException()
        {
            MyInvalidClassType obj = new MyInvalidClassType {
                Color = 3,
                X = 3.3,
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(
                () => sample.GetSampleAs<MyInvalidClassType>());
        }

        [Test]
        public void GetClassWithMissingFieldsIsEmpty()
        {
            MyFakeFieldsTypes obj = new MyFakeFieldsTypes {
                Color = "blue",
                X = 3,
                Fake = 4,
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            MyFakeFieldsTypes received = sample.GetSampleAs<MyFakeFieldsTypes>();

            Assert.AreEqual("blue", received.Color);
            Assert.AreEqual(3, received.X);
            Assert.AreEqual(0, received.Fake);
        }

        [Test]
        public void GetNumberSamplesAfterDisposingConnectorThrowsException()
        {
            SendAndTakeOrReadStandardSample(true);
            connector.Dispose();
            int count = 0;
            Assert.Throws<ObjectDisposedException>(() => count = samples.Count);
            Assert.Throws<ObjectDisposedException>(() => count = samples.Count());
            Assert.Throws<ObjectDisposedException>(() => samples.Single());
        }

        [Test]
        public void GetFieldsAfterDisposingConnectorThrowsException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            bool validSample = false;
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(() => sample.GetValueInt32("x"));
            Assert.Throws<ObjectDisposedException>(() => sample.GetValueString("color"));
            Assert.Throws<ObjectDisposedException>(() => sample.GetValueBool("hidden"));
            Assert.Throws<ObjectDisposedException>(() => validSample = sample.IsValid);
        }

        [Test]
        public void GetJsonSampleAfterDisposingConnectorThrowsException()
        {
            MyClassType obj = new MyClassType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            SendAndTakeObjectSample(obj);
            Sample sample = samples.Single();
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(() => sample.GetSampleAs<MyStructType>());
        }

        [Test]
        public void SetInstanceFieldDoesNotCleanJsonObj()
        {
            output.Instance.SetValue("shapesize", 10);
            MyClassType obj = new MyClassType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            output.Instance.SetValuesFrom(obj);
            output.Instance.SetValue("x", 5);
            output.Instance.SetValue("y", 4);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            input.Take();
            Sample sample = samples.Single();

            Assert.AreEqual("test", sample.GetValueString("color"));
            Assert.AreEqual(5, sample.GetValueInt32("x"));
            Assert.AreEqual(true, sample.GetValueBool("hidden"));
            Assert.AreEqual(4, sample.GetValueInt32("y"));
            Assert.AreEqual(10, sample.GetValueInt32("shapesize"));
        }

        [Test]
        public void SetJsonObjDoesNotResetInstance()
        {
            output.Instance.SetValue("shapesize", 10);
            output.Instance.SetValue("x", 5);
            output.Instance.SetValue("y", 4);
            MyClassType obj = new MyClassType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            output.Instance.SetValuesFrom(obj);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            input.Take();
            Sample sample = samples.Single();

            Assert.AreEqual("test", sample.GetValueString("color"));
            Assert.AreEqual(3, sample.GetValueInt32("x"));
            Assert.AreEqual(true, sample.GetValueBool("hidden"));
            Assert.AreEqual(4, sample.GetValueInt32("y"));
            Assert.AreEqual(10, sample.GetValueInt32("shapesize"));
        }

        // This is just for coverage, all IEnumerable<T> implementations
        // have also the non-generic version, we are testing it here:
        // Helper extension method
        static IEnumerable NonGenericSampleEnumerable(IEnumerable samples)
        {
            foreach (object sample in samples) {
                yield return sample;
            }
        }

        void SendAndTakeOrReadStandardSample(bool take)
        {
            output.Instance.SetValue("x", 3);
            output.Instance.SetValue("y", 4);
            output.Instance.SetValue("color", "BLUE");
            output.Instance.SetValue("hidden", true);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            if (take) {
                input.Take();
            } else {
                input.Read();
            }
        }

        void SendAndTakeObjectSample(object obj)
        {
            output.Instance.SetValuesFrom(obj);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            input.Take();
        }
    }
}
