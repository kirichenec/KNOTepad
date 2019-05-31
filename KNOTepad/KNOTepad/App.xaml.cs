using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;

namespace KNOTepad
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                FileInfo file = new FileInfo(e.Args[0]);
                if (file.Exists) //make sure it's actually a file
                {
                    double windowHeight = 0;
                    double windowWidth = 0;
                    double windowTop = 0;
                    double windowLeft = 0;

                    try
                    {
                        windowHeight = Int32.Parse(ConfigurationManager.AppSettings["WindowHeight"]);
                        windowWidth = Int32.Parse(ConfigurationManager.AppSettings["WindowWidth"]);
                        windowTop = Int32.Parse(ConfigurationManager.AppSettings["WindowTop"]);
                        windowLeft = Int32.Parse(ConfigurationManager.AppSettings["WindowLeft"]);
                    }
                    catch
                    {
                        // default values
                    }

                    try
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
                        AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");
                        appSettings.Settings.Clear();
                        appSettings.Settings.Add("WindowTop", windowTop.ToString());
                        appSettings.Settings.Add("WindowLeft", windowLeft.ToString());
                        appSettings.Settings.Add("WindowHeight", windowHeight.ToString());
                        appSettings.Settings.Add("WindowWidth", windowWidth.ToString());
                        appSettings.Settings.Add("LastFile", file.FullName);
                        config.Save();
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                    catch
                    {
                        // default values
                    }
                }
            }
        }
    }
}
