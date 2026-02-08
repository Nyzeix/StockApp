using StockApp.Models;
using CommunityToolkit.Maui.Views;

namespace StockApp.Views.Popups
{
    public partial class AddSupplierPopup : CommunityToolkit.Maui.Views.Popup
    {
        // On passe la liste des fournisseurs au constructeur pour remplir le picker
        public AddSupplierPopup()
        {
            InitializeComponent();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(null); // Retourne null si annulé
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text)) return;
            if (string.IsNullOrWhiteSpace(TypeEntry.Text)) return;

            var newSupplier = new Supplier
            {
                Name = NameEntry.Text,
                Type = TypeEntry.Text
            };

            Close(newSupplier); // Retourne l'objet créé à la page principale
        }
    }
}
