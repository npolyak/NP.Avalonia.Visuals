using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Avalonia.Visuals.ThemingAndL10N;

namespace NP.GridSplitterInOverlayWindowDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        #region IsOverlayWindowOpen Styled Avalonia Property
        public bool IsOverlayWindowOpen
        {
            get { return GetValue(IsOverlayWindowOpenProperty); }
            set { SetValue(IsOverlayWindowOpenProperty, value); }
        }

        public static readonly StyledProperty<bool> IsOverlayWindowOpenProperty =
            AvaloniaProperty.Register<MainWindow, bool>
            (
                nameof(IsOverlayWindowOpen)
            );
        #endregion IsOverlayWindowOpen Styled Avalonia Property

    }
}
