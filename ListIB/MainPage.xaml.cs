using ListIB.Models;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;

namespace ListIB
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Riik> _riikideKogum;
        private List<Riik> _kõikRiigid;
        
        // UI Controls
        private SearchBar _otsinguRiba;
        private ListView _riikideList;
        private Entry _nimiSisend;
        private Entry _pealinnSisend;
        private Stepper _hinnangStepper;
        private Slider _arenguTaseLiugur;
        private DatePicker _külastusKuuPäev;
        private Editor _infoToimetaja;
        
        private ImageSource? _praeguneLippSource = null;
        private Riik? _valitudRiik = null;

        public MainPage()
        {
            InitializeComponent();
            InitializeData();
            BuildMobileUI();
        }

        private void InitializeData()
        {
            _kõikRiigid = new List<Riik>
            {
                new Riik 
                { 
                    Nimi = "Estonia", 
                    Pealinn = "Tallinn", 
                    Rahvaarv = 1330000, 
                    Rating = 5, 
                    DevelopmentLevel = 8.5, 
                    Lisainfo = "Ilus riik Põhja-Euroopas.", 
                    Lipp = ImageSource.FromFile("ee.png") 
                }
            };
            _riikideKogum = new ObservableCollection<Riik>(_kõikRiigid);
        }

        private void BuildMobileUI()
        {
            this.BackgroundColor = Color.FromArgb("#f5f5f5");

            var mainGrid = new Grid
            {
                RowDefinitions = 
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = new GridLength(2.5, GridUnitType.Star) }
                }
            };

            // 1. SearchBar (Top)
            _otsinguRiba = new SearchBar { Placeholder = "Otsi riiki...", BackgroundColor = Colors.White, Margin = new Thickness(10, 10, 10, 0) };
            _otsinguRiba.TextChanged += OnSearchTextChanged;
            mainGrid.Add(_otsinguRiba, 0, 0);

            // 2. ListView (Middle)
            _riikideList = new ListView
            {
                HasUnevenRows = true,
                ItemsSource = _riikideKogum,
                SelectionMode = ListViewSelectionMode.Single,
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(10)
            };
            _riikideList.ItemTapped += OnItemTapped;

            _riikideList.ItemTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    CornerRadius = 10,
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = 10,
                    BackgroundColor = Colors.White,
                    HasShadow = true,
                    BorderColor = Colors.Transparent
                };

                var rowGrid = new Grid
                {
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    ColumnSpacing = 15
                };

                var flagImage = new Image
                {
                    WidthRequest = 50,
                    HeightRequest = 50,
                    Aspect = Aspect.AspectFill,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Clip = new EllipseGeometry { Center = new Point(25, 25), RadiusX = 25, RadiusY = 25 }
                };
                flagImage.SetBinding(Image.SourceProperty, "Lipp");

                var vStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 2 };
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Color.FromArgb("#2A3E52") };
                nameLabel.SetBinding(Label.TextProperty, "Nimi");

                var capitalLabel = new Label { FontSize = 14, TextColor = Colors.Gray };
                capitalLabel.SetBinding(Label.TextProperty, "Pealinn");

                vStack.Children.Add(nameLabel);
                vStack.Children.Add(capitalLabel);

                var favSwitch = new Switch { VerticalOptions = LayoutOptions.Center };
                favSwitch.SetBinding(Switch.IsToggledProperty, "IsFavorite");
                
                rowGrid.Add(flagImage, 0, 0);
                rowGrid.Add(vStack, 1, 0);
                rowGrid.Add(favSwitch, 2, 0);

                frame.Content = rowGrid;

                var deleteAction = new MenuItem { Text = "Kustuta", IsDestructive = true };
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, ".");
                deleteAction.Clicked += OnSwipeDeleteClicked;

                var viewCell = new ViewCell { View = frame };
                viewCell.ContextActions.Add(deleteAction);

                return viewCell;
            });

            mainGrid.Add(_riikideList, 0, 1);

            // 3. Form (Bottom)
            var contentLayout = new VerticalStackLayout { Spacing = 12, Padding = new Thickness(20, 10, 20, 20) };
            
            _nimiSisend = new Entry { Placeholder = "Riigi nimi", BackgroundColor = Colors.White };
            _pealinnSisend = new Entry { Placeholder = "Pealinn", BackgroundColor = Colors.White };
            
            var chooseFlagBtn = new Button 
            { 
                Text = "Vali lipp galeriist", 
                BackgroundColor = Color.FromArgb("#E67E22"), // Orange
                TextColor = Colors.White,
                CornerRadius = 8
            };
            chooseFlagBtn.Clicked += OnChooseFlagClicked;

            _hinnangStepper = new Stepper { Minimum = 1, Maximum = 5, Value = 1, HorizontalOptions = LayoutOptions.Start };
            var ratingLabel = new Label { Text = "Hinnang: 1", VerticalOptions = LayoutOptions.Center };
            _hinnangStepper.ValueChanged += (s, e) => ratingLabel.Text = $"Hinnang: {e.NewValue}";

            var stepperLayout = new HorizontalStackLayout { Spacing = 15 };
            stepperLayout.Children.Add(_hinnangStepper);
            stepperLayout.Children.Add(ratingLabel);

            _arenguTaseLiugur = new Slider { Minimum = 0, Maximum = 10, Value = 5 };
            var devLabel = new Label { Text = "Arengutase" };

            _külastusKuuPäev = new DatePicker { BackgroundColor = Colors.White };
            var dateLabel = new Label { Text = "Plaanitud külastuse kuupäev:" };

            _infoToimetaja = new Editor { Placeholder = "Kirjeldus (Lisainfo)", HeightRequest = 80, BackgroundColor = Colors.White };

            // Buttons
            var buttonsGrid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                ColumnSpacing = 5,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var addBtn = new Button { Text = "Lisa", BackgroundColor = Color.FromArgb("#27AE60"), TextColor = Colors.White, CornerRadius = 8 };
            addBtn.Clicked += OnAddClicked;
            var updateBtn = new Button { Text = "Muuda", BackgroundColor = Color.FromArgb("#2980B9"), TextColor = Colors.White, CornerRadius = 8 };
            updateBtn.Clicked += OnUpdateClicked;
            var deleteBtn = new Button { Text = "Kustuta valitud", BackgroundColor = Color.FromArgb("#C0392B"), TextColor = Colors.White, CornerRadius = 8 };
            deleteBtn.Clicked += OnDeleteSelectedClicked;

            buttonsGrid.Add(addBtn, 0, 0);
            buttonsGrid.Add(updateBtn, 1, 0);
            buttonsGrid.Add(deleteBtn, 2, 0);

            contentLayout.Children.Add(new Label { Text = "Halda riiki", FontAttributes = FontAttributes.Bold, FontSize = 18, TextColor = Color.FromArgb("#2A3E52") });
            contentLayout.Children.Add(_nimiSisend);
            contentLayout.Children.Add(_pealinnSisend);
            contentLayout.Children.Add(chooseFlagBtn);
            
            contentLayout.Children.Add(new BoxView { HeightRequest = 1, Color = Colors.LightGray, Margin = new Thickness(0, 5) });
            contentLayout.Children.Add(stepperLayout);
            contentLayout.Children.Add(devLabel);
            contentLayout.Children.Add(_arenguTaseLiugur);
            
            contentLayout.Children.Add(dateLabel);
            contentLayout.Children.Add(_külastusKuuPäev);
            contentLayout.Children.Add(_infoToimetaja);
            contentLayout.Children.Add(buttonsGrid);

            var scrollView = new ScrollView { Content = contentLayout };
            mainGrid.Add(scrollView, 0, 2);

            this.Content = mainGrid;
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            string keyword = e.NewTextValue?.ToLower() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                _riikideKogum = new ObservableCollection<Riik>(_kõikRiigid);
            }
            else
            {
                var filtered = _kõikRiigid.Where(r => r.Nimi.ToLower().Contains(keyword));
                _riikideKogum = new ObservableCollection<Riik>(filtered);
            }
            _riikideList.ItemsSource = _riikideKogum;
        }

        private async void OnChooseFlagClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    _praeguneLippSource = ImageSource.FromFile(result.FullPath);
                    await DisplayAlert("Edu", "Lipu pilt valitud!", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Viga", $"Pildi valimine ebaõnnestus: {ex.Message}", "OK");
            }
        }

        private void OnAddClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nimiSisend.Text) || string.IsNullOrWhiteSpace(_pealinnSisend.Text))
            {
                DisplayAlert("Valideerimise viga", "Nimi ja pealinn ei tohi olla tühjad.", "OK");
                return;
            }

            if (_kõikRiigid.Any(r => r.Nimi.Equals(_nimiSisend.Text, StringComparison.OrdinalIgnoreCase)))
            {
                DisplayAlert("Duplikaadi viga", $"Riik nimega '{_nimiSisend.Text}' on juba olemas.", "OK");
                return;
            }

            var newRiik = new Riik
            {
                Nimi = _nimiSisend.Text,
                Pealinn = _pealinnSisend.Text,
                Lipp = _praeguneLippSource ?? ImageSource.FromFile("dotnet_bot.png"),
                Rating = (int)_hinnangStepper.Value,
                DevelopmentLevel = _arenguTaseLiugur.Value,
                VisitDate = _külastusKuuPäev.Date ?? DateTime.Now,
                Lisainfo = _infoToimetaja.Text
            };

            _kõikRiigid.Add(newRiik);
            
            string keyword = _otsinguRiba.Text?.ToLower() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(keyword) || newRiik.Nimi.ToLower().Contains(keyword))
            {
                _riikideKogum.Add(newRiik);
            }
            
            ClearInputs();
        }

        private void OnUpdateClicked(object? sender, EventArgs e)
        {
            if (_valitudRiik == null)
            {
                DisplayAlert("Viga", "Palun vali nimekirjast riik, mida uuendada.", "OK");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(_nimiSisend.Text) || string.IsNullOrWhiteSpace(_pealinnSisend.Text))
            {
                DisplayAlert("Valideerimise viga", "Nimi ja pealinn ei tohi olla tühjad.", "OK");
                return;
            }

            if (_kõikRiigid.Any(r => r != _valitudRiik && r.Nimi.Equals(_nimiSisend.Text, StringComparison.OrdinalIgnoreCase)))
            {
                DisplayAlert("Duplikaadi viga", $"Teine riik nimega '{_nimiSisend.Text}' on juba olemas.", "OK");
                return;
            }

            _valitudRiik.Nimi = _nimiSisend.Text;
            _valitudRiik.Pealinn = _pealinnSisend.Text;
            
            if (_praeguneLippSource != null)
                _valitudRiik.Lipp = _praeguneLippSource;
                
            _valitudRiik.Rating = (int)_hinnangStepper.Value;
            _valitudRiik.DevelopmentLevel = _arenguTaseLiugur.Value;
            _valitudRiik.VisitDate = _külastusKuuPäev.Date ?? DateTime.Now;
            _valitudRiik.Lisainfo = _infoToimetaja.Text;

            RefreshList();
        }

        private async void OnDeleteSelectedClicked(object? sender, EventArgs e)
        {
            if (_valitudRiik == null)
            {
                await DisplayAlert("Viga", "Palun vali nimekirjast riik, mida kustutada.", "OK");
                return;
            }

            bool answer = await DisplayAlert("Kinnita kustutamine", $"Kas oled kindel, et soovid kustutada '{_valitudRiik.Nimi}'?", "Jah", "Ei");
            if (answer)
            {
                _kõikRiigid.Remove(_valitudRiik);
                _riikideKogum.Remove(_valitudRiik);
                ClearInputs();
            }
        }

        private void OnSwipeDeleteClicked(object? sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem?.CommandParameter is Riik riik)
            {
                _kõikRiigid.Remove(riik);
                _riikideKogum.Remove(riik);
                if (_valitudRiik == riik)
                {
                    ClearInputs();
                }
            }
        }

        private async void OnItemTapped(object? sender, ItemTappedEventArgs e)
        {
            if (e.Item is Riik riik)
            {
                _valitudRiik = riik;
                
                _nimiSisend.Text = riik.Nimi;
                _pealinnSisend.Text = riik.Pealinn;
                _hinnangStepper.Value = riik.Rating;
                _arenguTaseLiugur.Value = riik.DevelopmentLevel;
                _külastusKuuPäev.Date = riik.VisitDate;
                _infoToimetaja.Text = riik.Lisainfo;
                _praeguneLippSource = null;

                string infoMsg = $"Pealinn: {riik.Pealinn}\n" +
                                 $"Hinnang: {riik.Rating} / 5\n" +
                                 $"Arengutase: {riik.DevelopmentLevel:F1} / 10\n" +
                                 $"Külastuse kuupäev: {riik.VisitDate.ToShortDateString()}\n" +
                                 $"Lisainfo: {riik.Lisainfo}";
                await DisplayAlert(riik.Nimi, infoMsg, "OK");
            }
        }

        private void RefreshList()
        {
            _riikideList.ItemsSource = null;
            _riikideList.ItemsSource = _riikideKogum;
        }

        private void ClearInputs()
        {
            _nimiSisend.Text = string.Empty;
            _pealinnSisend.Text = string.Empty;
            _hinnangStepper.Value = 1;
            _arenguTaseLiugur.Value = 5;
            _külastusKuuPäev.Date = DateTime.Now;
            _infoToimetaja.Text = string.Empty;
            _praeguneLippSource = null;
            
            _valitudRiik = null;
            if (_riikideList != null)
            {
                _riikideList.SelectedItem = null;
            }
        }
    }
}
