using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneFactuurView : UserControl
    {
        public bool SearchIsUpdate = false;

        public OverledeneFactuurView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                GridFactuurView.IsEnabled = false;

            if (DataContext is OverledeneFactuurViewModel viewModel)
            {
                viewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsPopupVisible")
                        verzekeringPopup.IsOpen = viewModel.IsPopupVisible;
                };
            }

        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneFactuurViewModel)this.DataContext;
            viewModel.ReloadDynamicElements();
        }
        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneFactuurViewModel)DataContext;
            viewModel?.UpdateSubtotaal();
        }
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            // Check if the Delete key was pressed
            if (e.Key == Key.Delete)
            {
                // Get the currently selected cell
                var currentCell = dataGrid.CurrentCell;

                // Find the DataGridRow and DataGridCell
                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(currentCell.Item) as DataGridRow;
                if (row != null)
                {
                    // Find the corresponding DataGridCell for the column
                    DataGridCell cell = GetCell(dataGrid, row, currentCell.Column.DisplayIndex);

                    // Check if the cell is in editing mode
                    if (cell != null && cell.IsEditing)
                    {
                        // The cell is being edited, so allow deletion without confirmation
                        return;
                    }
                }

                // If not editing, show confirmation message for row deletion
                if (dataGrid.SelectedItems.Count > 0)
                {
                    var result = MessageBox.Show("Je staat op het punt deze regel te verwijderen van de kostenbegroting, weet je het zeker?",
                                                 "Kostenbegroting",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        // Cancel the delete action
                        e.Handled = true;
                    }
                }
            }


            if (e.Key == Key.Enter)
            {
                var viewModel = (OverledeneFactuurViewModel)DataContext;
                viewModel?.UpdateSubtotaal();

                e.Handled = false;
            }
        }

        // Helper function to get the DataGridCell
        private DataGridCell GetCell(DataGrid grid, DataGridRow row, int columnIndex)
        {
            if (row != null)
            {
                var presenter = GetVisualChild<DataGridCellsPresenter>(row);
                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[columnIndex]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }
                if (presenter != null)
                {
                    return presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }
            }
            return null;
        }

        // Helper method to get visual children (used for DataGridCell retrieval)
        private T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var viewModel = (OverledeneFactuurViewModel)DataContext;
                if (e.EditAction == DataGridEditAction.Commit && e.Row.IsNewItem)
                {
                    if (e.Row.GetIndex() == dataGrid.Items.Count - 1)
                        viewModel?.AddNewItemToCollection();
                }

                viewModel?.UpdateSubtotaal();
            }
        }
    }
}
