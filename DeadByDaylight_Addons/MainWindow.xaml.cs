﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace DeadByDaylight_Addons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string DEFAULT_DESCRIPTION = "Нет данных";
        private readonly int DEFAULT_HEADER_HEIGHT = 270;
        private readonly int DEFAULT_ADDONSLOT_HEIGHT = 80;
        private readonly int DEFAULT_PICSCOUNTER = 20;
        private readonly int DEFAULT_DESCRIPTION_LINES = 10;
        private readonly int DEFAULT_ADDON_HEIGHT = 70;
        private readonly int DEFAULT_STARS_WIDTH = 52;
        private readonly int DEFAULT_ADDONICON_WIDTH = 48;
        private readonly int DEFAULT_STARS_HEIGHT = 49;
        private readonly int DEFAULT_ADDONICON_HEIGHT = 48;
        private readonly int DEFAULT_MARGIN = 10;
        private readonly int DEFAULT_STARS_COUNT = 1;

        public MainWindow()
        {
            InitializeComponent();
            var iconUri = new Uri(Directory.GetCurrentDirectory(), UriKind.RelativeOrAbsolute);
            Left = SystemParameters.PrimaryScreenWidth - Width;
            var AllKillers = InitialKillers();
            KillerName.SelectionChanged += KillerName_SelectionChanged;
            InitialCmb(KillerName, AllKillers);
            KillerName.Focus();
        }

        private void KillerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = KillerName.SelectedItem as string;
            if (selectedItem != null)
            {
                RefreshInfo(selectedItem);
            }
        }

        private void RefreshInfo(string killerName)
        {
            //Инициализируем объект убийцы
            var killerInfo = new KillerInfo();
            var AllKillers = InitialKillers();///////////////////////////////////////////////////////////////////////////////New
            killerInfo = AllKillers.Find(x => x.KillerName == killerName);

            //Настройка высоты окна
            var picsCounter = killerInfo.AddonInfo.Count;
            if (picsCounter == 0)
            {
                picsCounter = DEFAULT_PICSCOUNTER;
            }
            var rowCounter = (picsCounter - 1) / 2 + 1;
            Application.Current.MainWindow.Height = DEFAULT_HEADER_HEIGHT + DEFAULT_ADDONSLOT_HEIGHT * rowCounter;
            MinHeight = DEFAULT_HEADER_HEIGHT + DEFAULT_ADDONSLOT_HEIGHT * rowCounter;

            InsertKillerIcon(killerInfo.KillerImagePath);
            
            FillDescription(killerInfo.KillerDescription);
            
            GridSetting(rowCounter);
            
            CreateMiniGrid(picsCounter, rowCounter, killerInfo.AddonInfo);
        }

        //Оформляем шапку
        private void InsertKillerIcon(string halfPath)
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), halfPath);
            if (File.Exists(path))
            {
                Icon.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            else
            {
                path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "addons", "icon.png");
                if (File.Exists(path))
                {
                    Icon.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                }
                else
                {
                    Icon.Source = null;
                }
            }
        }

        //Заполняем описание шапки
        private void FillDescription (string mainData)
        {
            Description_1.Text = null;
            Description_2.Text = null;
            string[] description = mainData.Split("\r\n");
            if (description.Length == 0)
            {
                Description_1.Text = DEFAULT_DESCRIPTION;
            }
            for (int count = 0; count < description.Length; count++)
            {
                if (count < DEFAULT_DESCRIPTION_LINES)
                {
                    Description_1.Text += description[count] + "\r\n";
                }
                if ((count >= DEFAULT_DESCRIPTION_LINES) && (count < 2 * DEFAULT_DESCRIPTION_LINES))
                {
                    Description_2.Text += description[count] + "\r\n";
                }
            }
        }

        //Настраиваем таблицу - вставляем строки
        private void GridSetting (int rowCounter)
        {
            AddonSlots.Children.Clear();
            AddonSlots.Height = DEFAULT_ADDONSLOT_HEIGHT * rowCounter;
            for (int count = 0; count < rowCounter; count++)
            {
                var rowActive = new RowDefinition();
                rowActive.Height = new GridLength(DEFAULT_ADDON_HEIGHT);
                AddonSlots.RowDefinitions.Add(rowActive);
                var rowPassive = new RowDefinition();
                rowPassive.Height = new GridLength(DEFAULT_ADDONSLOT_HEIGHT - DEFAULT_ADDON_HEIGHT);
                AddonSlots.RowDefinitions.Add(rowPassive);
            }
        }

        //Создаём подтаблицы
        private void CreateMiniGrid (int picsCounter, int rowCounter, List<AddonInfo> addonInfo)
        {
            for (int count = 0; count < picsCounter; count++)
            {
                var addonField = new Grid();

                var col1 = new ColumnDefinition();
                col1.Width = new GridLength(DEFAULT_STARS_WIDTH + DEFAULT_MARGIN);
                addonField.ColumnDefinitions.Add(col1);
                var col2 = new ColumnDefinition();
                col2.Width = new GridLength(DEFAULT_ADDONICON_WIDTH + DEFAULT_MARGIN);
                addonField.ColumnDefinitions.Add(col2);
                var col3 = new ColumnDefinition();
                addonField.ColumnDefinitions.Add(col3);
                var row = new RowDefinition();
                addonField.RowDefinitions.Add(row);

                if (count < rowCounter)
                {
                    Grid.SetRow(addonField, count * 2);
                    Grid.SetColumn(addonField, 1);
                }
                else
                {
                    Grid.SetRow(addonField, (count - rowCounter) * 2);
                    Grid.SetColumn(addonField, 3);
                }
                FillInfo(addonInfo[count], addonField);
                AddonSlots.Children.Add(addonField);
            }
        }

        //Вставляем картинку аддона
        private void InsertAddon(Image addon, string path)
        {
            if (File.Exists(path))
            {
                addon.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            else
            {
                path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "addons", "emptyAddon.png");
                if (File.Exists(path))
                {
                    addon.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                }
                else
                {
                    addon.Source = null;
                }
            }
        }

        //Вставляем картинку звёзд
        private void InsertStars(Image stars, string path)
        {
            if (File.Exists(path))
            {
                stars.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            else
            {
                stars.Source = null;
            }
        }

        //Вставляем описание аддона
        private void InsertDescription(TextBlock descript, string addonText)
        {
            if (string.IsNullOrEmpty(addonText))
            {
                descript.Text = DEFAULT_DESCRIPTION;
            }
            else
            {
                descript.Text = addonText;
            }
        }

        //Заполняем подтаблицу элементами
        private void FillInfo(AddonInfo Addon, Grid miniGrid)
        {
            var stars = Addon.Stars;
            if ((Addon.Stars < 1) || (Addon.Stars > 5))
            {
                stars = DEFAULT_STARS_COUNT;
            }
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "addons", "Stars", $"star{stars}.png");
            var starImage = new Image();
            starImage.Width = DEFAULT_STARS_WIDTH;
            starImage.Height = DEFAULT_STARS_HEIGHT;
            starImage.VerticalAlignment = VerticalAlignment.Top;
            starImage.HorizontalAlignment = HorizontalAlignment.Left;
            InsertStars(starImage, path);
            Grid.SetRow(starImage, 0);
            Grid.SetColumn(starImage, 0);

            var addonImage = new Image();
            addonImage.Width = DEFAULT_ADDONICON_WIDTH;
            addonImage.Height = DEFAULT_ADDONICON_HEIGHT;
            addonImage.VerticalAlignment = VerticalAlignment.Top;
            addonImage.HorizontalAlignment = HorizontalAlignment.Left;
            path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Addon.AddonImagePath);
            InsertAddon(addonImage, path);
            Grid.SetRow(addonImage, 0);
            Grid.SetColumn(addonImage, 1);

            var addonText = new TextBlock();
            InsertDescription(addonText, Addon.AddonDescription);
            addonText.TextWrapping = TextWrapping.WrapWithOverflow;
            Grid.SetRow(addonText, 0);
            Grid.SetColumn(addonText, 2);

            miniGrid.Children.Add(starImage);
            miniGrid.Children.Add(addonImage);
            miniGrid.Children.Add(addonText);
        }

        //Инициализация всех убийц
        private List<KillerInfo> InitialKillers ()
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "addons", "AllDescriptions.json");
            string text = null;
            if (File.Exists(path))
            {
                using (var sr = new StreamReader(path, Encoding.Default))
                {
                    text = sr.ReadToEnd();
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                return JsonConvert.DeserializeObject<List<KillerInfo>>(text);
            }
            return null;
        }

        //Инициализируем комбобокс
        private void InitialCmb(ComboBox cmb, List<KillerInfo> AllKillers)
        {
            foreach (KillerInfo obj in AllKillers)
            {
                var name = obj.KillerName;
                cmb.Items.Add(name);
            }

        }

        //Открывает окно заметок
        private void NotesButton_Click(object sender, RoutedEventArgs e)
        {
            var customMsgBox = new CustomMsgBox();
            customMsgBox.Show();
        }
    }

    public class AddonInfo
    {
        public int Stars;
        public string AddonImagePath;
        public string AddonDescription;
    }

    public class KillerInfo
    {
        public string KillerName;
        public string KillerImagePath;
        public string KillerDescription;
        public List<AddonInfo> AddonInfo;
    }
}
