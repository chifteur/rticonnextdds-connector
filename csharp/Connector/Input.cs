// ﻿   (c) 2005-2017 Copyright, Real-Time Innovations, All rights reserved.
//
// RTI grants Licensee a license to use, modify, compile, and create
// derivative works of the Software.  Licensee has the right to distribute
// object form only for use with RTI products. The Software is provided
// "as is", with no warranty of any type, including any warranty for fitness
// for any purpose. RTI is under no obligation to maintain or support the
// Software.  RTI shall not be liable for any incidental or consequential
// damages arising out of the use or inability to use the software.
namespace RTI.Connext.Connector
{
    using System;

    /// <summary>
    /// Connector input.
    /// </summary>
    public class Input : IDisposable
    {
        readonly Interface.Input input;

        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class.
        /// </summary>
        /// <param name="connector">Parent connector.</param>
        /// <param name="entityName">Entity name.</param>
        internal Input(Connector connector, string entityName)
        {
            Name = entityName;
            input = new Interface.Input(
                connector.InternalConnector,
                entityName);
            Samples = new SampleCollection(this);
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the samples read or taken from this input.
        /// </summary>
        /// <value>The samples read or taken.</value>
        public SampleCollection Samples {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Input"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed {
            get;
            private set;
        }

        internal Interface.Input InternalInput {
            get { return input; }
        }

        /// <summary>
        /// Reads samples with this input and do not remove them from the
        /// internal queue.
        /// </summary>
        /// <remarks>
        /// The samples are accessible from the <see cref="Samples"/> property. 
        /// </remarks>
        public void Read()
        {
            if (Disposed) {
                throw new ObjectDisposedException(nameof(Input));
            }

            input.Read();
        }

        /// <summary>
        /// Reads samples with this input and remove them from the
        /// internal queue.
        /// </summary>
        /// <remarks>
        /// The samples are accesible from the <see cref="Samples"/> property. 
        /// </remarks>
        public void Take()
        {
            if (Disposed) {
                throw new ObjectDisposedException(nameof(Input));
            }

            input.Take();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Input"/> object.
        /// </summary>
        /// <remarks>
        /// Calling this method doesn't delete the internal DDS DataReader.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool freeManagedResources)
        {
            if (Disposed) {
                return;
            }

            Disposed = true;
            if (freeManagedResources) {
                input.Dispose();
            }
        }
    }
}
