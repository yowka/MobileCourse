using Course.Models;
using Course.Services;
using Microsoft.Maui.Controls;
using System.Text.Json;

namespace Course.Views
{
    public partial class MainPage : ContentPage
    {
        private WeatherService _weatherService;
        private CitySearchResponse _currentCity;
        private bool _isFirstLoad = true; // Добавь эту переменную

        public MainPage()
        {
            _weatherService = new WeatherService();
            BuildUI();
        }

        private void BuildUI()
        {
            // Плюсик в углу
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "+",
                
            });
            ToolbarItems[0].Clicked += OnAddCityClicked;

            // Пустой контейнер
            Content = new ScrollView
            {
                Content = new StackLayout()
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Загружаем только при первом появлении
            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            var savedCityJson = Preferences.Get("current_city", null);
            if (!string.IsNullOrEmpty(savedCityJson))
            {
                _currentCity = JsonSerializer.Deserialize<CitySearchResponse>(savedCityJson);
                await ShowWeather();
            }
            else
            {
                ShowNoCity();
            }
        }

        private async Task ShowWeather()
        {
            var mainStack = (StackLayout)((ScrollView)Content).Content;
            mainStack.Children.Clear(); // Очищаем перед добавлением

            var loading = new ActivityIndicator { IsRunning = true };
            mainStack.Children.Add(loading);

            try
            {
                var weather = await _weatherService.GetCurrentWeatherAsync(
                    _currentCity.Latitude, _currentCity.Longitude);

                mainStack.Children.Remove(loading);

                if (weather != null)
                {
                    // БЛОК 1: Основная информация
                    var mainBlock = new Frame
                    {
                        CornerRadius = 20,
                        HasShadow = true,
                        Padding = 20,
                        Margin = new Thickness(20, 10, 20, 10)
                    };

                    var mainStackInner = new StackLayout { Spacing = 10 };

                    mainStackInner.Children.Add(new Label
                    {
                        Text = weather.CityName ?? _currentCity.Name,
                        FontSize = 28,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    });

                    mainStackInner.Children.Add(new Label
                    {
                        Text = $"{Math.Round(weather.Main?.Temperature ?? 0)}°C",
                        FontSize = 64,
                        HorizontalTextAlignment = TextAlignment.Center
                    });

                    mainStackInner.Children.Add(new Label
                    {
                        Text = weather.Weather?[0]?.Description ?? "",
                        FontSize = 20,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.Gray
                    });

                    mainBlock.Content = mainStackInner;
                    mainStack.Children.Add(mainBlock);

                    // БЛОК 2: Детали
                    var detailsBlock = new Frame
                    {
                        CornerRadius = 15,
                        HasShadow = true,
                        Padding = 15,
                        Margin = new Thickness(20, 5, 20, 10)
                    };

                    var detailsGrid = new Grid
                    {
                        ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() },
                        RowDefinitions = { new RowDefinition(), new RowDefinition() },
                        ColumnSpacing = 15,
                        RowSpacing = 15
                    };

                    // Ощущается
                    AddDetail(detailsGrid, 0, 0, "Ощущается", $"{Math.Round(weather.Main?.FeelsLike ?? 0)}°C");

                    // Влажность
                    AddDetail(detailsGrid, 0, 1, "Влажность", $"{weather.Main?.Humidity ?? 0}%");

                    // Давление
                    AddDetail(detailsGrid, 1, 0, "Давление", $"{weather.Main?.Pressure ?? 0} гПа");

                    // Ветер
                    AddDetail(detailsGrid, 1, 1, "Ветер", $"{weather.Wind?.Speed ?? 0} м/с");

                    detailsBlock.Content = detailsGrid;
                    mainStack.Children.Add(detailsBlock);
                }
            }
            catch (Exception ex)
            {
                mainStack.Children.Clear();
                mainStack.Children.Add(new Label
                {
                    Text = $"Ошибка: {ex.Message}",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                });
            }
        }

        private void AddDetail(Grid grid, int row, int col, string title, string value)
        {
            var stack = new StackLayout { Spacing = 2 };

            stack.Children.Add(new Label
            {
                Text = title,
                FontSize = 12,
                TextColor = Colors.Gray
            });

            stack.Children.Add(new Label
            {
                Text = value,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            });

            grid.Children.Add(stack);
            Grid.SetRow(stack, row);
            Grid.SetColumn(stack, col);
        }

        private void ShowNoCity()
        {
            var mainStack = (StackLayout)((ScrollView)Content).Content;
            mainStack.Children.Clear();

            var stack = new StackLayout
            {
                Spacing = 20,
                VerticalOptions = LayoutOptions.Center,
                Padding = 40
            };

            stack.Children.Add(new Label
            {
                Text = "Нет сохраненного города",
                FontSize = 24,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var addButton = new Button
            {
                Text = "Добавить город",
                HeightRequest = 50,
                BackgroundColor = Color.FromArgb("#2196F3"),
                TextColor = Colors.White
            };
            addButton.Clicked += OnAddCityClicked;

            stack.Children.Add(addButton);

            mainStack.Children.Add(stack);
        }

        private async void OnAddCityClicked(object sender, EventArgs e)
        {
            // Создаем простую страницу поиска
            var searchPage = new ContentPage
            {
                Title = "Поиск города",
                Padding = new Thickness(20)
            };

            var mainStack = new StackLayout { Spacing = 10 };

            // Поле поиска
            var searchEntry = new Entry
            {
                Placeholder = "Введите город...",
                HeightRequest = 50
            };

            var searchButton = new Button
            {
                Text = "Найти",
                HeightRequest = 50,
                BackgroundColor = Color.FromArgb("#2196F3"),
                TextColor = Colors.White
            };

            var citiesList = new ListView
            {
                HasUnevenRows = true
            };

            searchButton.Clicked += async (s, e2) =>
            {
                var cityName = searchEntry.Text;
                if (!string.IsNullOrWhiteSpace(cityName))
                {
                    var cities = await _weatherService.SearchCitiesAsync(cityName);

                    var cityList = new List<CitySearchResponse>();
                    foreach (var city in cities)
                    {
                        cityList.Add(city);
                    }

                    citiesList.ItemsSource = cityList;
                }
            };

            citiesList.ItemTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
                nameLabel.SetBinding(Label.TextProperty, "Name");

                var detailLabel = new Label { FontSize = 14, TextColor = Colors.Gray };
                detailLabel.SetBinding(Label.TextProperty, "Country");

                var selectButton = new Button
                {
                    Text = "Выбрать",
                    WidthRequest = 80,
                    BackgroundColor = Color.FromArgb("#4CAF50"),
                    TextColor = Colors.White
                };
                selectButton.SetBinding(Button.CommandParameterProperty, ".");

                selectButton.Clicked += (s, e2) =>
                {
                    if (selectButton.CommandParameter is CitySearchResponse city)
                    {
                        // Сохраняем город
                        _currentCity = city;
                        var json = JsonSerializer.Serialize(city);
                        Preferences.Set("current_city", json);

                        // Обновляем интерфейс
                        _ = ShowWeather();

                        // Закрываем поиск
                        Navigation.PopModalAsync();
                    }
                };

                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Padding = new Thickness(10)
                };

                var textStack = new StackLayout { Children = { nameLabel, detailLabel } };
                grid.Children.Add(textStack);
                grid.Children.Add(selectButton);
                Grid.SetColumn(selectButton, 1);

                return new ViewCell { View = grid };
            });

            // Кнопка "Назад"
            var backButton = new Button
            {
                Text = "Назад",
                HeightRequest = 50,
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Black
            };
            backButton.Clicked += async (s, e2) => await Navigation.PopModalAsync();

            mainStack.Children.Add(searchEntry);
            mainStack.Children.Add(searchButton);
            mainStack.Children.Add(citiesList);
            mainStack.Children.Add(backButton);

            searchPage.Content = mainStack;
            await Navigation.PushModalAsync(searchPage);
        }
    }
}