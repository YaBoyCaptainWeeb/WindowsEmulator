using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WindowsEmulator
{
    /// <summary>
    /// Логика взаимодействия для DisplayTile.xaml
    /// </summary>
    public partial class DisplayTile : UserControl
    {
        User user = new User();
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",typeof(string), typeof(DisplayTile), new PropertyMetadata(null));
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image",typeof(ImageSource), typeof(DisplayTile), new PropertyMetadata(null));
        public DisplayTile()
        {
            InitializeComponent();
            string user1 = File.ReadAllText(@"CurrentUser.txt");
            List<string> data = new List<string>();
            foreach(Match match in Regex.Matches(user1, @"\b\S+\b"))
            {
                data.Add(match.Value);
            }
            user._Journal = Convert.ToBoolean(data[5]);
            DataContext = this;
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // btn.Click += new RoutedEventHandler(OpenJournal);
        }

    }
}
