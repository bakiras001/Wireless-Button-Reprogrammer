using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace WBR
{
    internal class DevicePresets
    {
        private static Dictionary<string, List<List<byte>>> Presets = new Dictionary<string, List<List<byte>>>();
        private static readonly string DefaultJson = "{\"HyperX Cloud II Wireless (DTS)\":[[255,187,32,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[255,187,32,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[255,187,32,1,0,0,0,0,0,0,0,0,0,0,0,0,127,0,0],[255,187,32,0,0,0,0,0,0,0,0,0,0,0,0,0,127,0,0]],\"Corsair Virtuoso XT\":[[1,1,142,0,0,0,0,0,197,107,181,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[1,1,142,0,1,0,0,0,197,107,181,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]],\"HyperX Cloud III Wireless\":[[10,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[10,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]],\"HyperX Cloud Alpha\":[[187,35,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[187,35,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]]}";
        private static readonly string FileName = "presets.json";
        private static JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };
        public static void Init()
        {
            try
            {
                string json = FileHandler.ReadFromAppData(FileName);
                if(json == null)
                {
                    json = DefaultJson;
                }
                Presets = JsonSerializer.Deserialize<Dictionary<string, List<List<byte>>>>(json);
                Save();
            }
            catch (Exception e)
            {
                ErrorHandler.NewError(e);
            }
        }
        public static void Add(string name, List<List<byte>> bytes)
        {
            if (!Presets.ContainsKey(name))
            {
                Presets.Add(name, bytes);
            }
            Presets[name] = bytes; // allow user to overwrite possibly existing presets
            Save();
        }
        public static bool Contains(string device, byte[] bytes)
        {
            if (!Presets.ContainsKey(device))
            {
                ErrorHandler.NewError($"Preset {device} couldn't be found!");
                return false;
            }


            var byteArray = Presets[device];

            // amount of possible byte arrays
            int l1 = byteArray.Count();
            // byte array length / fixed packet size
            int l2 = byteArray[0].Count();

            // skip comparison if length not equal
            if (l2 != bytes.Length)
                return false;

            for (int i = 0; i < l1; i++)
            {
                bool same = true;
                for (int j = 0; j < l2; j++)
                {
                    if (bytes[j] != byteArray[i][j])
                        same = false;
                }
                if (same)
                    return true;
            }

            return false;
        }

        public static void Save()
        {
            string jsonString = JsonSerializer.Serialize(Presets, Options);

            FileHandler.WriteToAppData(FileName, jsonString);
        }

    }


}
