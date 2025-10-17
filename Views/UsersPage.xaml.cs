using StockApp.Models;
using StockApp.ViewModels;

namespace StockApp.Views;

public partial class UsersPage : ContentPage
{
	public UsersPage()
	{
		InitializeComponent();
        BindingContext = new UserViewModel();
    }

    public void TODO(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Product selectedUser)
        {
            // Navigate to ProductDetailPage
            // TODO
            //Navigation.PushAsync(new UserDetailPage(selectedUser));
        }
    }
}