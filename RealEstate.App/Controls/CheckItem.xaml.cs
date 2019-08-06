using System.Windows;
using System.Windows.Controls;

namespace RealEstate.App.Controls
{
    /// <summary>
    /// Interaction logic for CheckItem.xaml
    /// </summary>
    public partial class CheckItem : UserControl
    {
        public CheckItem()
        {
            InitializeComponent();
        }

        public bool Success
        {
            get => (bool)GetValue(SuccessProperty);
            set => SetValue(SuccessProperty, value);
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>Identifies the <see cref="Value"/>Set or get Value to show in left-side</summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(CheckItem));

        /// <summary>Identifies the <see cref="Success"/>Set or get state of check item and show via red/green light</summary>
        public static readonly DependencyProperty SuccessProperty = DependencyProperty.Register(nameof(Success), typeof(bool), typeof(CheckItem));

        /// <summary>Identifies the <see cref="Title"/>Set or get Title of check item</summary>
        private static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(CheckItem));
    }
}