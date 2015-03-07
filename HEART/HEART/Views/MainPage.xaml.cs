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


namespace HEART
{
    public sealed partial class MainPage : Page
    {
        public static string strCurrentDisplay = "---";
        private bool IsOn = false;
        private bool IsNavToIncidents = false;

        private StackPanel stack;
        private Popup pop;
        private IReadOnlyList<PeerInformation> listPeers;
        private Button btn;

        private bool bIsSuccess;

        public MainPage()
        {
            this.InitializeComponent();


            blkHeartRate.Text = strCurrentDisplay;

            // Connection protocool
            IsOn = false;

            // Set PeerFinder Event Handlers
            PeerFinder.ConnectionRequested += PeerFinder_ConnectionRequested;
            PeerFinder.TriggeredConnectionStateChanged += PeerFinder_TriggeredConnectionStateChanged;

            // TODO: Figure out automated call functionality
            // Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI("111", "Emergency");

            this.NavigationCacheMode = NavigationCacheMode.Required;

        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            if (bMockIsActive)
            {
                blkHeartRate.Text = await Task.Run(() => SvcManager.UpdateHeartRate());
            }

            Image img = imgIncident;

            string strHeartOn = @"ms-appx:///Assets/ic_recent_incidents.png";
            string strHeartOff = @"ms-appx:///Assets/ic_recent_incidents_bg.png";

            // Invert bool flag
            IsNavToIncidents = !IsNavToIncidents;

            // Alt button state
            string uriString = IsNavToIncidents ? strHeartOn : strHeartOff;
            img.Source = new BitmapImage(new Uri(uriString));

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }


        #region Peer_Finder_Events

        private async void PeerFinder_TriggeredConnectionStateChanged(object sender, TriggeredConnectionStateChangedEventArgs args)
        {

            MessageDialog m;

            if (args.State == TriggeredConnectState.PeerFound)
            {
                m = new MessageDialog("The device has been found");
                await m.ShowAsync();
            }
            else if (args.State == TriggeredConnectState.Connecting)
            {
                m = new MessageDialog("In Progress. . .");
                await m.ShowAsync();
            }
            else if (args.State == TriggeredConnectState.Completed)
            {
                m = new MessageDialog("Device connection completed");
                await m.ShowAsync();
            }
            else if (args.State == TriggeredConnectState.Failed)
            {
                m = new MessageDialog("Device connection failed");
                await m.ShowAsync();
            }
            else
            {
                m = new MessageDialog("Hmmm. . .");
                await m.ShowAsync();
            }
        }

        private async void PeerFinder_ConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            // args.PeerInformation is Peer Information
            StreamSocket soc = await PeerFinder.ConnectAsync(args.PeerInformation);
        }

        #endregion


        private async void Connect(PeerInformation _peerInfo)
        {
            StreamSocket soc = await PeerFinder.ConnectAsync(_peerInfo);
        }


        #region Button_Press_Events

        private async void btnSwitchDevice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //btn = ((Button)sender); // For TextBlock change the children collection index to 1
            Image img = (Image)sender;

            string strHeartOn = @"ms-appx:///Assets/ic_main_screen_on_off_on.png";
            string strHeartOff = @"ms-appx:///Assets/ic_main_screen_on_off_off.png";

            // Invert bool flag
            IsOn = !IsOn;

            // TODO: Remove Mock
            bMockIsActive = IsOn;

            // Alt button state
            string uriString = IsOn ? strHeartOn : strHeartOff;
            img.Source = new BitmapImage(new Uri(uriString));

            if (IsOn)
            {
                #region Connect_to_Polar_device_and_read_info


                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                listPeers = await PeerFinder.FindAllPeersAsync();



                if (listPeers.Count > 0)
                {
                    pop = new Popup();
                    stack = new StackPanel();

                    TextBlock header = new TextBlock();
                    header.Text = "Select a Heart monitor device to start the app:";
                    header.FontSize = 18;

                    //  SolidColorBrush colBrush = new SolidColorBrush(Windows.UI.Colors.Black)
                    stack.Background = new SolidColorBrush(Windows.UI.Colors.BlanchedAlmond);
                    stack.Children.Add(header);

                    RadioButton radioMockData = new RadioButton();
                    radioMockData.Content = "Mock Data";
                    radioMockData.GroupName = "Select Heart Monitor";
                    radioMockData.Checked += RadioButton_Checked;

                    stack.Children.Add(radioMockData);

                    for (int i = 0; i < listPeers.Count; ++i)
                    {
                        RadioButton radioEntry = new RadioButton();
                        radioEntry.Content = listPeers[i].DisplayName;
                        radioEntry.GroupName = "Select Heart Monitor";
                        radioEntry.Checked += RadioButton_Checked;

                        stack.Children.Add(radioEntry);
                    }

                    pop.Child = stack;
                    pop.Margin = new Thickness(44, 264, 36, 150);
                    pop.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    pop.IsOpen = true;
                }
                else
                {
                    MessageDialog m = new MessageDialog("No Devices");
                    await m.ShowAsync();
                }
                #endregion
            }
            else
            {
                // Shut down mock data
                blkHeartRate.Text = "---";
                blkAvgHeartBeat.Text = "Average Heart Rate: ---";
                SvcManager.Reset();
                // Discontinue any background tasks
                foreach (KeyValuePair<System.Guid, Windows.ApplicationModel.Background.IBackgroundTaskRegistration> o in BackgroundTaskRegistration.AllTasks)
                {
                    o.Value.Unregister(true);
                }
            }

        }

        private void btnProfile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Profile_Page));
            //this.Frame.Navigate(typeof(Incident_Log));
        }

        private void btnIncidentLog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = (Image)sender;

            string strHeartOn = @"ms-appx:///Assets/ic_recent_incidents.png";
            string strHeartOff = @"ms-appx:///Assets/ic_recent_incidents_bg.png";

            // Invert bool flag
            IsNavToIncidents = !IsNavToIncidents;

            // Alt button state
            string uriString = IsNavToIncidents ? strHeartOn : strHeartOff;
            img.Source = new BitmapImage(new Uri(uriString));

            this.Frame.Navigate(typeof(Incident_Log));
        }

        #endregion

        private TestService SvcManager = new TestService();
        private bool bMockIsActive = false;

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listPeers.Count; ++i)
            {
                if (listPeers[i].DisplayName == ((RadioButton)sender).Content.ToString())
                {
                    // TODO: connect with Polar strap.
                    bIsSuccess = false;

                    string strException = "";
                    // Do Stuff. . .
                    try
                    {
                        StreamSocket socket = new StreamSocket();
                        await socket.ConnectAsync(listPeers[i].HostName, "0");

                        bIsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        strException = ex.GetType().ToString() + ": " + ex.Message;
                    }

                    // Display the exception if appliable
                    if (strException.Count() > 0)
                    {
                        MessageDialog m = new MessageDialog(strException);
                        await m.ShowAsync();
                    }

                    // It worked!!!
                    if (bIsSuccess)
                    {
                        MessageDialog mSuccess = new MessageDialog("You have connected to " + listPeers[i].DisplayName);
                        await mSuccess.ShowAsync();

                        break;
                    }
                    else
                    {
                        // Undo the activation, and prompt the user with a fail
                        MessageDialog m = new MessageDialog(listPeers[i].DisplayName + " failed to connect.");
                        await m.ShowAsync();

                        Image img = (Image)(btn.ContentTemplateRoot as StackPanel).Children[0];

                        IsOn = false;
                        string strHeartOff = @"ms-appx:///Assets/ic_main_screen_on_off_off.png";

                        // Alt button state
                        string uriString = strHeartOff;
                        img.Source = new BitmapImage(new Uri(uriString));
                        break;
                    }
                }
                // Mock Data
                else if (((RadioButton)sender).Content.ToString().Contains("Mock"))
                {
                    blkHeartRate.Text = await Task.Run(() => SvcManager.UpdateHeartRate());
                    DispatcherTimerSetup();
                    bMockIsActive = true;

                }
            }

            pop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtHeartRate_TextChanged(object sender, TextChangedEventArgs e)
        {
        }


        #region Timer
        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset time;

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_TickEvent;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            //IsEnabled defaults to false
            startTime = DateTimeOffset.Now;
            lastTime = startTime;

            dispatcherTimer.Start();
            //IsEnabled should now be true after calling start
        }

        public void dispatcherTimer_TickEvent(object sender, object e)
        {
            // Event Logic. . .
            blkHeartRate.Text = SvcManager.UpdateHeartRate();
            blkAvgHeartBeat.Text = "Average Heart Beat: " + SvcManager.UpdateAvg();

            // Algorithm

            // Result
            if(true)
            {
                Incident_Log.CreateLogEntry(new Incident(DateTime.Now, int.Parse(blkHeartRate.Text), eClassification.LOW));
            }
            
            // Update timer
            time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }
        #endregion

    }

    public class TestService
    {
        private Random r = new Random(0);
        int iTotal = 0;
        int iCount = 0;

        public void Reset()
        {
            iTotal = 0;
            iCount = 0;
        }

        public string UpdateHeartRate()
        {
            // 240 = approx mock read for a 250 bpm 
            // 706 = approx mock read for a xxx bpm

            int i = r.Next(240, 706);

            ++iCount;
            iTotal += (60000 / i);

            return (60000 / i).ToString();
        }


        public int GetTotal() { return iTotal; }
        public int GetCount() { return iCount; }

        public int UpdateAvg()
        {
            int iAvg = iTotal / iCount;

            if (iCount >= 30)
            {
                iTotal = iAvg;
                iCount = 1;
            }

            return (iAvg);
        }
        public int GetGraphYPos(int _iHeartRate)
        {
            return (0);
        }
    }

}

/***  End of Code (Snippets down below. . .)  ***/

