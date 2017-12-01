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
        public void ReceivedSampleHasValidIntFields()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.AreEqual(3, sample.GetInt("x"));
            Assert.AreEqual(4, sample.GetInt("y"));
        }

        [Test]
        public void ReceivedSampleHasValidStringField()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.AreEqual("BLUE", sample.GetString("color"));
        }

        [Test]
        public void ReceivedSampleHasValidBoolField()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.AreEqual(true, sample.GetBool("hidden"));
        }

        [Test]
        public void GetGenericReturnsValidValues()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.AreEqual(3, sample.Get<int>("x"));
            Assert.AreEqual(4, sample.Get<int>("y"));
            Assert.AreEqual("BLUE", sample.Get<string>("color"));
            Assert.AreEqual(true, sample.Get<bool>("hidden"));
        }

        [Test]
        public void GetGenericWithInvalidFormatThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.Throws<FormatException>(() => sample.Get<double>("x"));
            Assert.Throws<FormatException>(() => sample.Get<decimal>("x"));
            Assert.Throws<FormatException>(() => sample.Get<float>("x"));
            Assert.Throws<FormatException>(() => sample.Get<byte>("x"));
            Assert.Throws<FormatException>(() => sample.Get<sbyte>("x"));
            Assert.Throws<FormatException>(() => sample.Get<ushort>("x"));
            Assert.Throws<FormatException>(() => sample.Get<short>("x"));
            Assert.Throws<FormatException>(() => sample.Get<uint>("x"));
            Assert.Throws<FormatException>(() => sample.Get<long>("x"));
            Assert.Throws<FormatException>(() => sample.Get<ulong>("x"));
        }

        [Test]
        public void GetNonExistingFieldsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetInt("fakeInt"));
            Assert.DoesNotThrow(() => sample.GetString("fakeString"));
            Assert.DoesNotThrow(() => sample.GetBool("fakeBool"));
        }

        [Test]
        public void GetWrongVariableTypeForIntsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetInt("color"));
            Assert.DoesNotThrow(() => sample.GetInt("hidden"));
            Assert.DoesNotThrow(() => sample.GetInt("angle"));
            Assert.DoesNotThrow(() => sample.GetInt("fillKind"));
        }

        [Test]
        public void GetWrongVariableTypeForStringsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetString("x"));
            Assert.DoesNotThrow(() => sample.GetString("hidden"));
            Assert.DoesNotThrow(() => sample.GetString("angle"));
            Assert.DoesNotThrow(() => sample.GetString("fillKind"));
        }

        [Test]
        public void GetWrongVariableTypeForBoolsDoesNotThrowException()
        {
            SendAndTakeOrReadStandardSample(true);
            Sample sample = samples.Single();
            Assert.DoesNotThrow(() => sample.GetBool("x"));
            Assert.DoesNotThrow(() => sample.GetBool("color"));
            Assert.DoesNotThrow(() => sample.GetBool("angle"));
            Assert.DoesNotThrow(() => sample.GetBool("fillKind"));
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
            Assert.AreEqual(4, samples.First().GetInt("y"));
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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            MyClassType received = sample.GetAs<MyClassType>();

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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            MyStructType received = sample.GetAs<MyStructType>();

            Assert.AreEqual("test", received.Color);
            Assert.AreEqual(3, received.X);
            Assert.AreEqual(true, received.Hidden);
        }

        [Test]
        public void SendClassAndReceiveStructSample()
        {
            MyStructType obj = new MyStructType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            MyClassType received = sample.GetAs<MyClassType>();

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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            dynamic received = sample.GetAsObject();

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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            var received = sample.GetAs<Dictionary<string, object>>();

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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            ComplexType received = sample.GetAs<ComplexType>();

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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(
                () => sample.GetAs<MyInvalidClassType>());
        }

        [Test]
        public void GetClassWithMissingFieldsIsEmpty()
        {
            MyFakeFieldsTypes obj = new MyFakeFieldsTypes {
                Color = "blue",
                X = 3,
                Fake = 4,
            };

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            MyFakeFieldsTypes received = sample.GetAs<MyFakeFieldsTypes>();

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
            Assert.Throws<ObjectDisposedException>(() => sample.Get<int>("x"));
            Assert.Throws<ObjectDisposedException>(() => sample.Get<string>("color"));
            Assert.Throws<ObjectDisposedException>(() => sample.Get<bool>("hidden"));
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

            SendAndTakeOrReadObjectSample(obj, true);
            Sample sample = samples.Single();
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(() => sample.GetAs<MyStructType>());
        }

        [Test]
        public void SetInstanceFieldDoesNotCleanJsonObj()
        {
            output.Instance.Set("shapesize", 10);
            MyClassType obj = new MyClassType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            output.Instance.SetFrom(obj);
            output.Instance.Set("x", 5);
            output.Instance.Set("y", 4);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            input.Take();
            Sample sample = samples.Single();

            Assert.AreEqual("test", sample.Get<string>("color"));
            Assert.AreEqual(5, sample.Get<int>("x"));
            Assert.AreEqual(true, sample.Get<bool>("hidden"));
            Assert.AreEqual(4, sample.Get<int>("y"));
            Assert.AreEqual(10, sample.Get<int>("shapesize"));
        }

        [Test]
        public void SetJsonObjDoesNotResetInstance()
        {
            output.Instance.Set("shapesize", 10);
            output.Instance.Set("x", 5);
            output.Instance.Set("y", 4);
            MyClassType obj = new MyClassType {
                Color = "test",
                X = 3,
                Hidden = true
            };

            output.Instance.SetFrom(obj);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            input.Take();
            Sample sample = samples.Single();

            Assert.AreEqual("test", sample.Get<string>("color"));
            Assert.AreEqual(3, sample.Get<int>("x"));
            Assert.AreEqual(true, sample.Get<bool>("hidden"));
            Assert.AreEqual(4, sample.Get<int>("y"));
            Assert.AreEqual(10, sample.Get<int>("shapesize"));
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
            output.Instance.Set("x", 3);
            output.Instance.Set("y", 4);
            output.Instance.Set("color", "BLUE");
            output.Instance.Set("hidden", true);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            if (take) {
                input.Take();
            } else {
                input.Read();
            }
        }

        void SendAndTakeOrReadObjectSample(object obj, bool take)
        {
            output.Instance.SetFrom(obj);
            output.Write();

            Assert.IsTrue(connector.Wait(1000));
            if (take) {
                input.Take();
            } else {
                input.Read();
            }
        }
    }
}
