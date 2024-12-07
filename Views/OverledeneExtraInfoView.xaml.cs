using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneExtraInfoView : UserControl
    {
        public OverledeneExtraInfoView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                ExtraInfoGrid.IsEnabled = false;

            input_DateOfBirthFamilie.SelectedDateChanged += DatePicker_SelectedDateChanged;
            input_DateOfBirthFamilieExtra.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (input_DateOfBirthFamilie.SelectedDate.HasValue)
            {
                DateTime birthDate = input_DateOfBirthFamilie.SelectedDate.Value;
                int age = CalculateAge(birthDate);

                input_AgeFamilie.Text = age.ToString();
            }

            if (input_DateOfBirthFamilieExtra.SelectedDate.HasValue)
            {
                DateTime birthDate = input_DateOfBirthFamilieExtra.SelectedDate.Value;
                int age = CalculateAge(birthDate);

                input_AgeFamilieExtra.Text = age.ToString();
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
        private void FetchExraInfoAddressInfoCommand(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneExtraInfoViewModal)this.DataContext;
            viewModel?.FetchExtraInfoAddressInfo();
        }
        private void FetchSecondExraInfoAddressInfoCommand(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneExtraInfoViewModal)this.DataContext;
            viewModel?.FetchSecondExtraInfoAddressInfo();
        }
    }
}
