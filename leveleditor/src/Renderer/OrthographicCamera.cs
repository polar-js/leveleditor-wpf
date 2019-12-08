using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmNet;

namespace leveleditor
{
    class OrthographicCamera
    {
        public mat4 ProjectionMatrix { get; private set; }
        public mat4 ViewMatrix { get; private set; }
        public mat4 ViewProjectionMatrix { get; private set; }

        private vec3 m_Position;
        private float m_Rotation;

        public OrthographicCamera(float left, float right, float bottom, float top, vec3 position = new vec3(), float rotation = 0.0f)
        {
            ViewMatrix = new mat4(1.0f);
            ProjectionMatrix = glm.ortho(left, right, bottom, top, -1.0f, 1.0f);
            ViewProjectionMatrix = new mat4(1.0f);

            m_Position = position;
            m_Rotation = rotation;

            RecalculateViewMatrix();
        }

        private void RecalculateViewMatrix()
        {
            mat4 transform = glm.translate(new mat4(1.0f), m_Position);
            transform = glm.rotate(transform, m_Rotation, new vec3(0.0f, 0.0f, 1.0f));
            ViewMatrix = glm.inverse(transform);
            ViewProjectionMatrix = ProjectionMatrix * ViewMatrix;
        }

        public void SetProjection(float left, float right, float bottom, float top)
        {
            ProjectionMatrix = glm.ortho(left, right, bottom, top, -1.0f, 1.0f);
            ViewProjectionMatrix = ProjectionMatrix * ViewMatrix;
        }

        public vec3 Position
        {
            get => m_Position;
            set
            {
                m_Position = value;
                RecalculateViewMatrix();
            }
        }

        public float Rotation
        {
            get => m_Rotation;
            set
            {
                m_Rotation = value;
                RecalculateViewMatrix();
            }
        }
    }
}
