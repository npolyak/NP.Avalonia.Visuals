using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Layout;
using System;
using System.Linq;
using Avalonia.Controls.Presenters;
using NP.Avalonia.Visuals.ThemingAndL10N;

namespace NP.LocalizationPrototype
{
    public partial class MainWindow : Window
    {
        private ThemeLoader? _themeLoader;

        private Data _data = new Data("FirstDataTemplateText");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _themeLoader = 
                Application.Current.Resources.MergedDictionaries.OfType<ThemeLoader>().FirstOrDefault()!;

            this.GetObservable<bool>(IsEnglishProperty).Subscribe(OnIsEnglishChanged);

            ContentControl el1 = this.FindControl<ContentControl>("_elementI");
            ContentControl el2 = this.FindControl<ContentControl>("_elementII");

            el1.Content = _data;
            el2.Content = new Data("SecondDataTemplateText");

            Button flagButton = this.FindControl<Button>("FlagButton");
            flagButton.Click += ButtonFlag_Click;

            Button errorButton = this.FindControl<Button>("ErrorButton");
            errorButton.Click += ButtonError_Click;

            Button closeButton = this.FindControl<Button>("CloseButton");
            closeButton.Click += ButtonClose_Click;

            //Button changeUidButton = this.FindControl<Button>("ChangeUidButton");
            //changeUidButton.Click += ChangeUidButton_Click;
        }

        private void ChangeUidButton_Click(object? sender, RoutedEventArgs e)
        {
            _data.Uid = "SecondDataTemplateText";
        }

        private void OnIsEnglishChanged(bool obj)
        {
            string localeName = IsEnglish ? "English" : "Hebrew";

            _themeLoader.SelectedDictionaryId = localeName;
        }

        #region IsEnglish Styled Avalonia Property
        public bool IsEnglish
        {
            get { return GetValue(IsEnglishProperty); }
            set { SetValue(IsEnglishProperty, value); }
        }

        public static readonly StyledProperty<bool> IsEnglishProperty =
            AvaloniaProperty.Register<MainWindow, bool>
            (
                nameof(IsEnglish),
                true
            );
        #endregion IsEnglish Styled Avalonia Property


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public int Uid
        {
            get { return 4; }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            BeginMoveDrag(e);
        }

        private void ButtonFlag_Click(object sender, RoutedEventArgs e)
        {
            IsEnglish = !IsEnglish;
        }

        private void ButtonError_Click(object sender, RoutedEventArgs e)
        {
            string message = null, closeWindowStr = null;
            if (_themeLoader.TryGetResource("NotEnoughMemory", out object result))
            {
                message = result.ToString();
            }

            if (_themeLoader.TryGetResource("CloseWindowMessage", out result))
            {
                closeWindowStr = result.ToString();
            }

            var window = CreateSampleWindow(message, closeWindowStr);

            window.ShowDialog(this);
        }

        private Window CreateSampleWindow(string msg, string closeWindowStr)
        {
            Button button;

            var window = new Window
            {
                Height = 100,
                Width = 300,
                Content = new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = msg, HorizontalAlignment = HorizontalAlignment.Center},
                        (button = new Button
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Content = closeWindowStr
                        })
                    }
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            button.Click += (_, __) => window.Close();

            return window;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
