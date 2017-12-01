// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
namespace RTI.Connector.UnitTests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    [TestFixture]
    public class InputTests
    {
        Connector connector;

        [SetUp]
        public void SetUp()
        {
            connector = TestResources.CreateSubscriberConnector();
        }

        [TearDown]
        public void TearDown()
        {
            connector?.Dispose();
        }

        [Test]
        public void ConstructorWithNullConnectorThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Input(null, TestResources.InputName));
        }

        [Test]
        public void ConstructorWithNullOrEmptyEntityNameThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Input(connector, null));
            Assert.Throws<ArgumentNullException>(
                () => new Input(connector, string.Empty));
        }

        [Test]
        public void ConstructorWithMissingEntityNameThrowsException()
        {
            Assert.Throws<COMException>(
                () => new Input(connector, "FakeSubscriber::MySquareReader"));
            Assert.Throws<COMException>(
                () => new Input(connector, "MySubscriber::FakeReader"));
        }

        [Test]
        public void ConstructorWithValidConfigIsSuccessful()
        {
            Input input = null;
            Assert.DoesNotThrow(
                () => input = new Input(connector, TestResources.InputName));
            Assert.IsNotNull(input.InternalInput);
        }

        [Test]
        public void ConstructorSetsProperties()
        {
            Input input = new Input(connector, TestResources.InputName);
            Assert.AreEqual(TestResources.InputName, input.Name);
            Assert.IsNotNull(input.Samples);
        }

        [Test]
        public void DisposeChangesProperty()
        {
            Input input = new Input(connector, TestResources.InputName);
            Assert.IsFalse(input.Disposed);
            input.Dispose();
            Assert.IsTrue(input.Disposed);
        }

        [Test]
        public void DisposeDoesNotDisposeConnector()
        {
            Input input = new Input(connector, TestResources.InputName);
            input.Dispose();
            Assert.IsTrue(input.Disposed);
            Assert.IsFalse(connector.Disposed);
        }

        [Test]
        public void DisposingTwiceDoesNotThrowException()
        {
            Input input = new Input(connector, TestResources.InputName);
            input.Dispose();
            Assert.DoesNotThrow(input.Dispose);
            Assert.IsTrue(input.Disposed);
        }

        [Test]
        public void ReadDoesNotThrowException()
        {
            Input input = new Input(connector, TestResources.InputName);
            Assert.DoesNotThrow(input.Read);
        }

        [Test]
        public void ReadAfterDisposeThrowsException()
        {
            Input input = new Input(connector, TestResources.InputName);
            input.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Read);
        }

        [Test]
        public void ReadAfterDisposingConnectorThrowsException()
        {
            Input input = new Input(connector, TestResources.InputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Read);
        }

        [Test]
        public void TakeDoesNotThrowException()
        {
            Input input = new Input(connector, TestResources.InputName);
            Assert.DoesNotThrow(input.Take);
        }

        [Test]
        public void TakeAfterDisposeThrowsException()
        {
            Input input = new Input(connector, TestResources.InputName);
            input.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Take);
        }

        [Test]
        public void TakeAfterDisposingConnectorThrowsException()
        {
            Input input = new Input(connector, TestResources.InputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Take);
        }
    }
}
