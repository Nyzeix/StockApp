using StockApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StockApp.ViewModels 
{

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