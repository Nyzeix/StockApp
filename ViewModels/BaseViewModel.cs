using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockApp.ViewModels
{
    /// <summary>
    /// Classe de base abstraite pour les ViewModels, implémentant <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        
        protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }


        /// <summary>
        /// Méthode pour notifier les changements de propriétés
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}