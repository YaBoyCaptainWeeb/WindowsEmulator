using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для Desktop.xaml
    /// </summary>
    public partial class Desktop : Window
    {
        User _currentUser;
        public ObservableCollection<User> _users { get; set; }
        DispatcherTimer dispatcher;
        Regex rgx = new Regex(@"\b[A-Za-zА-Яа-я0-9,]+|\b");

        public Desktop(User CurrentUser, ObservableCollection<User> users)
        {
            InitializeComponent();
            InitializeFolders();
            this.DataContext = this;
            dispatcher = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dispatcher.Tick += SystemTimer;
            dispatcher.Start();

            _currentUser = CurrentUser;
            _users = users;
            
            CurrentUserTitle.Text = "Пользователь: " + _currentUser._username + "\tУровень доступа: " + _currentUser._level;

        }
        private void SystemTimer(object sender, EventArgs e)
        {
            SysTime.Text = DateTime.Now.ToString("T") + "\n" + DateTime.Now.ToString("d");
        }
        public void InitializeFolders()
        {
            // Журнал безопасности
            DisplayTile journal = new DisplayTile();
            journal.Text = "Журнал безопасности";
            journal.Image = new BitmapImage(new Uri(@"Assets/Journal.png", UriKind.Relative));
            journal.MouseDoubleClick += new MouseButtonEventHandler(OpenJournal);
            FolderArea.Children.Add(journal);
            // Админ панель
            DisplayTile journalAdmin = new DisplayTile();
            journalAdmin.Text = "Панель администратора";
            journalAdmin.Image = new BitmapImage(new Uri(@"Assets/AdminPanel.png", UriKind.Relative));
            journalAdmin.MouseDoubleClick += new MouseButtonEventHandler(OpenAdminPanel);
            FolderArea.Children.Add(journalAdmin);
            Canvas.SetLeft(journalAdmin, 125);
            // Дискреционная модель
            DisplayTile discretionAdmin = new DisplayTile();
            discretionAdmin.Text = "Дискреционная панель";
            discretionAdmin.Image = new BitmapImage(new Uri(@"Assets/AdminPanel.png", UriKind.Relative));
            discretionAdmin.MouseDoubleClick += new MouseButtonEventHandler(OpenDiscretionPanel);
            FolderArea.Children.Add(discretionAdmin);
            Canvas.SetLeft(discretionAdmin, 250);
            // Папки пользователей
            try
            {
                string[] FolderList = File.ReadAllLines(@"Folders.txt");
                if (FolderList.Length != 0 && FolderList[0][0] != ' ')
                {
                    foreach (string folder in FolderList)
                    {
                        string[] FolderData = folder.Split('|');
                        string[] access = folder.Split('_');
                        string names = "";
                        DisplayTile newFolder = new DisplayTile();
                        newFolder.ContextMenu = new ContextMenu();
                        MenuItem item = new MenuItem();
                        item.Header = "Удалить";
                        item.Click += new RoutedEventHandler(DeleteFolder);
                        newFolder.ContextMenu.Items.Add(item);
                        MenuItem item1 = new MenuItem();
                        item1.Header = "Редактировать";
                        item1.Click += new RoutedEventHandler(EditFolder);
                        newFolder.ContextMenu.Items.Add(item1);
                        newFolder.Text = FolderData[0];
                        newFolder.Tag = FolderData[1];

                        for (int i = 1; i != access.Length; i++)
                        {
                            names += access[i] + "_";
                        }
                        names = names.Remove(names.Length - 1, 1);
                        newFolder.Name = names;

                        newFolder.Image = new BitmapImage(new Uri(@"Assets/Folder.png", UriKind.Relative));
                        newFolder.MouseDoubleClick += new MouseButtonEventHandler(OpenFolder);
                        FolderArea.Children.Add(newFolder);
                        Canvas.SetLeft(newFolder, (Convert.ToDouble(FolderData[2])));
                        Canvas.SetTop(newFolder, (Convert.ToDouble(FolderData[3])));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Список папок не обнаружен.");
                Process.GetCurrentProcess().Kill();
            }
        }

        private void DeleteFolder(object sender, RoutedEventArgs e)
        {
            var obj = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DisplayTile;
                if ((_currentUser._level >= Convert.ToInt32(obj.Tag)) || _currentUser._username == "Admin")
                {
                    FolderArea.Children.Remove(obj);
                    Directory.Delete(@"UserFolder\" + obj.Text);
                    string[] FoldersList = File.ReadAllLines(@"Folders.txt");
                    File.Delete(@"Folders.txt");
                    foreach (string folder in FoldersList)
                    {
                        string[] folderData = folder.Split('|');
                        if (folderData[0] != obj.Text)
                        {
                            File.AppendAllText(@"Folders.txt", folder + "\n");
                        }
                    }
                }
                else if (_currentUser._level >= Convert.ToInt32(obj.Tag))
                {
                    FolderArea.Children.Remove(obj);
                    Directory.Delete(@"UserFolder\" + obj.Text);
                    string[] FoldersList = File.ReadAllLines(@"Folders.txt");
                    File.Delete(@"Folders.txt");
                    foreach (string folder in FoldersList)
                    {
                        string[] folderData = folder.Split('|');
                        if (folderData[0] != obj.Text)
                        {
                            File.AppendAllText(@"Folders.txt", folder + "\n");
                        }
                        else
                        {
                            File.Create(@"Folders.txt");
                        }
                    }
                } else 
                     {
                MessageBox.Show("Вы не можете удалить эту папку", "Недостаточно прав");
                     }
        }

        string folderName = "";
        private void EditFolder(object sender, RoutedEventArgs e)
        {
            var obj = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DisplayTile;
                if ((_currentUser._level >= Convert.ToInt32(obj.Tag)) || _currentUser._username == "Admin")
                {
                    folderName = (((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DisplayTile).Text;
                    Point DropPos = (((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DisplayTile).PointToScreen(new Point(0, 0));
                    EditGrid.Visibility = Visibility.Visible;
                    Canvas.SetTop(EditGrid, DropPos.Y);
                    Canvas.SetLeft(EditGrid, DropPos.X);
                }
                else if (_currentUser._level >= Convert.ToInt32(obj.Tag))
                {
                    folderName = (((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DisplayTile).Text;
                    Point DropPos = (((sender as MenuItem).Parent as ContextMenu).PlacementTarget as DisplayTile).PointToScreen(new Point(0, 0));
                    EditGrid.Visibility = Visibility.Visible;
                    Canvas.SetTop(EditGrid, DropPos.Y);
                    Canvas.SetLeft(EditGrid, DropPos.X);
                }
            else 
            {
                MessageBox.Show("Вы не можете редактировать данную папку", "Недостаточно прав");
            }
        }

        private void EditFolder1(object sender, RoutedEventArgs e)
        {
            if (EditLevel.SelectedIndex + 1 <= _currentUser._level)
            {
                foreach (var child in FolderArea.Children)
                {
                    string type = child.GetType().ToString();
                    if (type == "WindowsEmulator.DisplayTile")
                    {
                        if ((child as DisplayTile).Text == folderName && EditFolderName.Text != " ")
                        {
                            string tag = (child as DisplayTile).Tag.ToString();
                            Directory.Delete(@"UserFolder\" + (child as DisplayTile).Text);

                            string[] folders = File.ReadAllLines(@"Folders.txt");
                            File.Delete(@"Folders.txt");

                            foreach (string folder in folders)
                            {
                                string[] folderData = folder.Split('|');
                                if ((child as DisplayTile).Text != folderData[0])
                                {
                                    File.AppendAllText(@"Folders.txt", folder + "\n");
                                }
                            }

                            (child as DisplayTile).Text = EditFolderName.Text;
                            (child as DisplayTile).Tag = EditLevel.SelectedIndex + 1;

                            Point pos = (child as DisplayTile).PointToScreen(new Point(0, 0));
                            Directory.CreateDirectory(@"UserFolder\" + EditFolderName.Text);
                            File.AppendAllText(@"Folders.txt", (child as DisplayTile).Text + "|" + (child as DisplayTile).Tag.ToString() + "|" + (pos.X - 45) + "|" + (pos.Y - 60) + "|_" + (child as DisplayTile).Name + "\n");

                            EditGrid.Visibility = Visibility.Hidden;
                            EditFolderName.Text = "";
                            EditLevel.SelectedIndex = 0;
                        }
                    }
                }
            } else
            {
                MessageBox.Show("Вы указали доступ к папке выше, чем ваш", "Ошибка");
            }
        }

        private void CreateFolder(object sender, MouseButtonEventArgs e)
        { 
            try
            {
                File.ReadAllText(@"Folders.txt");
            }
            catch
            {
                MessageBox.Show("Файла для хранения значений нет.");
                return;
            }
            Point DropPos = e.GetPosition(FolderArea);
            CreationGrid.Visibility = Visibility.Visible;
            Canvas.SetTop(CreationGrid, DropPos.Y - 80);
            Canvas.SetLeft(CreationGrid, DropPos.X - 90);
        }
     
        private void CreateFolder(object sender, RoutedEventArgs e)
        {
            if (Level.SelectedIndex + 1 <= _currentUser._level)
            {
                DisplayTile newFolder = new DisplayTile();

                newFolder.ContextMenu = new ContextMenu();
                MenuItem item = new MenuItem();
                item.Header = "Удалить";
                item.Click += new RoutedEventHandler(DeleteFolder);
                newFolder.ContextMenu.Items.Add(item);

                MenuItem item1 = new MenuItem();
                item1.Header = "Редактировать";
                item1.Click += new RoutedEventHandler(EditFolder);
                newFolder.ContextMenu.Items.Add(item1);

                if (!File.Exists(@"Folders.txt"))
                {
                    File.Create(@"Folders.txt");
                }

                Point DropPos = btn.PointToScreen(new Point(0, 0));
                if (FolderName.Text.Length != 0 && FolderName.Text != " ")
                {
                    newFolder.Image = new BitmapImage(new Uri(@"Assets/Folder.png", UriKind.Relative));
                    newFolder.MouseDoubleClick += new MouseButtonEventHandler(OpenFolder);
                    newFolder.Text = FolderName.Text;
                    newFolder.Name = _currentUser._username;
                    newFolder.Tag = Level.SelectedIndex + 1;

                    FolderArea.Children.Add(newFolder);
                    Canvas.SetTop(newFolder, DropPos.Y - 60);
                    Canvas.SetLeft(newFolder, DropPos.X - 45);

                    if (!Directory.Exists(@"UserFolder\" + FolderName.Text))
                    {
                        Directory.CreateDirectory(@"UserFolder\" + FolderName.Text);
                        File.AppendAllText(@"Folders.txt", FolderName.Text + "|"  + newFolder.Tag.ToString() + "|"
                            + (DropPos.X - 45).ToString() + "|" + (DropPos.Y - 60).ToString() + "|_" + newFolder.Name + "\n");
                        CreationGrid.Visibility = Visibility.Hidden;
                        FolderName.Text = "";
                        Level.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("Папка с таким именем уже существует");
                        FolderArea.Children.Remove(newFolder);
                        CreationGrid.Visibility = Visibility.Hidden;
                        FolderName.Text = "";
                        Level.SelectedIndex = 0;
                        return;
                    }
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Дайте папке имя, либо отмените создание", "Ошибка");
                    FolderArea.Children.Remove(newFolder);
                    CreationGrid.Visibility = Visibility.Hidden;
                    FolderName.Text = "";
                    Level.SelectedIndex = 0;
                    return;
                }
            } else
            {
                MessageBox.Show("Вы указали доступ к папке выше, чем ваш", "Ошибка");
            }
        }

        public void OpenFolder(object sender, RoutedEventArgs e)
        {
            string open = ((DisplayTile)sender).Text;
            string tag = ((DisplayTile)sender).Tag as string;
            string name = ((DisplayTile)sender).Name;
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = ".\\UserFolder\\" + open;
            string names = name;
            string[] name1 = names.Split('_');
            foreach (string d in name1)
            {
                if (((_currentUser._level >= Convert.ToInt32(tag)) || d == _currentUser._username) || _currentUser._username == "Admin")
                {
                    Process.Start(info);
                }
                else if (_currentUser._level >= Convert.ToInt32(tag) || d == _currentUser._username)
                {
                    Process.Start(info);
                }
            }
        }
        private void OpenDiscretionPanel(object sender, RoutedEventArgs e)
        {
            if (_currentUser._level >= 3)
            {
                var task = new DiscretionPanel(_currentUser, this);
                if (task.IsActive == false)
                {
                    task.ShowDialog();
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString("g") + " Вход в дискреционную панель: " + _currentUser._username + "\n");
                }
            }
            else
            {
                MessageBox.Show("У вас нет доступа к дискреционной панели.", "Ошибка", MessageBoxButton.OK);
                File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Попытка доступа к дискреционной панели: " + _currentUser._username + "\n");
            }
        }

        private void OpenAdminPanel(object sender, RoutedEventArgs e)
        {
            if (_currentUser._level >= 2)
            {
                var task = new AdminPanel(_currentUser);
                if (task.IsActive == false)
                {
                    task.ShowDialog();
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Вход в панель администратора: " + _currentUser._username + "\n");
                }
            }
            else
            {
                MessageBox.Show("У вас нет доступа к панели Администратора.", "Ошибка", MessageBoxButton.OK);
                File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Попытка доступа к панели администратора: " + _currentUser._username + "\n");
            }
        }

        private void OpenJournal(object sender, RoutedEventArgs e)
        {
            if (_currentUser._level >= 2) 
            {
                var task = new Journal();
                if (task.IsActive == false)
                {
                    task.Show();
                    File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Вход в журнал безопасности: " + _currentUser._username + "\n");
                }
            }
            else
            {
                MessageBox.Show("У вас нет доступа к журналу.", "Ошибка", MessageBoxButton.OK);
                File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Попытка доступа к журналу безопасности: " + _currentUser._username + "\n");
            }
        }

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
        private void ShutDown(object sender, RoutedEventArgs e)
        {
            File.AppendAllText(@"Journal.txt", DateTime.Now.ToString(@"g") + " Завершение работы системы" + "\n");
            Application.Current.Shutdown();
        }

        private void FolderArea_Drop(object sender, DragEventArgs e)
        {
            var obj = e.Data.GetData(typeof(DisplayTile)) as DisplayTile;
            Point DropPos = e.GetPosition(FolderArea);
            Canvas.SetLeft(obj, DropPos.X - 45);
            Canvas.SetTop(obj, DropPos.Y - 60);
            string[] FolderList = File.ReadAllLines(@"Folders.txt");
            if (FolderList.Length != 0 && FolderList[0][0] != ' ' && obj.Text != "Журнал безопасности" && obj.Text != "Панель администратора" && obj.Text != "Дискреционная панель")
                {
                File.Delete(@"Folders.txt");
                foreach (string folder in FolderList)
                    {
                    string[] FolderData = folder.Split('|');
                    if (FolderData[0] != obj.Text)
                    {
                        File.AppendAllText(@"Folders.txt", FolderData[0] + "|" + FolderData[1] + "|" + FolderData[2] + "|" + FolderData[3] + "|" + FolderData[4] + "\n");
                    } else
                    {
                        File.AppendAllText(@"Folders.txt", FolderData[0] + "|" + FolderData[1] + "|" + (DropPos.X - 45).ToString() + "|" + (DropPos.Y - 60).ToString() + "|" + FolderData[4] + "\n");
                    }
                    }
                }
        }

        private void FolderArea_DragOver(object sender, DragEventArgs e)
        {
            var obj = e.Data.GetData(typeof(DisplayTile)) as DisplayTile;
            Point DropPos = e.GetPosition(FolderArea);
            Canvas.SetLeft(obj, DropPos.X - 45);
            Canvas.SetTop(obj, DropPos.Y - 60);
        }
    }
}
