using System.IO;
using Newtonsoft.Json;

namespace CountdownTimerPro.Helpers
{
    public static class JsonHelper
    {
        public static void Save<T>(string fileName, T data)
        {
            string json =
                JsonConvert.SerializeObject(
                    data,
                    Formatting.Indented);

            File.WriteAllText(fileName, json);
        }

        public static T Load<T>(string fileName)
            where T : new()
        {
            if (!File.Exists(fileName))
                return new T();

            string json =
                File.ReadAllText(fileName);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}