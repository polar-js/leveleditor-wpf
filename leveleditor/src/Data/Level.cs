using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace leveleditor
{
    public class Level
    {
        private ECSState m_LevelState;
        public LevelProperties Properties { get; set; }

        public Level(ECSState levelState, LevelProperties properties)
        {
            m_LevelState = levelState;
            Properties = properties;
        }

        public static Level FromJSON(string json)
        {
            dynamic levelData = JObject.Parse(json);

            if (Verify(levelData))
            {
                return new Level(levelData.ecsState.ToObject<ECSState>(), new LevelProperties { ResourcePath = levelData.resourcePath ?? "" });
            }
            return null;
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
