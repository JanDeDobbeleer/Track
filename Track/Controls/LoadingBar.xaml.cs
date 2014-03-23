using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Track.Controls
{
    public partial class LoadingBar : UserControl
    {
        public static readonly DependencyProperty IsIndeterminateBindingProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(LoadingBar), new PropertyMetadata(OnIsIndeterminateChanged));

        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateBindingProperty); }
            set { SetValue(IsIndeterminateBindingProperty, value); }
        }

        private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingBar)d).ProgressBar.IsIndeterminate = (bool)e.NewValue;
        }

        public static readonly DependencyProperty ForegroundBindingProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(LoadingBar), new PropertyMetadata(OnForegroundChanged));

        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundBindingProperty); }
            set { SetValue(ForegroundBindingProperty, value); }
        }

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingBar)d).ProgressBar.Foreground = (Brush)e.NewValue;
            ((LoadingBar)d).TextBlock.Foreground = (Brush)e.NewValue;
        }

        public static readonly DependencyProperty TextBindingProperty = DependencyProperty.Register("Text", typeof(string), typeof(LoadingBar), new PropertyMetadata(OnTextChanged));
        
        public string Text
        {
            get { return (string)GetValue(TextBindingProperty); }
            set { SetValue(TextBindingProperty, value); }
        }


        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LoadingBar)d).TextBlock.Text = (string)e.NewValue;
        }

        public LoadingBar()
        {
            InitializeComponent();
        }
    }
}
