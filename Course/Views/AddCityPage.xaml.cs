using Course.Models;
using Course.ViewModels;
using Microsoft.Maui.Controls;

namespace Course.Views
{
    public partial class AddCityPage : ContentPage
    {
        private MainViewModel _viewModel; // Используем ту же ViewModel

        public AddCityPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            // Поиск городов нужно сделать отдельным сервисом
            // Пока оставим пустым или добавь отдельную VM для поиска
        }

        private async void OnCityTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is CitySearchResponse city)
            {
                // Сохраняем город
                _viewModel.SaveCity(city);

                // Закрываем страницу
                await Navigation.PopModalAsync();
            }
        }
    }
}