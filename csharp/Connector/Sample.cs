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
    using Newtonsoft.Json;
    
    /// <summary>
    /// Sample read with a <see cref="Input"/>.
    /// </summary>
    public class Sample
    {
        readonly Input input;
        readonly int index;
        readonly Interface.Sample internalSample;

        internal Sample(Input input, int index)
        {
            this.input = input;
            this.index = index;
            internalSample = new Interface.Sample(input.InternalInput, index);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Sample"/> contains
        /// data or just metadata information.
        /// </summary>
        /// <value><c>true</c> if contains data; otherwise, <c>false</c>.</value>
        public bool IsValid => internalSample.GetBoolFromInfo("valid_data");

        /// <summary>
        /// Gets the value from an double field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public double GetValueDouble(string field)
        {
            return internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an float field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public float GetValueFloat(string field)
        {
            return (float)internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an byte field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public byte GetValueByte(string field)
        {
            return (byte)internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an short field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public short GetValueInt16(string field)
        {
            return (short)internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an ushort field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        [CLSCompliant(false)]
        public ushort GetValueUInt16(string field)
        {
            return (ushort)internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an int field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public int GetValueInt32(string field)
        {
            return (int)internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an uint field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        [CLSCompliant(false)]
        public uint GetValueUInt32(string field)
        {
            return (uint)internalSample.GetNumberFromSample(field);
        }

        /// <summary>
        /// Gets the value from an boolean field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public bool GetValueBool(string field)
        {
            return internalSample.GetBoolFromSample(field);
        }

        /// <summary>
        /// Gets the value from an string field.
        /// </summary>
        /// <returns>The field value.</returns>
        /// <param name="field">Field name.</param>
        public string GetValueString(string field)
        {
            return internalSample.GetStringFromSample(field);
        }

        /// <summary>
        /// Get the sample as a deserialized-JSON object.
        /// </summary>
        /// <returns>The sample.</returns>
        public object GetSampleAsObject()
        {
            return JsonConvert.DeserializeObject(internalSample.GetJsonFromSample());
        }

        /// <summary>
        /// Deserialize the sample from the JSON internal representation.
        /// </summary>
        /// <returns>The sample.</returns>
        /// <typeparam name="T">Type of the sample.</typeparam>
        public T GetSampleAs<T>()
        {
            return JsonConvert.DeserializeObject<T>(internalSample.GetJsonFromSample());
        }
    }
}
