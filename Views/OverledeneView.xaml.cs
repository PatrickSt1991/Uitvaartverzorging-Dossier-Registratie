using Dossier_Registratie.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneView : UserControl
    {
        public OverledeneView()
        {
            InitializeComponent();
            input_DateOfBirthOverledene.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (input_DateOfBirthOverledene.SelectedDate.HasValue)
            {
                DateTime birthDate = input_DateOfBirthOverledene.SelectedDate.Value;
                int age = CalculateAge(birthDate);

                input_AgeOverledene.Text = age.ToString();
            }
        }
        private int CalculateAge(DateTime birthDate)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - birthDate.Year;

            if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneViewModel)this.DataContext;
            viewModel.RefresDynamicElements();
        }
        private void FetchPersoonAddressInfoCommand(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneViewModel)this.DataContext;
            viewModel?.FetchPersoonAddressInfo();
        }
        private void FetchOverledenAddressInfoCommand(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneViewModel)this.DataContext;
            viewModel?.FetchOverlijdenAddressInfo();
        }
        private void FetchOverledenLocationInfoCommand(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneViewModel)this.DataContext;
            viewModel?.FetchOverlijdenLocationInfo();
        }
    }
}
