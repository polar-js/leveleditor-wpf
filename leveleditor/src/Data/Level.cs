using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows;

namespace leveleditor
{
    [JsonObject(ItemRequired = Required.Always)]
    public class Level
    {
        [JsonProperty(PropertyName = "ecsState")]
        public ECSState State { get; set; }
        [JsonProperty(PropertyName = "properties")]
        public LevelProperties Properties { get; set; }

        public Level(ECSState levelState, LevelProperties properties)
        {
            State = levelState;
            Properties = properties;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Level FromJSON(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<Level>(json);
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static bool Verify(dynamic data)
        {
            if (data.ecsState == null) return false;

            if (data.ecsState.systemNames == null || data.ecsState.singletons == null || data.ecsState.entities == null) return false;
            
            if (data.ecsState.singletons.components == null) return false;
            
            foreach (dynamic entity in data.ecsState.entities)
            {
                if (entity.components == null)  return false;

                foreach (dynamic component in entity.components)
                {
                    if (component.type == null) return false;
                }
            }

            return true;
        }

        
    }
}
