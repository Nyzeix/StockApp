using StockApp.Models;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core;

namespace StockApp.Views.Popups
{
    public partial class AddSupplierPopup : CommunityToolkit.Maui.Views.Popup
    {
        // On passe la liste des fournisseurs au constructeur pour remplir le picker
        public AddSupplierPopup()
        {
            InitializeComponent();
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
