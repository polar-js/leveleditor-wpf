using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leveleditor
{
    public class ECSState
    {
        public List<string> systemNames;
        public List<EntityTemplate> entities;
        public EntityTemplate singletons;

        public ECSState(List<string> systemNames, List<EntityTemplate> entities, EntityTemplate singletons)
        {
            this.systemNames = systemNames;
            this.entities = entities;
            this.singletons = singletons;
        }
    }
}
