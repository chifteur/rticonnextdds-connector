// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
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
