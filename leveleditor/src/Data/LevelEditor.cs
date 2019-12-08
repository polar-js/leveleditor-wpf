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
        public static LevelEditor Editor { get; set; } = new LevelEditor();

        private Status m_Status;
        private Level m_Level;
        private OrthographicCamera m_Camera;
        private string m_LevelPath = "";

        public event PropertyChangedEventHandler PropertyChanged;
        public Status Status
        {
            get => m_Status;
            set
            {
                m_Status = value;
                OnPropertyChanged("Status");
            }
        }
        public Level Level
        {
            get => m_Level;
            set
            {
                m_Level = value;
                OnPropertyChanged("Level");
            }
        }
        public string LevelPath
        {
            get => m_LevelPath;
            set
            {
                m_LevelPath = value;
                OnPropertyChanged("LevelPath");
            }
        }


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
                Level = level;
                LevelPath = path;
                Status = new Status { Type = StatusType.Info, Body = "Loaded " + path };

                if (Level.Properties.ResourcePath == "")
                {
                    MessageBox.Show("Warning: The loaded project contains no resource directory." +
                        "\nTo set resource directory, go to Edit > Properties", "Polar Level Editor");
                }
            }
            else
            {
                Status = new Status { Type = StatusType.Error, Body = "Invalid level file " + path };
            }
        }
    }
}
