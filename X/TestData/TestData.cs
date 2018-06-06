using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace X.TestData
{
    public static class TestData
    {
        public static String TAG = "TestData";

        public static JObject getTestData(String testDataFile)
        {

            var assembly = typeof(TestData).GetTypeInfo().Assembly;
            System.IO.Stream stream = assembly.GetManifestResourceStream("X.TestData." + testDataFile);
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            stream.Dispose();
            JObject retVal = null;

            if (text.Length != 0)
            {
                retVal = JObject.Parse(text);
            }
            return retVal;
        }
    }
}
