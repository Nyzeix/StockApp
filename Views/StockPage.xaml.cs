using StockApp.Models;
using StockApp.ViewModels;

namespace StockApp.Views;

public partial class StockPage : ContentPage
{
	public StockPage()
	{
		InitializeComponent();
		BindingContext = new StockViewModel();
	}

	public void OnItemSelected(object sender, SelectionChangedEventArgs e)
	{
        if (e.CurrentSelection.FirstOrDefault() is Product selectedProduct)
        {
			// Navigate to ProductDetailPage
			// TODO
            //Navigation.PushAsync(new ProductDetailPage(selectedProduct));
        }
    }
}