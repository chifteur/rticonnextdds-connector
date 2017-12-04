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
    /// Connector output.
    /// </summary>
    public class Output : IDisposable
    {
        readonly Interface.Output output;

        /// <summary>
        /// Initializes a new instance of the <see cref="Output"/> class.
        /// </summary>
        /// <param name="connector">Parent connector.</param>
        /// <param name="entityName">Entity name.</param>
        internal Output(Connector connector, string entityName)
        {
            Name = entityName;
            output = new Interface.Output(connector.InternalConnector, entityName);
            Instance = new Instance(this);
        }

        /// <summary>
        /// Gets the entity name.
        /// </summary>
        /// <value>The entity name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the unique instance associated with this output.
        /// </summary>
        /// <value>The output instance.</value>
        public Instance Instance {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Output"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed {
            get;
            private set;
        }

        internal Interface.Output InternalOutput {
            get { return output; }
        }

        /// <summary>
        /// Clear all the members of the associated instance.
        /// </summary>
        public void ClearValues()
        {
            if (Disposed) {
                throw new ObjectDisposedException(nameof(Output));
            }

            output.Clear();
        }

        /// <summary>
        /// Write the output instance.
        /// </summary>
        public void Write()
        {
            if (Disposed) {
                throw new ObjectDisposedException(nameof(Output));
            }

            output.Write();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Output"/> object.
        /// </summary>
        /// <remarks>
        /// Calling this method doesn't delete the internal DDS DataWriter.
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
                output.Dispose();
            }
        }
    }
}
