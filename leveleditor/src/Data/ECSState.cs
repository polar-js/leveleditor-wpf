using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.ComponentModel;

namespace leveleditor
{
    public class ECSState
    {
        [JsonProperty(PropertyName = "systemNames")]
        public List<string> SystemNames { get; set; }

        [JsonProperty(PropertyName = "entities")]
        public List<EntityTemplate> Entities { get; set; }
        [JsonProperty(PropertyName = "singletons")]
        public EntityTemplate Singletons { get; set; }

        public ECSState(List<string> systemNames, List<EntityTemplate> entities, EntityTemplate singletons)
        {
            SystemNames = systemNames;
            Entities = entities;
            Singletons = singletons;
        }

        public ECSState()
        {
            SystemNames = new List<string>();
            Entities = new List<EntityTemplate>();
            Singletons = new EntityTemplate();
        }
    }
}
