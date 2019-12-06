using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace leveleditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL object.
            SharpGL.OpenGL gl = glControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.6f, 0.6f, 0.6f, 1.0f);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            SharpGL.OpenGL gl = glControl.OpenGL;
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                StatusTextBlock.Text = "Opening " + ofd.FileName;
                dynamic levelData = JObject.Parse(File.ReadAllText(ofd.FileName));
                
                if (levelData.systemNames != null && levelData.entities != null && levelData.singletons != null)
                {
                    // TODO: save / process / show level data.
                }
                else
                {
                    StatusTextBlock.Text = "Error: Invalid level file.";
                    MessageBox.Show("Error: Invalid level file.", "Polar Level Editor");
                }
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Exiting...";
            Close();
        }
    }
}
