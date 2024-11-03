using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dossier_Registratie.Helper
{
    public static class TextboxHelper
    {
        public static bool GetIsTimeMasked(DependencyObject obj) => (bool)obj.GetValue(IsTimeMaskedProperty);
        public static void SetIsTimeMasked(DependencyObject obj, bool value) => obj.SetValue(IsTimeMaskedProperty, value);

        public static readonly DependencyProperty IsTimeMaskedProperty =
            DependencyProperty.RegisterAttached(
                "IsTimeMasked",
                typeof(bool),
                typeof(TextboxHelper),
                new PropertyMetadata(false, OnIsTimeMaskedChanged));

        private static void OnIsTimeMaskedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += TimeTextBox_PreviewTextInput;
                    textBox.TextChanged += TimeTextBox_TextChanged;
                }
                else
                {
                    textBox.PreviewTextInput -= TimeTextBox_PreviewTextInput;
                    textBox.TextChanged -= TimeTextBox_TextChanged;
                }
            }
        }
        private static void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9:]");
        }

        private static void TimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length == 2 && !textBox.Text.Contains(":"))
            {
                textBox.Text += ":";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}
