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
using System.ComponentModel;

using static leveleditor.GL;
using Path = System.IO.Path;

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
            LevelEditor.Editor = new LevelEditor();
            LevelEditor.Editor.PropertyChanged += LevelEditor_PropertyChanged;
        }

        private void LevelEditor_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "Status":
                    switch (LevelEditor.Editor.Status.Type)
                    {
                        case StatusType.Trace:
                            StatusTextBlock.Text = LevelEditor.Editor.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            break;
                        case StatusType.Info:
                            StatusTextBlock.Text = LevelEditor.Editor.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
                            break;
                        case StatusType.Warning:
                            StatusTextBlock.Text = "Warning: " + LevelEditor.Editor.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 0));
                            break;
                        case StatusType.Error:
                            StatusTextBlock.Text = "Error: " + LevelEditor.Editor.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(240, 0, 0));
                            MessageBox.Show(StatusTextBlock.Text, "Polar Level Editor");
                            break;
                    }
                    break;
                case "Level":
                    MenuEditProperties.IsEnabled = LevelEditor.Editor.Level != null;
                    break;
                case "LevelPath":
                    Title = $"{Path.GetFileName(LevelEditor.Editor.LevelPath)} - Polar Level Editor";
                    break;
            }
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            gl = glControl.OpenGL;

            LevelEditor.Editor.RenderInit();
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            LevelEditor.Editor.Render();
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                LevelEditor.Editor.MenuFileOpen(ofd.FileName);
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Exiting...";
            Close();
        }

        private void MenuEditProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertiesWindow propertiesWindow = new PropertiesWindow();
            propertiesWindow.ShowDialog();
        }
    }
}
