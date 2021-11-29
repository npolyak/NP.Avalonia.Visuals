using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Avalonia.Visuals.ThemingAndL10N;

namespace NP.ThemingPrototype
{
    public partial class MainWindow : CustomWindow
    {
        ThemeLoader _lightDarkThemeLoader;
        ThemeLoader _accentThemeLoader;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _lightDarkThemeLoader = 
                Application.Current.Resources.GetThemeLoader("LightDarkThemeLoader")!;

            _accentThemeLoader =
                Application.Current.Resources.GetThemeLoader("AccentThemeLoader")!;

            Button button = this.FindControl<Button>("ChangeThemeButton");
            
            button.Click += Button_Click;
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            _lightDarkThemeLoader.SwitchTheme();

            if (_lightDarkThemeLoader.SelectedThemeId == "Light")
            {
                _accentThemeLoader.SelectedThemeId = "DarkBlue";
            }
            else
            {
                _accentThemeLoader.SelectedThemeId = "LightBlue";
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
