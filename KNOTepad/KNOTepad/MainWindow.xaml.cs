using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace KNOTepad
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Включение уведомлений об изменениях свойств
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Размеры и положение окна
        static double windowHeight;
        static double windowWidth;
        static double windowTop;
        static double windowLeft;
        #endregion

        #region IsDark
        private bool isDark = false;
        public bool IsDark
        {
            get { return isDark; }
            set { isDark = value; NotifyPropertyChanged(); }
        }
        #endregion

        private bool _currentisDark = false;

        private double _currentOpacity;

        #region RootOpacity
        private double rootOpacity = 0.95;

        public double RootOpacity
        {
            get { return rootOpacity; }
            set { rootOpacity = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region WinState
        private string wState = WindowState.Normal.ToString();

        public string WinState
        {
            get { return wState; }
            set { wState = value; NotifyPropertyChanged(); }
        }
        #endregion

        string lastFile = "";
        public string LastFile
        {
            get
            {
                return lastFile;
            }
            set
            {
                lastFile = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var prefsFile = ConfigurationManager.AppSettings["LastFile"];
            TextRange range;
            FileStream fStream;
            if (File.Exists(prefsFile))
            {
                range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                fStream = new FileStream(prefsFile, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                LastFile = prefsFile;
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openResultDialog = new OpenFileDialog();
            openResultDialog.DefaultExt = ".txt";
            openResultDialog.Filter = "Текстовые документы|*.txt|Документы rtf|*.rtf";
            Nullable<bool> result = openResultDialog.ShowDialog();
            TextRange range;
            FileStream fStream;
            if (result == true)
            {
                string fileName = openResultDialog.FileName;
                range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                fStream = new FileStream(fileName, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                LastFile = fileName;
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveResultDialog = new SaveFileDialog();
            saveResultDialog.DefaultExt = ".txt";
            saveResultDialog.Filter = "Текстовые документы|*.txt|Документы rtf|*.rtf";
            Nullable<bool> result = saveResultDialog.ShowDialog();
            if (result == true)
            {
                string fileName = saveResultDialog.FileName;
                TextRange t = new TextRange(RTB.Document.ContentStart,
                                            RTB.Document.ContentEnd);
                FileStream file = new FileStream(fileName, FileMode.Create);
                t.Save(file, DataFormats.Text);
                file.Close();
                LastFile = fileName;
            }
        }

        private void MWindow_Closing(object sender, CancelEventArgs e)
        {
            if (WinState == WindowState.Maximized.ToString())
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            if (WinState == WindowState.Normal.ToString())
                App.Current.MainWindow.WindowState = WindowState.Normal;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
                AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");
                appSettings.Settings.Clear();
                appSettings.Settings.Add("WindowTop", windowTop.ToString());
                appSettings.Settings.Add("WindowLeft", windowLeft.ToString());
                appSettings.Settings.Add("WindowHeight", windowHeight.ToString());
                appSettings.Settings.Add("WindowWidth", windowWidth.ToString());
                appSettings.Settings.Add("LastFile", lastFile);
                appSettings.Settings.Add("WindowState", WinState.ToString());
                appSettings.Settings.Add("IsDark", IsDark.ToString());
                appSettings.Settings.Add("Topmost", this.Topmost.ToString());
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
                SnackbarMessages.MessageQueue = SnackbarMessages.MessageQueue ?? new SnackbarMessageQueue();
                SnackbarMessages.MessageQueue.Enqueue("Maybe there is no spoon on closing");
            }

        }

        private void MWindow_LayoutUpdated(object sender, EventArgs e)
        {
            SaveWindowParamsMethod();
        }

        #region SaveWindowParams | Сохранение позиции и размеров окна
        private void SaveWindowParamsMethod()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                windowHeight = this.Height;
                windowWidth = this.Width;
                windowTop = this.Top;
                windowLeft = this.Left;
            }
        }
        #endregion

        private void MWindow_Initialized(object sender, EventArgs e)
        {
            try
            {
                this.Height = Int32.Parse(ConfigurationManager.AppSettings["WindowHeight"]);
                this.Width = Int32.Parse(ConfigurationManager.AppSettings["WindowWidth"]);
                this.Top = Int32.Parse(ConfigurationManager.AppSettings["WindowTop"]);
                this.Left = Int32.Parse(ConfigurationManager.AppSettings["WindowLeft"]);

                this.IsDark = bool.Parse(ConfigurationManager.AppSettings["isDark"]);
                new PaletteHelper().SetLightDark(this.IsDark);

                this.Topmost = bool.Parse(ConfigurationManager.AppSettings["Topmost"]);

                var wState = ConfigurationManager.AppSettings["WindowState"];
                if (wState == WindowState.Normal.ToString())
                    App.Current.MainWindow.WindowState = WindowState.Normal;
                else if (wState == WindowState.Maximized.ToString())
                    App.Current.MainWindow.WindowState = WindowState.Maximized;
                WinState = App.Current.MainWindow.WindowState.ToString();

                this.SaveWindowParamsMethod();
            }
            catch
            {
                SnackbarMessages.MessageQueue = SnackbarMessages.MessageQueue ?? new SnackbarMessageQueue();
                SnackbarMessages.MessageQueue.Enqueue("Maybe there is no spoon on load");
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            RTB.Document.Blocks.Clear();
            LastFile = "";
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            new PaletteHelper().SetLightDark(this.IsDark);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximazeButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.WindowState == WindowState.Normal)
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            else if (App.Current.MainWindow.WindowState == WindowState.Maximized)
                App.Current.MainWindow.WindowState = WindowState.Normal;
            WinState = App.Current.MainWindow.WindowState.ToString();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            _currentisDark = this.IsDark;
            _currentOpacity = this.Opacity;
            rootDialogHost.IsOpen = true;
        }

        private void SaveOptions_Click(object sender, RoutedEventArgs e)
        {
            rootDialogHost.IsOpen = false;
        }

        private void CancelOptions_Click(object sender, RoutedEventArgs e)
        {
            this.IsDark = _currentisDark;
            this.Opacity = _currentOpacity;
            new PaletteHelper().SetLightDark(this.IsDark);
            rootDialogHost.IsOpen = false;
        }
    }
}
