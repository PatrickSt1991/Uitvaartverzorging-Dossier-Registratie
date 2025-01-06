using Dossier_Registratie.Helper;
using Dossier_Registratie.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Dossier_Registratie.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _pollingTimer;
        private string ShutdownFilePath = DataProvider.ShutdownFile;
        private static bool isNotificationViewOpen = false;
        public ViewModelBase()
        {
            _pollingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _pollingTimer.Tick += PollingTimer_Tick;
            _pollingTimer.Start();
        }
        private void PollingTimer_Tick(object sender, EventArgs e)
        {
            CheckForShutdownCommand();
        }
        private void CheckForShutdownCommand()
        {
            try
            {
                if (File.Exists(ShutdownFilePath))
                {
                    using (var fileStream = new FileStream(ShutdownFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        string command = reader.ReadToEnd();
                        if (command.Trim().Equals("SHUTDOWN", StringComparison.OrdinalIgnoreCase))
                        {
                            ForceCloseApplication();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking shutdown command: {ex.Message}");
            }
        }
        private static async void ForceCloseApplication()
        {
            if (isNotificationViewOpen)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                isNotificationViewOpen = true;

                ShutdownNotificationView notificationView = new ShutdownNotificationView
                {
                    Owner = Application.Current.MainWindow
                };
                notificationView.ShowDialog();

                isNotificationViewOpen = false;
            });
        }
        public virtual void Cleanup()
        {
            _pollingTimer.Stop();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
