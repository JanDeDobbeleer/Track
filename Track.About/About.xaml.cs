using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Windows.System;
using Localization.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Track.About
{
    public partial class About : PhoneApplicationPage
    {
        private StackPanel _licenses;

        public About()
        {
            InitializeComponent();
            AssignValues();
        }

        private void AssignValues()
        {
            try
            {
                GetVersionNumber();
                LoadLicense();
                HyperLinkButton.NavigateUri = new Uri("http://www.jan-joris.be");
                ReviewBlock.Xaml = @AppResources.AboutViewDisclaimer;
            }
            catch (Exception)
            {
                //TODO: handle this
            }
        }

        private void GetVersionNumber()
        {
            VersionText.Text = Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0] ?? AppResources.AboutViewUnknownAssembly;
        }

        private void LoadLicense()
        {
            if (_licenses != null)
                return;
            _licenses = new StackPanel();
            _licenses.Children.Add(new RichTextBox { Xaml = @AppResources.AboutViewiRail, Foreground = new SolidColorBrush(Colors.Black) });
            _licenses.Children.Add(ReturnHyperlinkButton());
            Sv1.Content = _licenses;
        }

        private HyperlinkButton ReturnHyperlinkButton()
        {
            var link = new HyperlinkButton
            {
                Content = "iRail",
                NavigateUri = new Uri("https://hello.irail.be/about/", UriKind.RelativeOrAbsolute),
                TargetName = "blank",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0),
                Style = HyperLinkStyle,
                Foreground = new SolidColorBrush(Colors.LightGray)
            };
            return link;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var s = ((ButtonBase)sender).Tag as string;
            switch (s)
            {
                case "Review":
                    var task = new MarketplaceReviewTask();
                    task.Show();
                    break;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask
            {
                Subject = "Feedback from Track",
                To = "trackapplication@outlook.com"
            };
            emailComposeTask.Show();
        }

        private async void UIElement_OnTap(object sender, GestureEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("zune:search?publisher=Jan Joris"));
        }
    }
}