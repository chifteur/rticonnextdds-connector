﻿// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
namespace RTI.Connector
{
    /// <summary>
    /// Connector sample writer.
    /// </summary>
    public class Writer
    {
        readonly Connector connector;
        readonly Interface.Writer writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class.
        /// </summary>
        /// <param name="connector">Parent connector.</param>
        /// <param name="entityName">Entity name.</param>
        public Writer(Connector connector, string entityName)
        {
            this.connector = connector;
            Name = entityName;
            writer = new Interface.Writer(connector.InternalConnector, entityName);
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
        /// Gets the unique instance associated with this writer.
        /// </summary>
        /// <value>The writer instance.</value>
        public Instance Instance {
            get;
            private set;
        }

        internal Interface.Writer InternalWriter {
            get { return writer; }
        }

        /// <summary>
        /// Write the writer instance.
        /// </summary>
        public void Write()
        {
            writer.Write();
        }
    }
}
