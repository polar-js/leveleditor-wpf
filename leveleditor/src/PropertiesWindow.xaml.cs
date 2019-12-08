using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Win32;
using System.IO;

using Ookii.Dialogs.Wpf;

namespace leveleditor
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        private LevelProperties m_ChangedProperties;
        private bool m_Changed = false;

        private bool Changed
        {
            get => m_Changed;
            set
            {
                ApplyButton.IsEnabled = value;
                Title = value ? "Properties*" : "Properties";
                m_Changed = value;
            }
        }

        public PropertiesWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_ChangedProperties = LevelEditor.Editor.Level.Properties;
            ResourcePathTextBox.Text = LevelEditor.Editor.Level.Properties.ResourcePath;
            Changed = false;
        }

        private void ButtonResourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            
            if (fbd.ShowDialog() == true)
            {
                if (fbd.SelectedPath != m_ChangedProperties.ResourcePath)
                {
                    ResourcePathTextBox.Text = fbd.SelectedPath;
                    m_ChangedProperties.ResourcePath = fbd.SelectedPath;
                    Changed = true;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_ChangedProperties.ResourcePath = ResourcePathTextBox.Text;
            Changed = true;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Changed)
            {
                Apply();
            }
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        private void Apply()
        {
            LevelEditor.Editor.Level.Properties = m_ChangedProperties;
            Changed = false;
            LevelEditor.Editor.Status = new Status { Type = StatusType.Trace, Body = "Applied changes to properties." };
        }
    }
}
