using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using static leveleditor.GL;
using static SharpGL.OpenGL;
using GlmNet;

namespace leveleditor
{
    class Shader
    {
        private uint m_RendererID;
        private string m_Name;
        private Dictionary<string, int> m_Locations;

        public Shader(string name, string vertexSrc, string fragmentSrc)
        {
            m_Name = name;
            m_Locations = new Dictionary<string, int>();

            var sources = new Dictionary<uint, string>();
            sources.Add(GL_VERTEX_SHADER, vertexSrc);
            sources.Add(GL_FRAGMENT_SHADER, fragmentSrc);
            Compile(sources);
        }

        public void Compile(Dictionary<uint, string> shaderSources)
        {
            uint program = gl.CreateProgram();
            Debug.Assert(shaderSources.Count <= 2, "Level editor only supports up to 2 shaders.");
            uint[] glShaderIDs = new uint[2];
            int glShaderIDIndex = 0;
            foreach (KeyValuePair<uint, string> kv in shaderSources)
            {
                uint type = kv.Key;
                string source = kv.Value;

                uint shader = gl.CreateShader(type);
                gl.ShaderSource(shader, source);
                gl.CompileShader(shader);
                gl.AttachShader(program, shader);
                glShaderIDs[glShaderIDIndex++] = shader;
            }

            gl.LinkProgram(program);

            foreach (uint id in glShaderIDs)
            {
                gl.DetachShader(program, id);
                gl.DeleteShader(id);
            }

            m_RendererID = program;
        }

        public void Bind()
        {
            gl.UseProgram(m_RendererID);
        }

        public void Unbind()
        {
            gl.UseProgram(0);
        }

        public int GetUniformLocation(string name)
        {
            if (m_Locations.ContainsKey(name))
            {
                return m_Locations[name];
            }
            else
            {
                int location = gl.GetUniformLocation(m_RendererID, name);
                m_Locations.Add(name, location);
                return location;
            }
        }

        public void UploadUniformFloat(string name, float value)
        {
            gl.Uniform1(GetUniformLocation(name), value);
        }

        public void UploadUniformFloat2(string name, vec2 value)
        {
            gl.Uniform2(GetUniformLocation(name), value.x, value.y);
        }
        
        public void UploadUniformFloat3(string name, vec3 value)
        {
            gl.Uniform3(GetUniformLocation(name), value.x, value.y, value.z);
        }

        public void UploadUniformFloat4(string name, vec4 value)
        {
            gl.Uniform4(GetUniformLocation(name), value.x, value.y, value.z, value.w);
        }

        public void UploadUniformMat3(string name, mat3 value)
        {
            gl.UniformMatrix3(GetUniformLocation(name), 1, false, value.to_array());
        }

        public void UploadUniformMat4(string name, mat4 value)
        {
            gl.UniformMatrix4(GetUniformLocation(name), 1, false, value.to_array());
        }
    }
}
