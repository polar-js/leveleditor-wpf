using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static leveleditor.GL;
using static SharpGL.OpenGL;

namespace leveleditor
{
    enum ShaderDataType
    {
        None = 0, Float, Float2, Float3, Float4, Mat3, Mat4, Int, Int2, Int3, Int4, Bool, FloatArray
    }

    class BufferElement
    {
        public string Name { get; set; }
        public ShaderDataType Type { get; set; }
        public uint Size { get; set; }
        public uint Offset { get; set; }
        public bool Normalized { get; set; }

        public BufferElement(ShaderDataType type, string name, bool normalized = false)
        {
            Type = type;
            Name = name;
            Size = ShaderDataTypeSize(type);
            Offset = 0;
            Normalized = normalized;
        }

        public static uint ShaderDataTypeSize(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.Float: return 4;
                case ShaderDataType.Float2: return 4 * 2;
                case ShaderDataType.Float3: return 4 * 3;
                case ShaderDataType.Float4: return 4 * 4;
                case ShaderDataType.Mat3: return 4 * 3 * 3;
                case ShaderDataType.Mat4: return 4 * 4 * 4;
                case ShaderDataType.Int: return 4;
                case ShaderDataType.Int2: return 4 * 2;
                case ShaderDataType.Int3: return 4 * 3;
                case ShaderDataType.Int4: return 4 * 4;
                case ShaderDataType.Bool: return 1;
                case ShaderDataType.FloatArray: throw new ArgumentException("Type cannot be FloatArray", "type");
                default: throw new ArgumentException("Unknown ShaderDataType", "type");
            }
        }

        public static uint ShaderDataTypeToOpenGLBaseType(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.Float:  return GL_FLOAT;
                case ShaderDataType.Float2: return GL_FLOAT;
                case ShaderDataType.Float3: return GL_FLOAT;
                case ShaderDataType.Float4: return GL_FLOAT;
                case ShaderDataType.Mat3:   return GL_FLOAT;
                case ShaderDataType.Mat4:   return GL_FLOAT;
                case ShaderDataType.Int:    return GL_INT;
                case ShaderDataType.Int2:   return GL_INT;
                case ShaderDataType.Int3:   return GL_INT;
                case ShaderDataType.Int4:   return GL_INT;
                case ShaderDataType.Bool:   return GL_BOOL;
                default: throw new Exception("Unknown ShaderDataType");
            }
        }

        public uint getComponentCount()
        {
            switch (Type)
            {
                case ShaderDataType.Float: return 1;
                case ShaderDataType.Float2: return 2;
                case ShaderDataType.Float3: return 3;
                case ShaderDataType.Float4: return 4;
                case ShaderDataType.Mat3: return 3 * 3;
                case ShaderDataType.Mat4: return 4 * 4;
                case ShaderDataType.Int: return 1;
                case ShaderDataType.Int2: return 2;
                case ShaderDataType.Int3: return 3;
                case ShaderDataType.Int4: return 4;
                case ShaderDataType.Bool: return 1;
                default: throw new Exception("Invalid type");
            }
        }
    }

    class BufferLayout : IEnumerable<BufferElement>
    {
        public List<BufferElement> Elements { get; }
        public uint Stride { get; } = 0;

        public BufferLayout(List<BufferElement> elements)
        {
            Elements = elements;

            Stride = 0;
            foreach (BufferElement element in Elements)
            {
                element.Offset = Stride;
                Stride += element.Size;
            }
        }

        public IEnumerator<BufferElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class VertexBuffer
    {
        private uint m_RendererID;
        public BufferLayout Layout { get; set; }

        public VertexBuffer(float[] vertices)
        {
            uint[] buffers = new uint[1];
            gl.GenBuffers(1, buffers);
            m_RendererID = buffers[0];
            gl.BindBuffer(GL_ARRAY_BUFFER, m_RendererID);
            gl.BufferData(GL_ARRAY_BUFFER, vertices, GL_STATIC_DRAW);
        }

        ~VertexBuffer()
        {
            gl.DeleteBuffers(1, new uint[] { m_RendererID });
        }

        public void Bind()
        {
            gl.BindBuffer(GL_ARRAY_BUFFER, m_RendererID);
        }

        public void Unbind()
        {
            gl.BindBuffer(GL_ARRAY_BUFFER, 0);
        }
    }

    class IndexBuffer
    {
        private uint m_RendererID;
        public uint Count { get; private set; }

        public IndexBuffer(ushort[] vertices)
        {
            Count = (uint)vertices.Length;

            uint[] buffers = new uint[1];
            gl.GenBuffers(1, buffers);
            m_RendererID = buffers[0];
            gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_RendererID);
            gl.BufferData(GL_ELEMENT_ARRAY_BUFFER, vertices, GL_STATIC_DRAW);
        }

        ~IndexBuffer()
        {
            gl.DeleteBuffers(1, new uint[] { m_RendererID });
        }

        public void Bind()
        {
            gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_RendererID);
        }

        public void Unbind()
        {
            gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
        }
    }
}
