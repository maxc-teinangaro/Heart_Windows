using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace HEART.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Profile_Page : Page
    {
        public Profile_Page()
        {
            this.InitializeComponent();

            txtActiveTimeValue.Text = slideActivationTime.Value.ToString();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnCancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (((TextBlock)sender).Text == "Medical Condition")
            {
                if (stackMedicalCondition.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            else if (((TextBlock)sender).Text == "Assistance Needed")
            {

                if (stackAssistNeeded.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            else if (((TextBlock)sender).Text == "Medication")
            {
                if (stackMedication.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            else if (((TextBlock)sender).Text == "Message")
            {
                if (stackMessage.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            else if (((TextBlock)sender).Text == "Activation Time")
            {
                if (stackActivationTime.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
           
        }

        private void slideActivationTime_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            txtActiveTimeValue.Text = ((Slider)sender).Value.ToString();
        }
    }
}
