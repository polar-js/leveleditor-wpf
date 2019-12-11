using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Color = System.Windows.Media.Color;

using SharpGL;
using SharpGL.SceneGraph.Shaders;
using SharpGL.SceneGraph;

namespace leveleditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private float m_Zoom = 1.0f;

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
            UpdateFields();
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
                            MessageBox.Show(StatusTextBlock.Text, "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                    break;
                case "Level":
                    MenuEditProperties.IsEnabled = LevelEditor.Instance.Level != null;
                    UpdateFields();
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

        private void UpdateFields()
        {
            UpdateSystemList();
            UpdateSingletonTreeView();
        }

        private void UpdateSystemList()
        {
            SystemListView.Items.Clear();
            foreach (string systemName in LevelEditor.Instance.Level.State.SystemNames)
            {
                ContextMenu menu = new ContextMenu();

                MenuItem copy = new MenuItem() { Header = "Copy", InputGestureText = "Ctrl+C" };
                copy.Command = Commands.CopyCommand;

                MenuItem delete = new MenuItem() { Header = "Delete" };
                delete.Command = Commands.DeleteCommand;

                menu.Items.Add(copy);
                menu.Items.Add(delete);

                ListViewItem item = new ListViewItem()
                {
                    Content = systemName,
                    ContextMenu = menu

                };
                SystemListView.Items.Add(item);
            }
        }

        private void UpdateSingletonTreeView()
        {
            SingletonTreeView.Items.Clear();
            foreach (dynamic component in LevelEditor.Instance.Level.State.Singletons.Components)
            {
                if (component.type == null)
                    continue;

                TreeViewItem typeItem = new TreeViewItem();
                typeItem.Header = component.type;

                foreach(PropertyDescriptor prop in TypeDescriptor.GetProperties(component))
                {
                    TreeViewItem propItem = new TreeViewItem();

                    Label propLabel = new Label() { Content = prop.Name };
                    DockPanel.SetDock(propLabel, Dock.Left);

                    TextBox propData = new TextBox() { Text = prop.GetValue(component) };

                    Button setButton = new Button() { Content = "Set" };
                    DockPanel.SetDock(setButton, Dock.Right);
                    
                    DockPanel panel = new DockPanel();
                    panel.Children.Add(propLabel);
                    panel.Children.Add(setButton);
                    panel.Children.Add(propData);

                    propItem.Header = panel;
                    typeItem.Items.Add(propItem);
                }

                SingletonTreeView.Items.Add(typeItem);
            }
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

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SystemListView.SelectedItem != null)
            {
                if ((SystemListView.SelectedItem as ListViewItem).IsFocused)
                {
                    Clipboard.SetText((SystemListView.SelectedItem as ListViewItem).Content.ToString());
                    LevelEditor.Instance.Status = new Status() { Type = StatusType.Trace, Body = "Copied to clipboard. " };
                    return;
                }
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SystemListView.SelectedItem != null)
            {
                var item = SystemListView.SelectedItem as ListViewItem;
                if (item.IsFocused)
                {
                    if (MessageBox.Show($"Are you sure you want to delete system '{item.Content.ToString()}'?", "Polar Level Editor", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (LevelEditor.Instance.RemoveSystem(item.Content.ToString()))
                        {
                            SystemListView.Items.Remove(item);
                            LevelEditor.Instance.Status = new Status() { Type = StatusType.Trace, Body = "Removed system." };
                        }
                        else
                        {
                            LevelEditor.Instance.Status = new Status() { Type = StatusType.Error, Body = "Could not find system " + item.Content.ToString() };
                            MessageBox.Show("Could not find system:\n" + item.Content.ToString(), "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    
                    return;
                }
            }
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

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        ShaderProgram program = new ShaderProgram();

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {

        }

        private void AddSystemButton_Click(object sender, RoutedEventArgs e)
        {
            AddSystemWindow window = new AddSystemWindow();
            window.Closed += AddSystemWindow_Close;
            window.ShowDialog();
        }

        private void AddSystemWindow_Close(object sender, EventArgs e)
        {
            UpdateSystemList();
        }

        private void SystemListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {


            }
        }
    }
}
