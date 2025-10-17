using StockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace StockApp.ViewModels 
{

    class UserViewModel : BaseViewModel, INotifyPropertyChanged
    {
        // Event de notification de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<User> ExampleUsers { get; set; } = new();

        public UserViewModel()
        {
            // Chargement des données d'essais, sans passer par une BDD
            LoadExampleUsers();


        }

        private void LoadExampleUsers()
        {
            // Exemple de données statiques
            ExampleUsers.Add(new User { Username = "Pommes", IsAdmin = false });
            ExampleUsers.Add(new User { Username = "A", IsAdmin = true });
            ExampleUsers.Add(new User { Username = "X", IsAdmin = true });
        }

        // Appelle la vue si une propriété évolue
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}