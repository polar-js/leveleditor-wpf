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

namespace leveleditor
{
    /// <summary>
    /// Interaction logic for AddSystemWindow.xaml
    /// </summary>
    public partial class AddSystemWindow : Window
    {
        public AddSystemWindow()
        {
            InitializeComponent();
        }

        private void SystemNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddSystemButton.IsEnabled = SystemNameInput.Text.Trim() != "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        private void SystemNameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Submit();
            }
        }

        private void Submit()
        {
            if (LevelEditor.Instance.AddSystem(SystemNameInput.Text))
            {
                Close();
            }
            else
            {
                MessageBox.Show("Error: System already included!", "Polar Level Editor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
