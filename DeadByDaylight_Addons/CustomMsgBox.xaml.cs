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
using System.Configuration;

namespace DeadByDaylight_Addons
{
    /// <summary>
    /// Логика взаимодействия для CustomMsgBox.xaml
    /// </summary>
    public partial class CustomMsgBox : Window
    {
        private readonly int DEFAULT_RIGHT_MARGIN = 100;
        private readonly int DEFAULT_FONT_SIZE = 12;
        private readonly int DEFAULT_MARGIN = 60;
        private readonly int DEFAULT_BETWEEN_LINES = 5;

        public CustomMsgBox()
        {
            InitializeComponent();
            Left = SystemParameters.PrimaryScreenWidth - Width - DEFAULT_RIGHT_MARGIN;
            AddText();
            var bc = new BrushConverter();
            var manualBckgr = (Brush)bc.ConvertFrom(AppSettingsManager.GetBackgroundColor());
            var manualFrgr = (Brush)bc.ConvertFrom(AppSettingsManager.GetTextColor());
            Background = manualBckgr;
            Foreground = manualFrgr;
            OK.Background = manualBckgr;
            OK.Foreground = manualFrgr;
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
                text = "Нет данных." + Environment.NewLine+ "Проверьте в новом патче.";
            }
            notesText.Text = text;
            notesText.FontSize = DEFAULT_FONT_SIZE;
            MinHeight = text.Split(Environment.NewLine).Length * (DEFAULT_FONT_SIZE + DEFAULT_BETWEEN_LINES) + DEFAULT_MARGIN;
            Height = MinHeight;
        }

        //Закрываем окно
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
