using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace Track.View
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
        }

        //Hack this focus as I can't get Focusmanager to work here
        private void Search_OnLoaded(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox.Focus();
        }
    }
}