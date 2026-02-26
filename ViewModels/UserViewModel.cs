using StockApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StockApp.ViewModels 
{
    /// <summary>
    /// ViewModel pour la gestion des utilisateurs, incluant le filtrage, l'ajout, la modification et la suppression.
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        private readonly IAuthDbService _auth;

        // Variable "Users" utilisé pour le View
        public ObservableCollection<User> Users { get; set; } = new();
        // Liste interne
        private List<User> _user { get; set; } = new List<User>();

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value; //1
                    OnPropertyChanged(); //2
                    ApplyFilters(); //3
                }
            }
        }

        // Edition / Suppression
        public ICommand LongPressDeleteCommand { get; private set; }


        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UserViewModel"/>.
        /// Charge les utilisateurs depuis le service d'authentification.
        /// </summary>
        /// <param name="auth">Service d'authentification.</param>
        /// <param name="log">Service de journalisation.</param>
        public UserViewModel(IAuthDbService auth, ILogService log)
        {
            _auth = auth;
            foreach (var user in _auth.LoadUsers())
            {
                _user.Add(user);
            }

            LongPressDeleteCommand = new Command<User>(async (user) => await DeleteUserCommandAsync(user));

            ApplyFilters();
            
        }


        /// <summary>
        /// Charge les utilisateurs depuis la base de données de manière asynchrone.
        /// </summary>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        private async Task LoadUsersAsync()
        {
            var users = await _auth.GetUsersAsync();
            _user.Clear();
            foreach (var user in users)
            {
                _user.Add(user);
            }
            ApplyFilters(); //Update UI
        }


        /// <summary>
        /// Ajoute un utilisateur en base de données et rafraîchit la liste.
        /// </summary>
        /// <param name="user">L'utilisateur à ajouter.</param>
        /// <returns><c>true</c> si l'ajout a réussi ; sinon <c>false</c>.</returns>
        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                await _auth.AddUser(user.Username, user.PasswordHash, user.IsAdmin);
                await LoadUsersAsync();
                OnPropertyChanged(nameof(Users));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Met à jour un utilisateur existant en base de données et rafraîchit la liste.
        /// </summary>
        /// <param name="modifiedUser">L'utilisateur modifié.</param>
        /// <returns><c>true</c> si la mise à jour a réussi ; sinon <c>false</c>.</returns>
        // Devrait-on pouvoir modifier le nom d'utilisateur ?
        // Si oui, beaucoup de modification sont à exécuter.
        public async Task<bool> UpdateUserAsync(User modifiedUser)
        {
            try
            {
                await _auth.UpdateUserAsync(modifiedUser);
                await LoadUsersAsync();
                ApplyFilters();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Supprime un utilisateur de la base de données et rafraîchit la liste.
        /// </summary>
        /// <param name="user">L'utilisateur à supprimer.</param>
        /// <returns><c>true</c> si la suppression a réussi ; sinon <c>false</c>.</returns>
        private async Task<bool> DeleteUserCommandAsync(User user)
        {
            if (user == null) return false;
            try
            {
                await _auth.DeleteUser(user.Username);
                await LoadUsersAsync();
                OnPropertyChanged(nameof(Users));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Applique les filtres de recherche sur la liste des utilisateurs.
        /// </summary>
        private void ApplyFilters()
        {
            IEnumerable<User> filtered = _user;

            // Filtre par nom
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Username != null && s.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            Users.Clear();
            foreach (var user in filtered)
                Users.Add(user);
        }
    }
}