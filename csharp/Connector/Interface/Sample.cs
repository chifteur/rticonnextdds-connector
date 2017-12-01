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

    sealed class Sample
    {
        readonly Input input;
        readonly int index;

        public Sample(Input input, int index)
        {
            this.input = input;
            this.index = index;
        }

        public int GetNumberFromSample(string field)
        {
            if (input.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            return (int)NativeMethods.RTIDDSConnector_getNumberFromSamples(
                input.Connector.Handle,
                input.EntityName,
                index,
                field);
        }

        public bool GetBoolFromSample(string field)
        {
            if (input.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            return NativeMethods.RTIDDSConnector_getBooleanFromSamples(
                input.Connector.Handle,
                input.EntityName,
                index,
                field) != 0;
        }

        public string GetStringFromSample(string field)
        {
            if (input.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            return NativeMethods.RTIDDSConnector_getStringFromSamples(
                input.Connector.Handle,
                input.EntityName,
                index,
                field);
        }

        public string GetJsonFromSample()
        {
            if (input.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            return NativeMethods.RTIDDSConnector_getJSONSample(
                input.Connector.Handle,
                input.EntityName,
                index);
        }

        public bool GetBoolFromInfo(string field)
        {
            if (input.Connector.Disposed) {
                throw new ObjectDisposedException(nameof(Connector));
            }

            return NativeMethods.RTIDDSConnector_getBooleanFromInfos(
                input.Connector.Handle,
                input.EntityName,
                index,
                field) != 0;
        }

        static class NativeMethods
        {
            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern int RTIDDSConnector_getBooleanFromInfos(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                int index,
                string name);
            
            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern double RTIDDSConnector_getNumberFromSamples(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                int index,
                string name);
            
            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern int RTIDDSConnector_getBooleanFromSamples(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                int index,
                string name);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern string RTIDDSConnector_getStringFromSamples(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                int index,
                string name);

            [DllImport("rtiddsconnector", CharSet = CharSet.Ansi)]
            public static extern string RTIDDSConnector_getJSONSample(
                Connector.ConnectorPtr connectorHandle,
                string entityName,
                int index);
        }
    }
}
