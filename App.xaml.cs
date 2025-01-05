using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

namespace Dossier_Registratie
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-NL");
            base.OnStartup(e);


            if (!DataProvider.SetupComplete)
            {
                ShowSetupWindow();
            }
            else
            {
                ClearShutdownFile();
                FetchUsersCredentials();
            }
        }
        private static void FetchUsersCredentials()
        {
            try
            {
                var searchOperation = new SearchOperations();
                var (permissionLevelId, permissionLevelName) = searchOperation.FetchUserCredentials(Environment.UserName);

                Globals.PermissionLevelId = permissionLevelId;
                Globals.PermissionLevelName = permissionLevelName;
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex).ConfigureAwait(false);
                var dbError = MessageBox.Show("Database connectie error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (dbError == MessageBoxResult.OK)
                    Current.Shutdown();
            }
        }
        static void ClearShutdownFile()
        {
            if (!string.IsNullOrEmpty(DataProvider.ShutdownFile) && File.Exists(DataProvider.ShutdownFile))
                File.WriteAllText(DataProvider.ShutdownFile, string.Empty);
        }
        private static void ShowSetupWindow()
        {
            var setupViewModel = new GeneralSetupViewModel();

            var setupWindow = new Window
            {
                Title = "Installatie",
                Content = new Dossier_Registratie.Views.GeneralSetup { DataContext = setupViewModel },
                Width = 850,
                Height = 850,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            bool? result = setupWindow.ShowDialog();

            if (result == true)
            {
                ClearShutdownFile();
                FetchUsersCredentials();
            }
        }

    }
}
