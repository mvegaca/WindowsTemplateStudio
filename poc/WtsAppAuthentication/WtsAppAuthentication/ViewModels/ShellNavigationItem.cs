using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using WtsAppAuthentication.Helpers;

namespace WtsAppAuthentication.ViewModels
{
    public enum ShellNavigationItemType
    {
        Page = 0,
        Action = 1
    }

    public class ShellNavigationItem : Observable
    {
        public ShellNavigationItemType ItemType { get; set; }

        public string Label { get; set; }

        public Symbol Symbol { get; set; }

        public Type PageType { get; set; }

        public Action Action { get; set; }

        private Visibility _selectedVis = Visibility.Collapsed;

        public Visibility SelectedVis
        {
            get { return _selectedVis; }

            set { Set(ref _selectedVis, value); }
        }

        public char SymbolAsChar
        {
            get { return (char)Symbol; }
        }

        private readonly IconElement _iconElement = null;

        public IconElement Icon
        {
            get
            {
                var foregroundBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("SelectedForeground"),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };

                if (_iconElement != null)
                {
                    BindingOperations.SetBinding(_iconElement, IconElement.ForegroundProperty, foregroundBinding);

                    return _iconElement;
                }

                var fontIcon = new FontIcon { FontSize = 16, Glyph = SymbolAsChar.ToString() };

                BindingOperations.SetBinding(fontIcon, IconElement.ForegroundProperty, foregroundBinding);

                return fontIcon;
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                Set(ref _isSelected, value);

                SelectedVis = value ? Visibility.Visible : Visibility.Collapsed;

                SelectedForeground = IsSelected
                    ? Application.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush
                    : GetStandardTextColorBrush();
            }
        }

        private SolidColorBrush _selectedForeground = null;

        public SolidColorBrush SelectedForeground
        {
            get { return _selectedForeground ?? (_selectedForeground = GetStandardTextColorBrush()); }

            set { Set(ref _selectedForeground, value); }
        }

        public ShellNavigationItem(string label, Symbol symbol, Action action)
            : this(label, action)
        {
            Symbol = symbol;
        }

        public ShellNavigationItem(string label, IconElement icon, Action action)
            : this(label, action)
        {
            _iconElement = icon;
        }

        public ShellNavigationItem(string label, Symbol symbol, Type pageType)
            : this(label, pageType)
        {
            Symbol = symbol;
        }

        public ShellNavigationItem(string label, IconElement icon, Type pageType)
            : this(label, pageType)
        {
            _iconElement = icon;
        }

        public ShellNavigationItem(string label, Type pageType)
        {
            Label = label;
            PageType = pageType;
            ItemType = ShellNavigationItemType.Page;
        }

        public ShellNavigationItem(string label, Action action)
        {
            Label = label;
            Action = action;
            ItemType = ShellNavigationItemType.Action;
        }

        public static ShellNavigationItem FromType<T>(string label, Symbol symbol)
            where T : Page
        {
            return new ShellNavigationItem(label, symbol, typeof(T));
        }

        public static ShellNavigationItem FromType<T>(string label, IconElement icon)
            where T : Page
        {
            return new ShellNavigationItem(label, icon, typeof(T));
        }

        private SolidColorBrush GetStandardTextColorBrush()
        {
            var brush = Application.Current.Resources["ThemeControlForegroundBaseHighBrush"] as SolidColorBrush;

            return brush;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
