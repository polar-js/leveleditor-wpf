using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace leveleditor
{
    public class EntityTemplate
    {
        [JsonProperty(PropertyName = "components")]
        public List<dynamic> Components;

        public EntityTemplate()
        {
            Components = new List<dynamic>();
        }

        public EntityTemplate(List<dynamic> components)
        {
            Components = components;
        }
    }
}
