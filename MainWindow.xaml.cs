using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.ViewModels;
using Dossier_Registratie.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dossier_Registratie
{
    public partial class MainWindow : Window
    {
        private TabItem _previousTabItem;
        private bool _isFirstSelection = true;

        private bool _tabControlChecked = false;
        public static readonly RoutedEvent NextClickedEvent = EventManager.RegisterRoutedEvent("ClickedNext", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindow));
        public static readonly RoutedEvent PreviousClickedEvent = EventManager.RegisterRoutedEvent("ClickedPrevious", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindow));

        public bool TabControlChecked
        {
            get => _tabControlChecked;
            set => _tabControlChecked = value;
        }

        public MainWindow()
        {
            InitializeComponent();

            Closing += MainWindow_Closing;
            CheckWindowsUserEmployee();

            lblLoggedIn.Content = "Gebruiker: " + Environment.UserName;
            lblAccessLevel.Content = "Rechten: " + Globals.PermissionLevelName;

            AddHandler(NextClickedEvent, new RoutedEventHandler(OnNextClickedEvent));
            AddHandler(PreviousClickedEvent, new RoutedEventHandler(OnPreviousClickedEvent));

            if (Globals.PermissionLevelName == "Gebruiker" || Globals.PermissionLevelName == "NotRegistered")
            {
                NewDossier.IsEnabled = false;
                NewDossier.Background = new SolidColorBrush(Colors.LightGray);
            }

            ComboBox permissionComboBox = MainComboBox;
            ComboBoxItem permissionComboBoxItem = new ComboBoxItem();
            permissionComboBoxItem.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFC92F39");
            permissionComboBoxItem.Foreground = new SolidColorBrush(Colors.White);
            permissionComboBoxItem.Height = 38;
            permissionComboBoxItem.FontSize = 14;
            permissionComboBoxItem.Content = "Beheer";

            switch (Globals.PermissionLevelId.ToUpper())
            {
                case "A224C94E-2F54-4D43-A976-11E24287A8E0": //Beheerder
                case "D8454762-9245-4B6C-9D29-293B9BC2FFB2": //System
                case "D3BD7AE6-978D-4F1A-972C-B033CFC801E3": //Uitvaartleider - Limited
                case "8DBB3112-153D-4592-ABE2-77C79D61F81A": //Financieel
                    permissionComboBox.Items.Add(permissionComboBoxItem);
                    break;
            }
        }
        private void CheckWindowsUserEmployee()
        {
            var searchOperation = new SearchOperations();
            var (permissionLevelId, permissionLevelName) = searchOperation.FetchUserCredentials(Environment.UserName);

            Globals.PermissionLevelId = permissionLevelId;
            Globals.PermissionLevelName = permissionLevelName;

            if (permissionLevelId == Guid.Empty.ToString())
            {
                MessageBoxResult createAccount = MessageBox.Show("Je wilt de applicatie openen met een account die niet bekend is.\r\n" +
                                "Wil je een account aanmaken?", "Onbekend account", MessageBoxButton.YesNo);
                if (createAccount == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    if (DataContext is MainWindowViewModal viewModel)
                    {
                        viewModel.IsCreateUserPopupOpen = true;
                    }
                }
            }
        }
        public static void OnNextClickedEvent(object sender, RoutedEventArgs e)
        {
            IntAggregator.Transmit(1);
        }
        public void OnPreviousClickedEvent(object sender, RoutedEventArgs e)
        {
            if (TabHeader.SelectedIndex == 0)
            {
                MessageBox.Show("Je kunt niet verder terug");
            }
            else
            {
                TabHeader.SelectedIndex--;
            }
        }
        private void AlleUitvaarten_Click(object sender, RoutedEventArgs e)
        {
            TabHeader.SelectedIndex = 13;
        }
        private void AgendaUitvaarten_Click(object sender, RoutedEventArgs e)
        {
            TabHeader.SelectedIndex = 10;
        }
        private void MainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = MainComboBox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedItemString = selectedItem.ToString();

                if (selectedItemString.Contains("Start"))
                {
                    TabHeader.SelectedIndex = 0;
                    MainComboBox.SelectedItem = null;
                }
                else if (selectedItemString.Contains("Notificaties"))
                {
                    var notificatieViewModel = new OverledeneNotificationViewModel();
                    OverledeneNotification notificatieWindow = new()
                    {
                        DataContext = notificatieViewModel
                    };
                    notificatieWindow.Show();
                    MainComboBox.SelectedItem = null;
                }
                else if (selectedItemString.Contains("Beheer"))
                {
                    BeheerWindow beheerWindow = new();
                    beheerWindow.Show();
                    MainComboBox.SelectedItem = null;
                }
                else if (selectedItemString.Contains("Agenda"))
                {
                    TabHeader.SelectedIndex = 10;
                    MainComboBox.SelectedItem = null;
                }
                else if (selectedItemString.Contains("Help"))
                {
                    TabHeader.SelectedIndex = 12;
                    MainComboBox.SelectedItem = null;
                }
                else if (selectedItemString.Contains("Alle Uitvaarten"))
                {
                    TabHeader.SelectedIndex = 13;
                    MainComboBox.SelectedItem = null;
                }
                else if (selectedItemString.Contains("Handleiding"))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://patrickst1991.github.io/Uitvaartverzorging-Dossier-Registratie/",
                        UseShellExecute = true
                    });

                    MainComboBox.SelectedItem = null;
                }
            }
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Globals.NewDossierCreation)
                return;

            if (sender is not TabControl tabControl) 
                return;

            HandleFirstSelection(tabControl, e);
            HandleTabChange(tabControl, e);
        }
        private void HandleFirstSelection(TabControl tabControl, SelectionChangedEventArgs e)
        {
            if (_isFirstSelection)
            {
                _isFirstSelection = false;
                return;
            }

            if (tabControl.SelectedIndex == 0) MainComboBox.SelectedIndex = -1;
        }
        private void HandleTabChange(TabControl tabControl, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is TabItem oldTabItem)
            {
                HandleTabDeactivation(oldTabItem);
            }

            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem selectedTabItem)
            {
                HandleTabActivation(selectedTabItem);
                _previousTabItem = selectedTabItem;
            }
        }
        private void HandleTabDeactivation(TabItem oldTabItem)
        {
            if (oldTabItem.Content is FrameworkElement oldTabContent)
            {
                var viewModel = oldTabContent.DataContext;
                if (viewModel != null)
                {
                    ExecuteSaveCommand(viewModel);
                }
            }
        }
        private void ExecuteSaveCommand(object viewModel)
        {
            var saveCommandProperty = viewModel.GetType().GetProperty("SaveCommand");
            if (saveCommandProperty?.GetValue(viewModel) is ICommand saveCommand && saveCommand.CanExecute(null))
            {
                saveCommand.Execute(null);
                TabControlChecked = true;
            }
            else
            {
                TabControlChecked = false;
            }
        }
        private void HandleTabActivation(TabItem selectedTabItem)
        {
            if (selectedTabItem.Content is FrameworkElement selectedTabContent)
            {
                var viewModel = selectedTabContent.DataContext;
                if (viewModel != null)
                {
                    ExecuteTabSpecificActions(viewModel);
                }
            }
        }
        private void ExecuteTabSpecificActions(object viewModel)
        {
            if (viewModel.GetType().Name == "MainWindowViewModal")
                ExecuteClearAllModelsCommand(viewModel);
            else
                InvokeRequestedDossierInformationMethod(viewModel, Globals.UitvaartCode);
        }
        private void ExecuteClearAllModelsCommand(object viewModel)
        {
            var clearCommandProperty = viewModel.GetType().GetProperty("ClearAllModelsCommand");
            if (clearCommandProperty?.GetValue(viewModel) is ICommand clearCommand && clearCommand.CanExecute(null))
            {
                clearCommand.Execute(null);
                TabControlChecked = false;
            }
        }
        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TabItem && Globals.NewDossierCreation)
            {
                new ToastWindow("De navigatie balk is alleen beschikbaar bij een bestaand dossier!").Show();
                e.Handled = true;
            }
        }
        /*
                private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    if (!Globals.NewDossierCreation)
                    {
                        if (sender is not TabControl tabControl) return;

                        if (tabControl.SelectedIndex == 0) MainComboBox.SelectedIndex = -1;

                        if (e.Source is TabControl && e.RemovedItems.Count > 0 && e.RemovedItems[0] is TabItem oldTabItem && oldTabItem.Content is FrameworkElement oldTabContent)
                        {
                            var viewModel = oldTabContent.DataContext;
                            if (viewModel != null)
                            {
                                var saveCommandProperty = viewModel.GetType().GetProperty("SaveCommand");
                                if (saveCommandProperty?.GetValue(viewModel) is ICommand saveCommand && saveCommand.CanExecute(null))
                                {
                                    saveCommand.Execute(null);
                                    TabControlChecked = true;
                                }
                                else
                                {
                                    TabControlChecked = false;
                                    return;
                                }
                            }
                        }

                        if (_isFirstSelection)
                        {
                            _isFirstSelection = false;
                            return;
                        }

                        if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem selectedTabItem &&
                            selectedTabItem != _previousTabItem)
                        {
                            if (selectedTabItem.Content is FrameworkElement selectedTabContent)
                            {
                                var viewModel = selectedTabContent.DataContext;
                                if (viewModel != null)
                                {
                                    if (viewModel.GetType().Name == "MainWindowViewModal")
                                    {
                                        var clearCommandProperty = viewModel.GetType().GetProperty("ClearAllModelsCommand");
                                        if (clearCommandProperty?.GetValue(viewModel) is ICommand clearCommand && clearCommand.CanExecute(null))
                                        {
                                            clearCommand.Execute(null);
                                            TabControlChecked = false;
                                        }
                                    }
                                    else
                                    {
                                        InvokeRequestedDossierInformationMethod(viewModel, Globals.UitvaartCode);
                                    }
                                }
                            }

                            _previousTabItem = selectedTabItem;
                        }
                    }
                }
        */
        private static void InvokeRequestedDossierInformationMethod(object viewModel, string uitvaartNummer)
        {
            var method = viewModel.GetType().GetMethod("RequestedDossierInformationBasedOnUitvaartId");
            if (method != null)
                _ = method.Invoke(viewModel, new object[] { uitvaartNummer });
        }
        /*
        public static class AgenAggregator
        {
            public static void BroadCast(string message)
            {
                if (OnMessageTransmitted != null)
                    OnMessageTransmitted(message);
            }

            public static Action<string> OnMessageTransmitted;
        }
        */
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModal viewModel)
            {
                viewModel.Cleanup();
            }
        }
    }
}