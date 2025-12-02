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


        public ObservableCollection<User> ExampleUsers { get; set; } = new();

        public UserViewModel(IAuthService auth)
        {
            // Chargement des données d'essais, sans passer par une BDD
            //LoadExampleUsers();

            _auth = auth;
            // Ajouter les utilisateurs dans la liste ExampleUsers
            foreach (var user in _auth.LoadUsers())
            {
                ExampleUsers.Add(user);
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
                    ExampleUsers.Add(new User { Username = username, PasswordHash = password, IsAdmin = isAdmin });
                    OnPropertyChanged(nameof(ExampleUsers));
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
                    var userToRemove = ExampleUsers.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                    if (userToRemove != null)
                    {
                        ExampleUsers.Remove(userToRemove);
                        OnPropertyChanged(nameof(ExampleUsers));
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