using Dossier_Registratie.Interfaces;
using System.Windows;

namespace Dossier_Registratie.Helper
{
    public class DocumentGeneratingWindow : IGeneratingDocumentWindow
    {
        private readonly Window _window;

        public DocumentGeneratingWindow(Window window)
        {
            _window = window;
        }

        public void Show()
        {
            _window.Dispatcher.Invoke(() => _window.Visibility = Visibility.Visible);
        }

        public void Hide()
        {
            _window.Dispatcher.Invoke(() => _window.Visibility = Visibility.Collapsed);
        }
    }

}
