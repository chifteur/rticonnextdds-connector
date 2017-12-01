// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
namespace RTI.Connext.Connector.UnitTests
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    class MyClassType
    {
        public string Color { get; set; }

        public int X { get; set; }

        public bool Hidden { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    class MyInvalidClassType
    {
        public int Color { get; set; }

        public double X { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    class MyFakeFieldsTypes
    {
        public string Color { get; set; }

        public int X { get; set; }

        public bool Hidden { get; set; }

        public int Fake { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    struct MyStructType
    {
        public string Color { get; set; }

        public int X { get; set; }

        public bool Hidden { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    class ComplexType : MyClassType
    {
        public float Angle { get; set; } 

        public int[] List { get; set; }

        public InnerType Inner { get; set; }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class InnerType
        {
            public int Z { get; set; }
        }
    }
}
