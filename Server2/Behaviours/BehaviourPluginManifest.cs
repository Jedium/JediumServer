using System.IO;
using Newtonsoft.Json;

namespace Server2.Behaviours
{
    public class BehaviourPluginManifest
    {
        public string ClientDLL;
        public string Name;
        public string ServerDLL;
        public string SharedDLL;
        public string Version;


        public void SaveToFile(string filename)
        {
            string serial = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(serial);
                sw.Close();
            }
        }

        public static BehaviourPluginManifest LoadFromFile(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                string serial = sr.ReadToEnd();

                BehaviourPluginManifest ret = JsonConvert.DeserializeObject<BehaviourPluginManifest>(serial);

                return ret;
            }
        }
    }
}