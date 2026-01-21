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
        private readonly IAuthDbService _auth;
        private readonly ILogService _log;
        private readonly string log_tag = "UserManagement";
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<User> UsersList { get; set; } = new();

        public UserViewModel(IAuthDbService auth, ILogService log)
        {
            _auth = auth;
            _log = log;
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
                _log.LogInfo(log_tag, $"User '{username}' added.");
                return "Utilisateur ajouté avec succès.";
            }
            else
            {
                return "L'utilisateur existe déjà.";
            }
        }

        public async Task<string> DeleteUserAsync(string username)
        {
            var result = _auth.DeleteUser(username).Result;
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
                _log.LogInfo(log_tag, $"User '{username}' deleted.");
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