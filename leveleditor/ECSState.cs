using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leveleditor
{
    class ECSState
    {
        public string[] systemNames;
        public List<EntityTemplate> entities;
        public EntityTemplate singletons;

        public ECSState(string[] systemNames, List<EntityTemplate> entities, EntityTemplate singletons)
        {
            this.systemNames = systemNames;
            this.entities = entities;
            this.singletons = singletons;
        }
    }
}
