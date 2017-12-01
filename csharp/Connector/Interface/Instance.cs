// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
namespace RTI.Connext.Connector.Interface
{
    using System;
    using System.Runtime.InteropServices;

    sealed class Instance
    {
        readonly Output output;

        public Instance(Output output)
        {
            this.output = output;
        }

        public void SetNumber(string field, int val)
        {
            if (output.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_setNumberIntoSamples(
                output.Connector.Handle,
                output.EntityName,
                field,
                val);
        }

        public void SetBool(string field, bool val)
        {
            if (output.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_setBooleanIntoSamples(
                output.Connector.Handle,
                output.EntityName,
                field,
                val ? 1 : 0);
        }

        public void SetString(string field, string val)
        {
            if (output.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_setStringIntoSamples(
                output.Connector.Handle,
                output.EntityName,
                field,
                val);
        }

        public void SetJson(string json)
        {
            if (output.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_setJSONInstance(
                output.Connector.Handle,
                output.EntityName,
                json);
        }

        public void Clear()
        {
            if (output.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            NativeMethods.RTIDDSConnector_clear(
                output.Connector.Handle,
                output.EntityName);
        }

        static class NativeMethods
        {
            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_setNumberIntoSamples(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                string name,
                double val);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_setBooleanIntoSamples(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                string name,
                int val);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_setStringIntoSamples(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                string name,
                string val);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_setJSONInstance(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                string json);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern void RTIDDSConnector_clear(
                Connector.ConnectorPtr connectorHandle,
                string entityName);
        }
    }
}
