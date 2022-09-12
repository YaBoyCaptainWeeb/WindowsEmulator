using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal : Window 
    {
        string[] _Journal;
        ObservableCollection<JournalLine> _Journals = new ObservableCollection<JournalLine>();
        public class JournalLine
        {
            public string Date { get; set; }
            public string Time { get; set; }
            public string Action { get; set; }
            public string User { get; set; }
            public JournalLine(string date, string time, string action, string user)
            {
                this.Date = date;
                this.Time = time;
                this.Action = action;
                this.User = user;
            }
            public JournalLine(string date, string time, string action)
            {
                this.Date=date;
                this.Time=time;
                this.Action=action;
                this.User = "";
            }
        }
        public Journal()
        {
             InitializeComponent();
             DisplayJournal();
             GroupBy.ItemsSource = typeof(JournalLine).GetProperties().Select((o) => o.Name);         
        }   
        
        public void DisplayJournal()
        {
            _Journal = File.ReadAllLines(@"Journal.txt");
            if (_Journal.Length != 0 && _Journal[0][0] != ' ')
            {
                foreach (string line in _Journal)
                {
                    string[] Data = new string[4];
                    int i = 0;
                    int j = 0;
                    while (i != 2)
                    {
                        if (line[j] != ' ')
                        {
                            Data[i] += line[j];
                            j++;
                        }
                        else
                        {
                            j++;
                            i++;
                        }
                    }
                    while (j != line.Length)
                    {
                        if (line[j] == ':')
                        {
                            j += 2;
                            while (j != line.Length)
                            {
                                Data[3] += line[j];
                                j++;
                            }
                        }
                        else
                        {
                            Data[2] += line[j];
                            j++;
                        }
                    }
                    _Journals.Add(new JournalLine(Data[0], Data[1], Data[2], Data[3]));
                }
                JournalList.ItemsSource = _Journals;
            }
        }
        public void GroupList() // Переназначение группировки
        {
            JournalList.Items.GroupDescriptions.Clear();
            var property = GroupBy.SelectedItem as string;
            if (property == "Без группировки")
            {
                return;
            }
            JournalList.Items.GroupDescriptions.Add(new PropertyGroupDescription(property));
        }

        private void GroupByUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupList();
        }
    }
}
