using System.Windows;

namespace Dossier_Registratie.Views
{
    public partial class CustomMessageBox : Window
    {
        public enum CustomMessageBoxResult
        {
            Continue,
            Stop
        }
        public CustomMessageBoxResult Result { get; private set; }
        public CustomMessageBox(string title, string header, string message, string continueButton, string stopButton)
        {
            InitializeComponent();
            this.Title = title;
            HeaderTextBlock.Text = header;
            MessageTextBlock.Text = message;
            ContinueButton.Content = continueButton;
            StopButton.Content = stopButton;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CustomMessageBoxResult.Continue;
            this.DialogResult = true;
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CustomMessageBoxResult.Stop;
            this.DialogResult = true;
        }

        public static CustomMessageBoxResult Show(string title, string header, string message, string continueButton, string stopButton)
        {
            CustomMessageBox box = new CustomMessageBox(title, header, message, continueButton, stopButton);
            box.ShowDialog();
            return box.Result;
        }
    }
}
