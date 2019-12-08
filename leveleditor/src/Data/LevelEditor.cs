﻿using System;
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
        public static LevelEditor Instance { get; set; } = new LevelEditor();

        private OrthographicCamera m_Camera;
        private Status m_Status;
        private Level m_Level;
        private string m_LevelPath = "";
        private string m_LevelFileName = "";
        private bool m_Changed = false;

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
        public string LevelFileName
        {
            get => m_LevelFileName;
            set
            {
                m_LevelFileName = value;
                OnPropertyChanged("LevelFileName");
            }
        }
        public bool Changed
        {
            get => m_Changed;
            set
            {
                m_Changed = value;
                OnPropertyChanged("Changed");
            }
        }

        public LevelEditor()
        {
            m_Status = new Status();
            Level = new Level(new ECSState(), new LevelProperties { ResourcePath = "" });
            LevelFileName = "untitled.json";
            LevelPath = "";
            Changed = true;
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
                LevelFileName = Path.GetFileName(path);
                Changed = false;
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

        public void SaveTo(string path)
        {
            File.WriteAllText(path, Level.ToJSON());
            LevelFileName = Path.GetFileName(path);
            LevelPath = path;
            Changed = false;
            Status = new Status { Type = StatusType.Trace, Body = $"Saved as {LevelFileName}" };
        }

        public void Save()
        {
            File.WriteAllText(LevelPath, Level.ToJSON());
            Changed = false;
            Status = new Status { Type = StatusType.Trace, Body = $"Saved {LevelFileName}" };
        }
    }
}
