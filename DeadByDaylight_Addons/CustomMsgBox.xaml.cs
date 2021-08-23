using System;
using System.Collections.Generic;
using System.IO;
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

namespace DeadByDaylight_Addons
{
    /// <summary>
    /// Логика взаимодействия для CustomMsgBox.xaml
    /// </summary>
    public partial class CustomMsgBox : Window
    {
        private readonly int DEFAULT_RIGHT_MARGIN = 100;

        public CustomMsgBox()
        {
            InitializeComponent();
            Left = SystemParameters.PrimaryScreenWidth - Width - DEFAULT_RIGHT_MARGIN;
            AddText();
            OK.Focus();
        }

        //Добавляем текст
        private void AddText ()
        {
            string text = null;
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "addons", "Notes.txt");
            if (File.Exists(path))
            {
                using (var sr = new StreamReader(path, Encoding.Default))
                {
                    text = sr.ReadToEnd();
                }
            }
            if (string.IsNullOrEmpty(text))
            {
                text = "Нет данных.\r\nПроверьте в новом патче.";
            }
            notesText.Text = text;
        }

        //Закрываем окно
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
