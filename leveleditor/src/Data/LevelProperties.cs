using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace leveleditor
{
    public struct LevelProperties
    {
        [JsonProperty(PropertyName = "resourcePath")]
        public string ResourcePath { get; set; }
    }
}
