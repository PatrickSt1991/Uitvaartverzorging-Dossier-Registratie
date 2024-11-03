using System;
using System.Timers;
using System.Windows;

namespace Dossier_Registratie.Helper
{
    public partial class ToastWindow : Window
    {
        private Timer _timer;
        public ToastWindow(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the screen width and calculate the horizontal center
            double screenWidth = SystemParameters.WorkArea.Width; // WorkArea excludes taskbar
            double screenHeight = SystemParameters.WorkArea.Height; // WorkArea excludes taskbar
            double windowWidth = this.Width;
            double windowHeight = this.Height;

            // Calculate the top center position
            this.Left = (screenWidth - windowWidth) / 2; // Horizontally center the window
            this.Top = 10; // Position it at the top (adjust as needed)

            // Optionally, close the window after a few seconds (e.g., 3 seconds)
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }
    }
}
