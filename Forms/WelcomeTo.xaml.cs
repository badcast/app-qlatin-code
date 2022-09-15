using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QCode
{
    /// <summary>
    /// Логика взаимодействия для WelcomeTo.xaml
    /// </summary>
    public partial class WelcomeTo : Window
    {
        public WelcomeTo()
        {
            InitializeComponent();
            _title.Content = "Добро пожаловать в программу " + DataBase.LIBRARY_NAME;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Content = null;
            this.Close();
        }
    }
}
