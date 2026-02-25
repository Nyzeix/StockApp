using StockApp.Models;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core;

namespace StockApp.Views.Popups
{
    public partial class AddUserPopup : CommunityToolkit.Maui.Views.Popup
    {
        // On passe la liste des fournisseurs au constructeur pour remplir le picker
        public AddUserPopup()
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
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text)) return;
            if (string.IsNullOrWhiteSpace(PasswordEntry.Text)) return;

            var newUser = new User
            {
                Username = UsernameEntry.Text,
                PasswordHash = PasswordEntry.Text,
                IsAdmin = IsAdminSwitch.IsToggled
            };

            Close(newUser); // Retourne l'objet créé à la page principale
        }
    }
}
