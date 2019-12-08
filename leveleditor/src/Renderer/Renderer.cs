using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmNet;

namespace leveleditor
{
    static class Renderer
    {
        private static mat4 m_ViewProjectionMatrix;
        private static ShaderLibrary m_ShaderLibrary;

        private static VertexArray m_TextureQuadVA;

        public static void Init()
        {
            m_ShaderLibrary = new ShaderLibrary();
            m_ShaderLibrary.Add(new Shader("TextureShader", "Texture"));

            m_TextureQuadVA = new VertexArray();

            float[] quadVertices = new float[]{
                -0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
                 0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
                 0.5f, 0.5f, 0.0f, 1.0f, 0.0f,
                -0.5f, 0.5f, 0.0f, 0.0f, 0.0f
            };

            VertexBuffer quadVB = new VertexBuffer(quadVertices);

            ushort[] quadIndices = new ushort[] { 0, 1, 2, 0, 2, 3 };
            IndexBuffer quadIB = new IndexBuffer(quadIndices);
            m_TextureQuadVA.SetIndexBuffer(quadIB);

            BufferLayout quadLayout = new BufferLayout(new List<BufferElement>
            {
                new BufferElement(ShaderDataType.Float3, "a_Position"),
                new BufferElement(ShaderDataType.Float2, "a_TexCoord")
            });

            quadVB.Layout = quadLayout;
            m_TextureQuadVA.AddVertexBuffer(quadVB, m_ShaderLibrary.Get("TextureShader"));
        }

        public static void BeginScene()
        {

        }

        public static void EndScene()
        {

        }
    }
}
