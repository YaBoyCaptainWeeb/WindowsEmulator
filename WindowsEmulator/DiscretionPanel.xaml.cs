using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для DiscretionPanel.xaml
    /// </summary>
    public partial class DiscretionPanel : Window
    {        
        public DiscretionPanel(User _currentUser, Desktop desktop)
        {
            InitializeComponent();
            BuildTable();
            
        }

        private void BuildTable()
        {
            string[] folders = File.ReadAllLines(@"Folders.txt");
            foreach (string folder in folders)
            {
                string[] folderData = folder.Split('_');
                for (int i = 0; i != folderData.Length; i++)
                {
                    MessageBox.Show(folderData[i]);
                }

            }
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {

        }
    }
}
