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
                var file = new FileInfo(e.Args[0]);
                if (file.Exists)
                {
                    try
                    {
                        double.TryParse(ConfigurationManager.AppSettings["WindowHeight"], out var windowHeight);
                        double.TryParse(ConfigurationManager.AppSettings["WindowWidth"], out var windowWidth);
                        double.TryParse(ConfigurationManager.AppSettings["WindowTop"], out var windowTop);
                        double.TryParse(ConfigurationManager.AppSettings["WindowLeft"], out var windowLeft);

                        var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
                        var appSettings = (AppSettingsSection)config.GetSection("appSettings");
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
