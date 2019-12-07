using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static leveleditor.GL;
using static SharpGL.OpenGL;

using System.Diagnostics;

namespace leveleditor
{
    class VertexArray
    {
        private uint m_RendererID;
        public List<VertexBuffer> VertexBuffers { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }

        public VertexArray()
        {
            uint[] ids = new uint[1];
            gl.GenVertexArrays(1, ids);
            m_RendererID = ids[0];

            VertexBuffers = new List<VertexBuffer>();
        }

        public void Bind()
        {
            gl.BindVertexArray(m_RendererID);
        }

        public void Unbind()
        {
            gl.BindVertexArray(0);
        }

        public void AddVertexBuffer(VertexBuffer vertexBuffer, Shader shader)
        {
            Debug.Assert(vertexBuffer.Layout.Elements.Count > 0, "Vertex buffer has no layout!");

            gl.BindVertexArray(m_RendererID);
            vertexBuffer.Bind();

            uint index = 0;
            foreach (BufferElement element in vertexBuffer.Layout)
            {
                gl.EnableVertexAttribArray(index);
                gl.VertexAttribPointer(index,
                    (int)element.getComponentCount(),
                    BufferElement.ShaderDataTypeToOpenGLBaseType(element.Type),
                    element.Normalized,
                    (int)vertexBuffer.Layout.Stride,
                    (IntPtr)element.Offset);
                index++;
            }
            VertexBuffers.Add(vertexBuffer);
        }

        public void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            gl.BindVertexArray(m_RendererID);
            indexBuffer.Bind();

            IndexBuffer = indexBuffer;
        }
    }
}
