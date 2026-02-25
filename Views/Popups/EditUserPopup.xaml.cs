using CommunityToolkit.Maui.Views;
using StockApp.Models;
using CommunityToolkit.Maui.Core;

namespace StockApp.Views
{
    public partial class EditUserPopup : Popup
    {
        private User _userToEdit;

        public EditUserPopup(User user)
        {
            InitializeComponent();
            _userToEdit = user;

            UsernameEntry.Text = user.Username;
            PasswordEntry.Text = user.PasswordHash;
            IsAdminSwitch.IsToggled = user.IsAdmin;

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
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                return;
            }

            _userToEdit.Username = UsernameEntry.Text;
            _userToEdit.PasswordHash = PasswordEntry.Text;
            _userToEdit.IsAdmin = IsAdminSwitch.IsToggled;

            await CloseAsync(_userToEdit);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await CloseAsync(null);
        }
    }
}
