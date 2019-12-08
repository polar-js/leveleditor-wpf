using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leveleditor
{
    class ShaderLibrary
    {
        private Dictionary<string, Shader> m_Shaders;

        public ShaderLibrary()
        {
            m_Shaders = new Dictionary<string, Shader>();
        }

        public void Add(Shader shader)
        {
            if (m_Shaders.ContainsKey(shader.Name))
                throw new Exception("Shader already exists!");
            else
                m_Shaders.Add(shader.Name, shader);
        }

        public void Add(Shader shader, string name)
        {
            if (m_Shaders.ContainsKey(name))
                throw new Exception("Shader already exists!");
            else
                m_Shaders.Add(name, shader);
        }

        public void Set(Shader shader)
        {
            if (m_Shaders.ContainsKey(shader.Name))
                m_Shaders[shader.Name] = shader;
            else
                m_Shaders.Add(shader.Name, shader);
        }

        public void Set(Shader shader, string name)
        {
            if (m_Shaders.ContainsKey(name))
                m_Shaders[name] = shader;
            else
                m_Shaders.Add(name, shader);
        }

        public Shader Get(string name)
        {
            return m_Shaders[name];
        }

        public bool Exists(string name)
        {
            return m_Shaders.ContainsKey(name);
        }
    }
}
