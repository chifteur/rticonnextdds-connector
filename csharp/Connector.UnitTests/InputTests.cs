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
        public void SetsProperties()
        {
            Input input = connector.GetInput(TestResources.InputName);
            Assert.AreEqual(TestResources.InputName, input.Name);
            Assert.IsNotNull(input.Samples);
        }

        [Test]
        public void DisposeChangesProperty()
        {
            Input input = connector.GetInput(TestResources.InputName);
            Assert.IsFalse(input.Disposed);
            input.Dispose();
            Assert.IsTrue(input.Disposed);
        }

        [Test]
        public void DisposeDoesNotDisposeConnector()
        {
            Input input = connector.GetInput(TestResources.InputName);
            input.Dispose();
            Assert.IsTrue(input.Disposed);
            Assert.IsFalse(connector.Disposed);
        }

        [Test]
        public void DisposingTwiceDoesNotThrowException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            input.Dispose();
            Assert.DoesNotThrow(input.Dispose);
            Assert.IsTrue(input.Disposed);
        }

        [Test]
        public void ReadDoesNotThrowException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            Assert.DoesNotThrow(input.Read);
        }

        [Test]
        public void ReadAfterDisposeThrowsException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            input.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Read);
        }

        [Test]
        public void ReadAfterDisposingConnectorThrowsException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Read);
        }

        [Test]
        public void TakeDoesNotThrowException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            Assert.DoesNotThrow(input.Take);
        }

        [Test]
        public void TakeAfterDisposeThrowsException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            input.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Take);
        }

        [Test]
        public void TakeAfterDisposingConnectorThrowsException()
        {
            Input input = connector.GetInput(TestResources.InputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(input.Take);
        }
    }
}
