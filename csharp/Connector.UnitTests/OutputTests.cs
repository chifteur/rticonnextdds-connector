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
        public void SetsProperties()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            Assert.AreEqual(TestResources.OutputName, output.Name);
            Assert.IsNotNull(output.Instance);
            Assert.IsNotNull(output.InternalOutput);
            Assert.IsFalse(output.Disposed);
        }

        [Test]
        public void WriteAfterDisposeThrowsException()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            output.Dispose();
            Assert.Throws<ObjectDisposedException>(output.Write);
        }

        [Test]
        public void WriteWithDisposedConnectorThrowsException()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(output.Write);
        }

        [Test]
        public void WriteDoesNotThrowException()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            Assert.DoesNotThrow(output.Write);
        }

        [Test]
        public void ClearDoesNotThrowExceptionWithoutSettingFields()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            Assert.DoesNotThrow(output.ClearValues);
        }

        [Test]
        public void ClearDoesNotThrowExceptionAfterSettingFields()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            output.Instance.SetValue("x", 3);
            Assert.DoesNotThrow(output.ClearValues);
        }

        [Test]
        public void ClearDoesNotThrowExceptionAfterInvalidAssignment()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            output.Instance.SetValue("fakeInt", 4);
            Assert.DoesNotThrow(output.ClearValues);

            Assert.DoesNotThrow(() => output.Instance.SetValue("x", "test"));
            Assert.DoesNotThrow(output.ClearValues);

            Assert.DoesNotThrow(() => output.Instance.SetValue("color", 3));
            Assert.DoesNotThrow(output.ClearValues);

            Assert.DoesNotThrow(() => output.Instance.SetValue("hidden", "test"));
            Assert.DoesNotThrow(output.ClearValues);
        }

        [Test]
        public void ClearFieldsAfterDisposeThrowsException()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            output.Dispose();
            Assert.Throws<ObjectDisposedException>(output.ClearValues);
        }

        [Test]
        public void ClearFieldsAfterDisposingConnectorThrowsException()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            connector.Dispose();
            Assert.Throws<ObjectDisposedException>(output.ClearValues);
        }

        [Test]
        public void DisposeChangesProperty()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            Assert.IsFalse(output.Disposed);
            output.Dispose();
            Assert.IsTrue(output.Disposed);
        }

        [Test]
        public void DisposeDoesNotDisposeConnector()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            output.Dispose();
            Assert.IsTrue(output.Disposed);
            Assert.IsFalse(connector.Disposed);
        }

        [Test]
        public void DisposingTwiceDoesNotThrowException()
        {
            Output output = connector.GetOutput(TestResources.OutputName);
            output.Dispose();
            Assert.DoesNotThrow(output.Dispose);
            Assert.IsTrue(output.Disposed);
        }
    }
}
