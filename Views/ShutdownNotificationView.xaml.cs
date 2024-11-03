using System.ComponentModel;
using System.Timers;
using System.Windows;

namespace Dossier_Registratie.Views
{
    public partial class ShutdownNotificationView : Window, INotifyPropertyChanged
    {
        private Timer _timer;
        private int _remainingSeconds;
        private string _notificationText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NotificationText
        {
            get => _notificationText;
            set
            {
                _notificationText = value;
                OnPropertyChanged(nameof(NotificationText));
            }
        }

        public ShutdownNotificationView()
        {
            InitializeComponent();
            DataContext = this;

            _remainingSeconds = 30;
            NotificationText = $"In verband met een verplichte update sluit de applicatie zichzelf over \r\n" +
                                            $" {_remainingSeconds} seconden. \r\n " +
                                            $"Sla het openstaande werk op.";

            StartCountdown();
        }

        private void StartCountdown()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _remainingSeconds--;

            if (_remainingSeconds <= 0)
            {
                _timer.Stop();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_remainingSeconds < 2)
                    {
                        NotificationText = $"In verband met een verplichte update sluit de applicatie zichzelf over \r\n" +
                                            $" {_remainingSeconds} seconde. \r\n " +
                                            $"Sla het openstaande werk op.";
                    }
                    else
                    {
                        NotificationText = $"In verband met een verplichte update sluit de applicatie zichzelf over \r\n" +
                                            $" {_remainingSeconds} seconden. \r\n " +
                                            $"Sla het openstaande werk op.";
                    }

                });
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
