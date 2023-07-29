using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace DeadByDaylight_Addons
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private readonly string DEFAULT_DESCRIPTION = "Нет данных";
        private readonly int DEFAULT_HEADER_HEIGHT = 332;
        private readonly int DEFAULT_ADDONSLOT_HEIGHT = 70;
        private readonly int DEFAULT_PICSCOUNTER = 20;
        private readonly int DEFAULT_DESCRIPTION_LINES = 14;
        private readonly int DEFAULT_ADDON_HEIGHT = 70;
        private readonly int DEFAULT_STARS_WIDTH = 52;
        private readonly int DEFAULT_ADDONICON_WIDTH = 48;
        private readonly int DEFAULT_STARS_HEIGHT = 49;
        private readonly int DEFAULT_ADDONICON_HEIGHT = 48;
        private readonly int DEFAULT_MARGIN = 10;
        private readonly int DEFAULT_STARS_COUNT = 1;
        private readonly string PATCH_MACROS = "%PATCH%";

        private List<KillerInfo> _allKillers;

        public MainWindow()
        {
            InitializeComponent();
            var iconUri = new Uri(Directory.GetCurrentDirectory(), UriKind.RelativeOrAbsolute);
            Left = SystemParameters.PrimaryScreenWidth - Width;
            MaxHeight = SystemParameters.PrimaryScreenHeight + 10;
            MaxWidth = SystemParameters.PrimaryScreenWidth + 15;
            _allKillers = InitialKillers();
            KillerName.SelectionChanged += KillerName_SelectionChanged;
            SortAddons.SelectionChanged += KillerName_SelectionChanged;
            DbD_Addons.SizeChanged += Window_SizeChanged;
            InitialKillerCmb(KillerName, _allKillers);
            InitialSortCmb(SortAddons);
            KillerName.Focus();

            Title = Title.Replace(PATCH_MACROS, AppSettingsManager.GetPatchNumber());
            Height = DEFAULT_HEADER_HEIGHT + DEFAULT_ADDONSLOT_HEIGHT * DEFAULT_PICSCOUNTER / 2;

            var bc = new BrushConverter();
            var manualBckgr = (Brush)bc.ConvertFrom(AppSettingsManager.GetBackgroundColor());
            var manualFrgr = (Brush)bc.ConvertFrom(AppSettingsManager.GetTextColor());
            Background = manualBckgr;
            Foreground = manualFrgr;
            NotesLabel.Foreground = manualFrgr;
            Sorting.Foreground = manualFrgr;
            NotesButton.Background = manualBckgr;
            KillerName.Foreground = manualFrgr;
            SortAddons.Foreground = manualFrgr;
        }

        private void KillerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _selectedKiller = KillerName.SelectedItem as string;
            var _selectedSort = SortAddons.SelectedItem as CustomComboboxItem;
            if ((_selectedKiller != null) && (_selectedSort != null))
            {
                RefreshInfo(_selectedKiller, _selectedSort.Order);
            }
        }

        //Изменение ширины окна
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Width < MainGrid.MinWidth)
            {
                mainScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
            else
            {
                mainScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            if (Width != MainGrid.Width)
            {
                MainGrid.Width = Width - 10;
            }
        }

        private void RefreshInfo(string killerName, KindSortEnum sortion)
        {
            //Инициализируем объект убийцы
            var killerInfo = _allKillers.Find(x => x.KillerName == killerName);

            //Настройка высоты окна
            var picsCounter = killerInfo.AddonInfo.Count;
            if (picsCounter == 0)
            {
                picsCounter = DEFAULT_PICSCOUNTER;
            }
            var rowCounter = (picsCounter - 1) / 2 + 1;

            InsertKillerIcon(killerInfo.KillerImagePath);

            FillDescription(killerInfo.KillerDescription);

            GridSetting(rowCounter);

            CreateMiniGrid(picsCounter, rowCounter, killerInfo.AddonInfo, sortion);
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
        private void FillDescription(string mainData)
        {
            Description_1.Text = null;
            Description_2.Text = null;
            string[] description = mainData.Split(Environment.NewLine);
            if (description.Length == 0)
            {
                Description_1.Text = DEFAULT_DESCRIPTION;
            }
            for (int count = 0; count < description.Length; count++)
            {
                if (count < DEFAULT_DESCRIPTION_LINES)
                {
                    Description_1.Text += description[count] + Environment.NewLine;
                }
                if ((count >= DEFAULT_DESCRIPTION_LINES) && (count < 2 * DEFAULT_DESCRIPTION_LINES))
                {
                    Description_2.Text += description[count] + Environment.NewLine;
                }
            }
        }

        //Настраиваем таблицу - вставляем строки
        private void GridSetting(int rowCounter)
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
        private void CreateMiniGrid(int picsCounter, int rowCounter, List<AddonInfo> addonInfo, KindSortEnum sortion)
        {
            if (sortion == KindSortEnum.Standart)
            {
                addonInfo = addonInfo.OrderBy(x => x.Order).ToList();
            }
            else if (sortion == KindSortEnum.Rate)
            {
                addonInfo = addonInfo.OrderByDescending(x => x.Stars).ToList();
            }

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

                if (sortion == KindSortEnum.Standart)
                {
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
                }
                else if (sortion == KindSortEnum.Rate)
                {
                    if (count % 2 == 0)
                    {
                        Grid.SetRow(addonField, count);
                        Grid.SetColumn(addonField, 1);
                    }
                    else
                    {
                        Grid.SetRow(addonField, count - (count % 2));
                        Grid.SetColumn(addonField, 3);
                    }
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
            if ((Addon.Stars < 0) || (Addon.Stars > 5))
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
            if (!String.IsNullOrEmpty(Addon.Tips))
            {
                var tip = new ToolTip();
                var bc = new BrushConverter();
                tip.Content = Addon.Tips;
                tip.Background = (Brush)bc.ConvertFrom(AppSettingsManager.GetBackgroundColor());
                tip.Foreground = (Brush)bc.ConvertFrom(AppSettingsManager.GetTextColor());
                ToolTipService.SetShowDuration(starImage, 30000);
                starImage.ToolTip = tip;
            }

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
        private List<KillerInfo> InitialKillers()
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

        //Инициализируем комбобокс убийц
        private void InitialKillerCmb(ComboBox cmb, List<KillerInfo> AllKillers)
        {
            foreach (KillerInfo obj in AllKillers)
            {
                var name = obj.KillerName;
                cmb.Items.Add(name);
            }

        }

        //Инициализируем комбобокс сортировки
        private void InitialSortCmb(ComboBox cmb)
        {
            foreach (KindSortEnum value in Enum.GetValues(typeof(KindSortEnum)))
            {
                var item = new CustomComboboxItem()
                {
                    Order = value
                };
                cmb.Items.Add(item);
            };
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
        public string Tips;
        public int Order => Convert.ToInt32(AddonImagePath.Split("\\").LastOrDefault().Split(".").FirstOrDefault());
    }

    public class KillerInfo
    {
        public string KillerName;
        public string KillerImagePath;
        public string KillerDescription;
        public List<AddonInfo> AddonInfo;
    }
}
