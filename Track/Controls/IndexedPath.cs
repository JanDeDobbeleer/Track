using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Track.Controls
{
    public class IndexedPath:Path
    {
        public static readonly DependencyProperty SelectedIndexBindingProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(IndexedPath), new PropertyMetadata(OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexBindingProperty); }
            set { SetValue(SelectedIndexBindingProperty, value); }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((IndexedPath)d).Fill = ((int)e.NewValue == ((IndexedPath)d).Index) ? new SolidColorBrush(Colors.White) : (SolidColorBrush)Application.Current.Resources["InactiveBrush"];
        }

        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                Fill = (value == SelectedIndex) ? new SolidColorBrush(Colors.White) : (SolidColorBrush)Application.Current.Resources["InactiveBrush"];
            }
        }
    }
}
