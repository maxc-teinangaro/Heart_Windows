using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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



using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

using Windows.Phone.UI.Input;
using Windows.UI.Notifications;
using Windows.Data.Xml;
using Windows.Data.Xml.Dom;

using HEART.Views;
using HEART.Data;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace HEART.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Profile_Page : Page
    {
        string strMedicalCondition;
        bool bIsDefib = false;
        bool bIsCPR = true;
        string strMedication;
        string strMessage;
        int iActive;

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
            //strMedicalCondition = editMedical.
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
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    // New
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    arrMedicalCondition.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowSelectedV2.png"));
                }
                else
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    arrMedicalCondition.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowUnselected.png"));
                }
            }
            else if (((TextBlock)sender).Text == "Assistance Needed")
            {

                if (stackAssistNeeded.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    //
                    arrAssistance.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowSelectedV2.png"));
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    arrAssistance.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowUnselected.png"));
                }
            }
            else if (((TextBlock)sender).Text == "Medication")
            {
                if (stackMedication.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    //
                    arrMedication.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowSelectedV2.png"));
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    arrMedication.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowUnselected.png"));
                }
            }
            else if (((TextBlock)sender).Text == "Message")
            {
                if (stackMessage.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    //
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible; 
                    arrMessage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowSelectedV2.png"));
                    
                }
                else
                {
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    arrMessage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowUnselected.png"));
                }
            }
            else if (((TextBlock)sender).Text == "Activation Time")
            {
                if (stackActivationTime.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                {
                    stackMedicalCondition.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackAssistNeeded.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMedication.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    //
                    arrActivation.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowSelectedV2.png"));
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    stackActivationTime.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    arrActivation.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/PHPMenuArrowUnselected.png"));
                }
            }
           
        }

        private void slideActivationTime_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
           txtActiveTimeValue.Text = ((Slider)sender).Value.ToString();
        }

        private void ComboBox_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void btnSave_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await (new MessageDialog("Changes Saved")).ShowAsync();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void btnCancel_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Name_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Name.SelectAll();
        }

        private void btnSave_Tapped_1(object sender, TappedRoutedEventArgs e)
        {

        }

       
    }
}
