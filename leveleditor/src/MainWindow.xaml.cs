using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static leveleditor.GL;

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
            LevelEditor.Instance = new LevelEditor();
            LevelEditor.Instance.PropertyChanged += LevelEditor_PropertyChanged;
            Commands.Init();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuEditProperties.IsEnabled = LevelEditor.Instance.Level != null;
            if (LevelEditor.Instance.Changed)
                Title = $"{LevelEditor.Instance.LevelFileName}* - Polar Level Editor";
            else
                Title = $"{LevelEditor.Instance.LevelFileName} - Polar Level Editor";
        }

        private void LevelEditor_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "Status":
                    switch (LevelEditor.Instance.Status.Type)
                    {
                        case StatusType.Trace:
                            StatusTextBlock.Text = LevelEditor.Instance.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            break;
                        case StatusType.Info:
                            StatusTextBlock.Text = LevelEditor.Instance.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
                            break;
                        case StatusType.Warning:
                            StatusTextBlock.Text = "Warning: " + LevelEditor.Instance.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 0));
                            break;
                        case StatusType.Error:
                            StatusTextBlock.Text = "Error: " + LevelEditor.Instance.Status.Body;
                            StatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(240, 0, 0));
                            MessageBox.Show(StatusTextBlock.Text, "Polar Level Editor");
                            break;
                    }
                    break;
                case "Level":
                    MenuEditProperties.IsEnabled = LevelEditor.Instance.Level != null;
                    break;
                case "LevelFileName":
                    if (LevelEditor.Instance.Changed)
                        Title = $"{LevelEditor.Instance.LevelFileName}* - Polar Level Editor";
                    else
                        Title = $"{LevelEditor.Instance.LevelFileName} - Polar Level Editor";
                    break;
                case "Changed":
                    if (LevelEditor.Instance.Changed)
                        Title = $"{LevelEditor.Instance.LevelFileName}* - Polar Level Editor";
                    else
                        Title = $"{LevelEditor.Instance.LevelFileName} - Polar Level Editor";
                    break;
            }
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            gl = glControl.OpenGL;

            LevelEditor.Instance.RenderInit();
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            LevelEditor.Instance.Render();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                LevelEditor.Instance.MenuFileOpen(ofd.FileName);
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Exiting...";
            Close();
        }

        private void PropertiesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (LevelEditor.Instance.Level != null)
            {
                PropertiesWindow propertiesWindow = new PropertiesWindow();
                propertiesWindow.ShowDialog();
            }
        }

        private void MenuFileNewLevel_Click(object sender, RoutedEventArgs e)
        {
            NewLevelWindow newLevelWindow = new NewLevelWindow();
            newLevelWindow.ShowDialog();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (LevelEditor.Instance.LevelPath == "")
            {
                SaveAs();
            }
            else
            {
                LevelEditor.Instance.Save();
            }
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "JSON Level File(*.json)|*.json|All(*.*)|*",
                Title = "Save Level As",
                FileName = LevelEditor.Instance.LevelFileName
            };

            if (dialog.ShowDialog() == true)
            {
                LevelEditor.Instance.SaveTo(dialog.FileName);
            }
        }
    }
}
