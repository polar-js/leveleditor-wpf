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

using Microsoft.Win32;
using System.ComponentModel;

using static leveleditor.GL;

namespace leveleditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LevelEditor m_LevelEditor;

        public MainWindow()
        {
            InitializeComponent();
            m_LevelEditor = new LevelEditor();
            m_LevelEditor.PropertyChanged += LevelEditor_PropertyChanged;

        }

        private void LevelEditor_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Status")
            {
                switch (m_LevelEditor.Status.Type)
                {
                    case StatusType.Trace:
                        StatusTextBlock.Text = m_LevelEditor.Status.Body;
                        StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        break;
                    case StatusType.Info:
                        StatusTextBlock.Text = m_LevelEditor.Status.Body;
                        StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
                        break;
                    case StatusType.Warning:
                        StatusTextBlock.Text = "Warning: " + m_LevelEditor.Status.Body;
                        StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 0));
                        break;
                    case StatusType.Error:
                        StatusTextBlock.Text = "Error: " + m_LevelEditor.Status.Body;
                        StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(240, 0, 0));
                        MessageBox.Show(StatusTextBlock.Text, "Polar Level Editor");
                        break;
                }
                
            }
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            gl = glControl.OpenGL;

            m_LevelEditor.RenderInit();
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            m_LevelEditor.Render();
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                m_LevelEditor.MenuFileOpen(ofd.FileName);
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Exiting...";
            Close();
        }
    }
}
