using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static QCode.DataBase;
using System.Windows.Forms;

namespace QCode
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _lngs = { "Кириллица", "Латинский" };
        string filter = "Microsoft Word (*.doc)|*.doc|Microsoft Word (*.docx)|*.docx";
        string[] filters = { "Microsoft Word (*.doc)|*.doc", "Microsoft Word(*.docx)|*.docx" };
        private int _index;
        private SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "Текстовые документы(*.txt)|*.txt|Все файлы(*.*)|*.*" };
        private System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer();
#if SUPPORT
        private WordPlugin w = new WordPlugin();
#endif
        private OpenFileDialog openFileDlg = new OpenFileDialog();
        private SaveFileDialog saveFileDlg = new SaveFileDialog();

        public string TextOne { get { return editOne.Text; } set { editOne.Text = value; } }
        public string TextTwo { get { return editTwo.Text; } set { editTwo.Text = value; } }
        public MainWindow()
        {

            int[] partx = { 1, 2, 4, 5, 5, 16, 20, 0, 31, 200, 200, 200 };
            int[] part2x = { 1, 201, 202, 203, 204, 205, 206, 207, 208, 233, 244, 255 };

            partx = new int[39999789];


            int[] merge_sort_my_version2(int[] part, int[] part2)
            {
                //Содаем слияемой буффер
                int[] buffer = new int[part.Length + part2.Length];
                int n1 = 0, n2 = 0; // индексаторы для первого и второго массива
                int low = 0; // индексатор общего массива
                int high = buffer.Length;
                int val = 0;// Переменная для выполнения операций присваивания. 
                while (low != high)
                {
                    //Ежик, это мое рассуждение. С начала проверяю есть ли объекты у первого массива. 
                    if (n1 < part.Length)
                    {
                        // проверяю не пуст ли второй массив
                        if (n2 < part2.Length)
                        {
                            //Самое меньшее меньше [x1 < x2]
                            if (part[n1] < part2[n2])
                                val = part[n1++];
                            else // получить от part2 [x2]
                                val = part2[n2++];
                        }
                        else
                        {
                            val = part[n1++];
                        }
                    }
                    else // отсутствует у первого, значит собираем оставшиеся
                        val = part2[n2++];

                    buffer[low++] = val;
                }

                return buffer;
            }

            List<int> merge(List<int> first, List<int> second)
            {
                var ans = new List<int>(first.Count + second.Count);
                int fPointer = 0, sPointer = 0;
                while (fPointer < first.Count && sPointer < second.Count)
                {
                    while (fPointer < first.Count && first[fPointer] <= second[sPointer] && sPointer < second.Count)
                    {

                        ans.Add(first[fPointer]);
                        ++fPointer;
                    }
                    if (fPointer >= first.Count)
                    {
                        break;
                    }
                    while (sPointer < second.Count && second[sPointer] <= first[fPointer] && fPointer < first.Count)
                    {
                        ans.Add(second[sPointer]);
                        ++sPointer;
                    }
                }
                for (int i = fPointer; i < first.Count; ++i)
                {
                    ans.Add(first[i]);
                }
                for (int i = sPointer; i < second.Count; ++i)
                {
                    ans.Add(second[i]);
                }
                return ans;
            }

            TimeSpan resultA, resultB;

            List<int> x1 = new List<int>(partx);
            List<int> x2 = new List<int>(part2x);
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            long end = 0;
            while (-1==~0)
            {

                long start = DateTime.Now.Ticks;
                List<int> xx = merge(x1, x2);
                end = DateTime.Now.Ticks;
                resultA = TimeSpan.FromTicks(end - start);

                start = DateTime.Now.Ticks;
                int[] xxx = merge_sort_my_version2(partx, part2x);
                end = DateTime.Now.Ticks;
                resultB = TimeSpan.FromTicks(end - start);
            }
            InitializeComponent();
        }

        private void showFullScreen()
        {
            Window _fullScreen = new Window();
            _fullScreen.WindowState = WindowState.Maximized;
            //_fullScreen.Topmost = true;
            _fullScreen.WindowStyle = WindowStyle.None;
            _fullScreen.Cursor = null;
            _fullScreen.Title = "Logo Screen";
            SolidColorBrush solidColorBrush = new SolidColorBrush((Color)App.Current.Resources["__c_def_logo"]);
            _fullScreen.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            var _cnt = new Border() { Background = solidColorBrush };
            _fullScreen.Content = _cnt;
            _fullScreen.Show();
            DateTime _startTime = DateTime.Now;
            double waitSecond = 5;
            DispatcherTimer ds = new DispatcherTimer();
            ds.Interval = TimeSpan.FromMilliseconds(1);
            ds.Tick += (o, e) =>
            {
                if ((DateTime.Now - _startTime).TotalSeconds < waitSecond)
                {

                    int val = (int)255 - (int)(255 * (DateTime.Now - _startTime).TotalSeconds / waitSecond);
                    var c = solidColorBrush.Color;
                    c.A = (byte)val;
                    solidColorBrush.Color = c;
                }
                else
                {
                    ds.Stop();
                    _fullScreen.Close();
                }
            };

            ds.Start();
        }
        private void ChangeType(bool isLatin)
        {
            _index = !isLatin ? 0 : 1;

            string _one = "";
            string _two = "";
            if (_index > 1)
                _index = 0;
            if (_index == 0)
            {
                _one = _lngs[0];
                _two = _lngs[1];
            }
            else
            {
                _one = _lngs[1];
                _two = _lngs[0];
            }

            _lOne.Content = _one;
            _ltwo.Content = _two;
        }
        private void RefreshBox()
        {
            ChangeType(false);
            ToChange();
        }


        private void ToChange(int selType = -1)
        {

            void methodOne()
            {
                TextTwo = _index == 0 ? TranslateToLatin(TextOne) : TranslateToCyrillic(TextOne);
            }

            void methodTwo()
            {
                TextOne = _index == 0 ? TranslateToCyrillic(TextTwo) : TranslateToLatin(TextTwo);
            }

            void generalMethod()
            {
                _reverseTo.Focus();
                methodOne();
                methodTwo();
            }

            string generalString = selType == 0 ? TextOne : selType == 1 ? TextTwo : "";

            if (selType == 0)
            {
                methodOne();
            }
            else if (selType == 1)
            {
                methodTwo();
            }
            else
                generalMethod();

            int length = generalString.Length;
            ShowMaxSymbolError(length > 500);


        }
        private void _reverseTo_Click(object sender, RoutedEventArgs e)
        {
            ChangeType(_index != 1);
            ToChange();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.footerString.Content = string.Format("Ⓒ 2018-2019 Это программа защищена законом об авторских прав \"{0}\"", DataBase.LIBRARY_NAME);
            RefreshBox();
            var last = new WelcomeTo();
            last.Title = this.Title = LIBRARY_NAME;
            last.ShowDialog();
            soundPlayer.Stream = Properties.Resources.error_sound01;

        }

        private void _frameClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _frameHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SaveToContent(int textSelect)
        {
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;

                File.WriteAllText(fileName, textSelect == 0 ? TextOne : TextTwo, Encoding.UTF8);
            }
        }

        private void editOne_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!editOne.IsFocused)
                return;

            ToChange(0);
        }

        private void editTwo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!editTwo.IsFocused)
                return;

            ToChange(1);
        }


        private void _butSave01_Click(object sender, RoutedEventArgs e)
        {
            SaveToContent(0);
        }

        private void _butSave02_Click(object sender, RoutedEventArgs e)
        {
            SaveToContent(1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveTo(true);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }

        private void _ToCyrillic_Click(object sender, RoutedEventArgs e)
        {
            SaveTo(false);
        }


        private void ShowMaxSymbolError(bool showed)
        {
            this._info.Visibility = showed ? Visibility.Visible : Visibility.Hidden;
            if (showed)
                soundPlayer.Play();
        }

        void SaveTo(bool toLatin)
        {

            saveFileDlg.FileName = openFileDlg.FileName = string.Empty;
            openFileDlg.Filter = filter;

            bool operationState = false;
            int index = -1;
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                index = openFileDlg.FilterIndex - 1;

                saveFileDlg.Filter = filters[index];

                if (saveFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    operationState = true;
                }
            }



            if (!operationState)
            {
                System.Windows.MessageBox.Show("Операция отменена пользователем.");
            }
            else
            {
                if (!Plugin.isSupported)
                {
                    System.Windows.MessageBox.Show("В программе не поддерживается Word API Interface.");
                }
#if SUPPORT
                Microsoft.Office.Interop.Word.WdSaveFormat frm = index == 0 ? Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument : Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocumentDefault;
                this.w.SaveTo(openFileDlg.FileName, saveFileDlg.FileName, toLatin ? AlphabetType.Latin : AlphabetType.Cyrillic, frm);
#endif
            }
        }
    }
}