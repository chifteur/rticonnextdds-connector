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
    public class OutputTests
    {
        Connector connector;

        [SetUp]
        public void SetUp()
        {
            connector = TestResources.CreatePublisherConnector();
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
                () => new Output(null, TestResources.OutputName));
        }

        [Test]
        public void ConstructorWithNullOrEmptyEntityNameThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Output(connector, null));
            Assert.Throws<ArgumentNullException>(
                () => new Output(connector, string.Empty));
        }

        [Test]
        public void ConstructorWithMissingEntityNameThrowsException()
        {
            Assert.Throws<COMException>(
                () => new Output(connector, "FakePublisher::MySquareWriter"));
            Assert.Throws<COMException>(
                () => new Output(connector, "MyPublisher::FakeWriter"));
        }

        [Test]
        public void ConstructorWithValidConfigIsSuccessful()
        {
            Output output = null;
            Assert.DoesNotThrow(
                () => output = new Output(connector, TestResources.OutputName));
            Assert.IsNotNull(output.InternalOutput);
        }

        [Test]
        public void ConstructorSetsProperties()
        {
            Output output = new Output(connector, TestResources.OutputName);
            Assert.AreEqual(TestResources.OutputName, output.Name);
            Assert.IsNotNull(output.Instance);
        }

        [Test]
        public void WriteAfterDisposeThrowsException()
        {
            Output output = new Output(connector, TestResources.OutputName);
            output.Dispose();
            Assert.Throws<ObjectDisposedException>(output.Write);
        }

        [Test]
        public void WriteWithDisposedConnectorThrowsException()
        {
            Output output = new Output(connector, TestResources.OutputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(output.Write);
        }

        [Test]
        public void WriteDoesNotThrowException()
        {
            Output output = new Output(connector, TestResources.OutputName);
            Assert.DoesNotThrow(output.Write);
        }

        [Test]
        public void DisposeChangesProperty()
        {
            Output output = new Output(connector, TestResources.OutputName);
            Assert.IsFalse(output.Disposed);
            output.Dispose();
            Assert.IsTrue(output.Disposed);
        }

        [Test]
        public void DisposeDoesNotDisposeConnector()
        {
            Output output = new Output(connector, TestResources.OutputName);
            output.Dispose();
            Assert.IsTrue(output.Disposed);
            Assert.IsFalse(connector.Disposed);
        }

        [Test]
        public void DisposingTwiceDoesNotThrowException()
        {
            Output output = new Output(connector, TestResources.OutputName);
            output.Dispose();
            Assert.DoesNotThrow(output.Dispose);
            Assert.IsTrue(output.Disposed);
        }
    }
}
