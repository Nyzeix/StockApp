using CommunityToolkit.Maui.Views;
using StockApp.Models;

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
