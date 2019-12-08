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
using System.Windows.Shapes;

using Ookii.Dialogs.Wpf;


namespace leveleditor
{
    /// <summary>
    /// Interaction logic for NewLevelWindow.xaml
    /// </summary>
    public partial class NewLevelWindow : Window
    {
        public NewLevelWindow()
        {
            InitializeComponent();
        }

        private void ButtonResourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();

            if (fbd.ShowDialog() == true)
            {
                ResourcePathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            LevelEditor.Instance.Level = new Level(new ECSState(), 
                new LevelProperties { ResourcePath = ResourcePathTextBox.Text });
            LevelEditor.Instance.LevelFileName = "untitled.json";
            LevelEditor.Instance.LevelPath = "";
            LevelEditor.Instance.Changed = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
