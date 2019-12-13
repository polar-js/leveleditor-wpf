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
using System.Runtime.Remoting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        #region System List
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
        #endregion System List

        #region Singleton Tree View
        private void UpdateSingletonTreeView()
        {
            SingletonTreeView.Items.Clear();
            foreach (dynamic component in LevelEditor.Instance.Level.State.Singletons.Components)
            {
                if (component.type == null)
                    continue;

                TreeViewItem typeItem = new TreeViewItem();
                typeItem.Header = new TextBlock() { Text = component.type, Margin = new Thickness(5, 5, 5, 5) };
                JObject jcomponent = component as JObject;

                List<JProperty> properties = jcomponent.Properties().ToList();

                foreach (JProperty property in properties)
                {
                    typeItem.Items.Add(GenerateComponentItem(property));
                }

                SingletonTreeView.Items.Add(typeItem);
            }
        }

        private TreeViewItem GenerateComponentItem(JProperty property)
        {
            TreeViewItem componentTreeViewItem = new TreeViewItem() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            if (property.Value.Type == JTokenType.String)
            {
                componentTreeViewItem.Header = CreatePropertyDockPanel<string>(property);
            }
            else if (property.Value.Type == JTokenType.Integer)
            {
                componentTreeViewItem.Header = CreatePropertyDockPanel<int>(property);
            }
            else if (property.Value.Type == JTokenType.Float)
            {
                componentTreeViewItem.Header = CreatePropertyDockPanel<float>(property);
            }
            else if (property.Value.Type == JTokenType.Array)
            {
                componentTreeViewItem.Header = new TextBlock() { Text = property.Name, Margin = new Thickness(5, 5, 5, 5) };
                PopulateArrayItems(property.Value, componentTreeViewItem);
            }
            else
            {
                Console.WriteLine($"{property.Name} is {property.Value.Type.ToString()}!");
            }
            return componentTreeViewItem;
        }

        private void PopulateArrayItems(JToken token, TreeViewItem parent)
        {
            if (token.Type == JTokenType.Array)
            {
                foreach (JToken value in token.Children())
                {
                    TreeViewItem treeViewItem = new TreeViewItem() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                    if (value.Type == JTokenType.Array)
                    {
                        treeViewItem.Header = new TextBlock() { Text = "<Array>", Margin = new Thickness(5, 5, 5, 5) };
                        PopulateArrayItems(value, treeViewItem);
                    }
                    else if (value.Type == JTokenType.String)
                    {
                        treeViewItem.Header = CreateTokenDockPanel<string>(value);
                    }
                    else if (value.Type == JTokenType.Integer)
                    {
                        treeViewItem.Header = CreateTokenDockPanel<int>(value);
                    }
                    else if (value.Type == JTokenType.Float)
                    {
                        treeViewItem.Header = CreateTokenDockPanel<float>(value);
                    }
                    parent.Items.Add(treeViewItem);
                }

                // Create 'Add Item' button for array.
                TreeViewItem buttonItem = new TreeViewItem() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
                Button button = new Button() { Content = "Add Item", Margin = new Thickness(5, 5, 5, 5), Padding = new Thickness(3, 3, 3, 3) };
                buttonItem.Header = button;
                parent.Items.Add(buttonItem);
            }
        }
        private DockPanel CreatePropertyDockPanel<T>(JProperty property)
        {
            DockPanel panel = new DockPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            
            TextBlock nameTextBlock = new TextBlock() { Text = property.Name, VerticalAlignment = VerticalAlignment.Center };
            DockPanel.SetDock(nameTextBlock, Dock.Left);
            panel.Children.Add(nameTextBlock);

            if (typeof(T) == typeof(bool))
            {
                CheckBox checkbox = new CheckBox() { Content = property.Name, IsChecked = property.Value.Value<bool>() };
                checkbox.Checked += SingletonProperty_Changed;
                checkbox.Unchecked += SingletonProperty_Changed;
                panel.Children.Add(checkbox);
            }
            else
            {
                TextBox textbox = new TextBox() { Text = property.Value.Value<T>().ToString() ?? "TYPE ERROR", Margin = new Thickness(5, 5, 5, 5), Padding = new Thickness(3, 3, 3, 3), VerticalContentAlignment = VerticalAlignment.Center, MinWidth = 50.0 };
                textbox.TextChanged += SingletonProperty_Changed;
                panel.Children.Add(textbox);
            }

            TextBlock typeTextBlock = new TextBlock() { Text = $"<{property.Value.Type.ToString()}>", VerticalAlignment = VerticalAlignment.Center };
            DockPanel.SetDock(typeTextBlock, Dock.Right);
            panel.Children.Add(typeTextBlock);

            return panel;
        }

        private DockPanel CreateTokenDockPanel<T>(JToken value)
        {
            DockPanel panel = new DockPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            if (typeof(T) == typeof(bool))
            {
                CheckBox checkbox = new CheckBox() { IsChecked = value.Value<bool>() };
                checkbox.Checked += SingletonProperty_Changed;
                checkbox.Unchecked += SingletonProperty_Changed;
                panel.Children.Add(checkbox);
            }
            else
            {
                TextBox textbox = new TextBox() { Text = value.Value<T>().ToString() ?? "TYPE ERROR", Margin = new Thickness(5, 5, 5, 5), Padding = new Thickness(3, 3, 3, 3), VerticalContentAlignment = VerticalAlignment.Center, MinWidth = 50.0 };
                textbox.TextChanged += SingletonProperty_Changed;
                panel.Children.Add(textbox);
            }

            TextBlock typeTextBlock = new TextBlock() { Text = $"<{value.Type.ToString()}>", VerticalAlignment = VerticalAlignment.Center };
            DockPanel.SetDock(typeTextBlock, Dock.Right);
            panel.Children.Add(typeTextBlock);

            return panel;
        }

        private void SingletonProperty_Changed(object sender, EventArgs e)
        {
            SingletonApplyButton.IsEnabled = true;
        }

        private void SingletonApplyButton_Click(object sender, RoutedEventArgs e)
        {
            List<JObject> components = new List<JObject>();
            foreach (TreeViewItem componentTVI in SingletonTreeView.Items)
            {
                JObject component = new JObject();
                foreach (TreeViewItem propertyTVI in componentTVI.Items)
                {
                    if (propertyTVI.Header == null) continue;

                    if (propertyTVI.Header as DockPanel != null)
                    {
                        DockPanel panel = propertyTVI.Header as DockPanel;
                        var textblocks = panel.Children.OfType<TextBlock>();
                        if (textblocks.Count() == 2 && panel.Children.OfType<TextBox>().First() != null)
                        {
                            string name = textblocks.First().Text;
                            string data = panel.Children.OfType<TextBox>().First().Text;
                            string type = textblocks.Last().Text.Trim(new char[] { '<', '>' });

                            if (type == JTokenType.String.ToString())
                            {
                                component.Add(name, new JValue(data));
                            }
                            else if (type == JTokenType.Integer.ToString())
                            {
                                if (int.TryParse(data, out int result))
                                    component.Add(name, new JValue(result));
                                else
                                {
                                    MessageBox.Show($"Not an integer:\n{name}: {data}", "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                            else if (type == JTokenType.Float.ToString())
                            {
                                if (float.TryParse(data, out float result))
                                    component.Add(name, new JValue(result));
                                else
                                {
                                    MessageBox.Show($"Not a float:\n{name}: {data}", "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                        }
                        else if (textblocks.Count() == 1 && panel.Children.OfType<CheckBox>().First() != null)
                        {
                            string name = textblocks.First().Text;
                            bool isChecked = panel.Children.OfType<CheckBox>().First().IsChecked ?? false;
                            component.Add(name, new JValue(isChecked));
                        }
                    }
                    else if (propertyTVI.Header as TextBlock != null)
                    {
                        string name = (propertyTVI.Header as TextBlock).Text.Trim(new char[] { '<', '>' });

                        JToken token = GetTokens(propertyTVI);
                        if (token != null)
                            component.Add(name, token);
                    }
                }
                components.Add(component);
            }

            LevelEditor.Instance.Level.State.Singletons.Components.Clear();
            foreach (var component in components)
            {
                LevelEditor.Instance.Level.State.Singletons.Components.Add(component);
            }
            SingletonApplyButton.IsEnabled = false;
            LevelEditor.Instance.Changed = true;
        }

        private JToken GetTokens(TreeViewItem tvi)
        {
            if (tvi.Header as DockPanel != null)
            {
                DockPanel panel = tvi.Header as DockPanel;
                var textblocks = panel.Children.OfType<TextBlock>();
                if (textblocks.Count() == 1 && panel.Children.OfType<TextBox>().First() != null)
                {
                    string data = panel.Children.OfType<TextBox>().First().Text;
                    string type = textblocks.First().Text.Trim(new char[] { '<', '>' });

                    if (type == JTokenType.String.ToString())
                    {
                       return new JValue(data);
                    }
                    else if (type == JTokenType.Integer.ToString())
                    {
                        if (int.TryParse(data, out int result))
                            return new JValue(result);
                        else
                            MessageBox.Show($"Not an integer:\n{data}", "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (type == JTokenType.Float.ToString())
                    {
                        if (float.TryParse(data, out float result))
                            return new JValue(result);
                        else
                            MessageBox.Show($"Not a float:\n{data}", "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (textblocks.Count() == 1 && panel.Children.OfType<CheckBox>().First() != null)
                {
                    bool isChecked = panel.Children.OfType<CheckBox>().First().IsChecked ?? false;
                    return new JValue(isChecked);
                }
            }
            else if (tvi.Header as TextBlock != null)
            {
                JArray array = new JArray();
                foreach (TreeViewItem subItem in tvi.Items)
                {
                    if (subItem.Header != null)
                    {
                        var token = GetTokens(subItem);
                        if (token != null)
                        {
                            array.Add(token);
                        }
                    }
                }
                return array;
            }
            return null;
        }
        #endregion Singleton Tree View

        #region Commands
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

        #endregion commands

        #region OpenGL
        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        ShaderProgram program = new ShaderProgram();

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {

        }
        #endregion OpenGL

        

        
    }
}
