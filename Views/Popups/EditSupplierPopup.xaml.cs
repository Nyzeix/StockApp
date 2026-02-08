using CommunityToolkit.Maui.Views;
using StockApp.Models;
using CommunityToolkit.Maui.Core;

namespace StockApp.Views
{
    public partial class EditSupplierPopup : Popup
    {
        private Supplier _supplierToEdit;

        public EditSupplierPopup(Supplier supplier)
        {
            InitializeComponent();
            _supplierToEdit = supplier;

            NameEntry.Text = supplier.Name;
            TypeEntry.Text = supplier.Type;

            Opened += OnPopupOpened;

        }

        
        // OnOpened pour connaître la taille de la fenêtre parente
        private void OnPopupOpened(object? sender, PopupOpenedEventArgs e)
        {
            if (Shell.Current?.CurrentPage != null)
            {
                double targetWidth = Shell.Current.CurrentPage.Width * 0.60;
                PopupContainer.WidthRequest = targetWidth;
                var measure = PopupContainer.Measure(targetWidth, double.PositiveInfinity);
                Size = new Size(targetWidth, measure.Height);
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(TypeEntry.Text))
            {
                return;
            }

            _supplierToEdit.Name = NameEntry.Text;
            _supplierToEdit.Type = TypeEntry.Text;

            await CloseAsync(_supplierToEdit);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await CloseAsync(null);
        }
    }
}
