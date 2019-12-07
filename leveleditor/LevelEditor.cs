using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.IO;


using static leveleditor.GL;
using static SharpGL.OpenGL;
using System.Windows;

namespace leveleditor
{
    public class LevelEditor : INotifyPropertyChanged
    {
        private Status m_Status;
        private Level m_Level;

        public event PropertyChangedEventHandler PropertyChanged;

        public LevelEditor()
        {
            m_Status = new Status();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void RenderInit()
        {
            //  Set the clear color.
            gl.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
        }

        public void Render()
        {
            gl.Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }

        public void MenuFileOpen(string path)
        {
            Status = new Status { Type = StatusType.Info, Body = "Opening " + path };

            Level level = Level.FromJSON(File.ReadAllText(path));
            if (level != null)
            {
                m_Level = level;
                Status = new Status { Type = StatusType.Info, Body = "Loaded " + path };
            }
            else
            {
                Status = new Status { Type = StatusType.Error, Body = "Invalid level file " + path };
            }
        }

        public Status Status
        {
            get => m_Status;
            set
            {
                m_Status = value;
                OnPropertyChanged("Status");
            }
        }
    }
}
