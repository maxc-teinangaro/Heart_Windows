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


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            //if (bMockIsActive)
            //{
            //    blkHeartRate.Text = await Task.Run(() => SvcManager.UpdateHeartRate());
            //}

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
                    header.Text = "Select a Heart monitor device:";
                    header.FontSize = 24;

                    //  SolidColorBrush colBrush = new SolidColorBrush(Windows.UI.Colors.Black)

                    stack.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                    stack.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(240, 0, 0, 0));
                    stack.Children.Add(header);
                    stack.Margin = new Thickness(0, 120, 0, 60);

                    RadioButton radioMockData = new RadioButton();
                    radioMockData.Content = "Mock Data";
                    radioMockData.GroupName = "Select Heart Monitor";
                    radioMockData.Checked += RadioButton_Checked;

                    stack.Children.Add(radioMockData);

                    for (int i = 0; i < listPeers.Count; ++i)
                    {
                        RadioButton radioEntry = new RadioButton();
                        radioEntry.FontSize = 18;
                        radioEntry.Content = listPeers[i].DisplayName;
                        radioEntry.GroupName = "Select Heart Monitor";
                        radioEntry.Checked += RadioButton_Checked;

                        stack.Children.Add(radioEntry);
                    }

                    pop.Child = stack;
                   // pop.Margin = new Thickness(44, 264, 36, 150);
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
                GraphLine.Points.Clear();
                dispatcherTimer.Stop();
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
        }

        private void btnIncidentLog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Image img = (Image)sender;

            //string strHeartOn = @"ms-appx:///Assets/ic_recent_incidents.png";
            //string strHeartOff = @"ms-appx:///Assets/ic_recent_incidents_bg.png";

            //// Invert bool flag
            //IsNavToIncidents = !IsNavToIncidents;

            //// Alt button state
            //string uriString = IsNavToIncidents ? strHeartOn : strHeartOff;
            //img.Source = new BitmapImage(new Uri(uriString));

            this.Frame.Navigate(typeof(IncidentLog));
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

                        Image img = null;

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
                    //blkHeartRate.Text = await Task.Run(() => SvcManager.UpdateHeartRate());
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

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_TickEvent;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            var ttv = GraphLine.TransformToVisual(Window.Current.Content);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));
            GraphLine.Points.Add(screenCoords);

            iGraphPlotter = 0;
            GraphLine.Points.Clear();
            dispatcherTimer.Start();
        }

        byte iFlash = 255;
        int iTimer = 0;
        int iGraphPlotter = 0;


        int iMax = 240;
        int iMin = 40;
        int iChange = 10000000;

        public void dispatcherTimer_TickEvent(object sender, object e)
        {
            dispatcherTimer.Stop();
            // Event Logic. . .
            int iPrev = 0;
            int iDelta = 0;

            if (!int.TryParse(blkHeartRate.Text, out iPrev))
            {
                iPrev = 0;
            }

            blkHeartRate.Text = SvcManager.UpdateHeartRate();
            int iAvg = SvcManager.UpdateAvg();
            blkAvgHeartBeat.Text = "Average Heart Beat: " + iAvg.ToString();

            double iHeartRate = double.Parse(blkHeartRate.Text);
            iDelta = (int)iHeartRate - iPrev;

            double dStart = 200;
            double dStep = (double)(gridGraph.Width / 10);

            if(GraphLine.Points.Count > 10)
            {
                GraphLine.Points.Clear();
                iGraphPlotter = 0;
            }

            ++iGraphPlotter;
            GraphLine.Points.Add(new Point(dStart - (dStep * iGraphPlotter), dStart - iHeartRate));



            //GraphLine.Points.Insert(0, new Point(dStart - (dStep * iGraphPlotter), dStart - iHeartRate));
            //for (int i = 0; i < GraphLine.Points.Count; ++i)
            //{
            //    Point p = new Point(GraphLine.Points[i].X - 20 * i, GraphLine.Points[i].Y);
            //    GraphLine.Points[i] = p;
            //}

            
            // Algorithm
            AlertFunc((int)iHeartRate, iAvg, iDelta);

            dispatcherTimer.Start();
        }

        int iDelayTime = 15;
        int iHighCount = 0;

        private void AlertFunc(int iHeartRate, int _iAvg, int iDelta)
        {
            // Trigger the warning page
            bool bIsTriggered = ((_iAvg >= iMax) || (_iAvg <= iMin) || (iDelta >= iChange) || (iDelta <= -iChange));
            bool bIsModerate = ((iHeartRate >= iMax) || (iHeartRate <= iMin) || (iDelta >= iChange) || (iDelta >= 7) || (iDelta <= -7));
            bool bIsMinor = (iDelta >= 5) || (iDelta <= -5);

            eClassification eClass = eClassification.INVALID;

            if (bIsTriggered)
            {
                eClass = eClassification.HIGH;
                ++iHighCount;
            }
            else
            {
                eClass = bIsModerate ? eClassification.MEDIUM : eClass = bIsMinor ? eClass = eClassification.LOW : eClass = eClassification.INVALID;
                iHighCount = (iHighCount > 0) ? --iHighCount : iHighCount = 0;
            }

            if(eClass != eClassification.INVALID)
            {
                IncidentLog.CreateLogEntry(new Incident(DateTime.Now, iHeartRate, eClass));
            }

            if (iHighCount > 5 && DelayAlertGrid.Visibility == Windows.UI.Xaml.Visibility.Collapsed && 
                EmergencyAlertGrid.Visibility == Windows.UI.Xaml.Visibility.Collapsed && AssistanceGrid.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                DelayAlertGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            // Start the delay timer
            if (DelayAlertGrid.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                ++iTimer;
                bgFlash.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(125, iFlash, 0, 0));
            }
            else
            {
                // Reset if cancelled
                iTimer = 0;
            }

            // Assist screen
            // TODO: Trigger cloud functionality
            if (iTimer == iDelayTime)
            {
                iTimer = 0;

                DelayAlertGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                EmergencyAlertGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }
        #endregion

        private void btnCancelAlert_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DelayAlertGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }


        private void btnAssist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void btnCPR_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CPRInstructions));
        }

        private void btnAED_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AEDLocator));
        }

        private void btnAssist_Click(object sender, RoutedEventArgs e)
        {
            AssistanceGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            EmergencyAlertGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            iMax = 240;
            iMin = 40;
            iChange = 1;
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            iMax = 400;
            iMin = 0;
            iChange = 1000;
        }

        private void btnExit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AssistanceGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

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
            int iMinMockRate = 62;
            int iMaxMockRate = 102;

            int i = r.Next(iMinMockRate, iMaxMockRate);

            ++iCount;
            iTotal += i;

            return i.ToString();
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

