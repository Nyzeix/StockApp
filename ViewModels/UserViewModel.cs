using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Maui.ApplicationModel;

namespace StockApp.ViewModels 
{

    public class UserViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly IAuthService _auth;
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<User> UsersList { get; set; } = new();

        public UserViewModel(IAuthService auth)
        {
            _auth = auth;
            foreach (var user in _auth.LoadUsers())
            {
                UsersList.Add(user);
            }
        }

        public async Task<string> AddUserAsync(string username, string password, Boolean isAdmin)
        {
            // Autoremediation: vérifier si l'utilisateur existe déjà (J'ai découvert un mot)
            var result = await _auth.AddUser(username, password, isAdmin).ConfigureAwait(false);
            if (result)
            {
                // Copilot à la resscousse pour mettre à jour l'UI thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UsersList.Add(new User { Username = username, PasswordHash = password, IsAdmin = isAdmin });
                    OnPropertyChanged(nameof(UsersList));
                });

                return "Utilisateur ajouté avec succès.";
            }
            else
            {
                return "L'utilisateur existe déjà.";
            }
        }

        public async Task<string> DeleteUserAsync(string username)
        {
            var result = await _auth.DeleteUser(username).ConfigureAwait(false);
            if (result)
            {
                // Met à jour l'UI thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var userToRemove = UsersList.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                    if (userToRemove != null)
                    {
                        UsersList.Remove(userToRemove);
                        OnPropertyChanged(nameof(UsersList));
                    }
                });
                return "Utilisateur supprimé avec succès.";
            }
            else
            {
                return "Échec de la suppression de l'utilisateur.";
            }
        }

        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}