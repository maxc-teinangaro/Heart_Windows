﻿using System;
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
using HEART.Data;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace HEART.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IncidentLog : Page
    {
        //private static ScrollViewer s_ScrollViewer;
        private static StackPanel s_StackPanel = new StackPanel();

        private static SolidColorBrush s;
        private static SolidColorBrush alert;
        private static SolidColorBrush brushTxtBlock;

        public IncidentLog()
        {
            this.InitializeComponent();

            if (s_StackPanel == null)
            {
                s_StackPanel = new StackPanel();
            }
            if (scrollIncidentLog.Content == null)
            {
                scrollIncidentLog.Content = s_StackPanel;
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (((Image)sender).Name == "Day")
            {
                txtHeader.Text = "Today's Incidents";
            }
            else if (((Image)sender).Name == "Week")
            {
                txtHeader.Text = "This Week's Incidents";
            }
            else if (((Image)sender).Name == "Month")
            {
                txtHeader.Text = "This Month's Incidents";
            }

        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (scrollIncidentLog.Content == null)
            //{
            //    scrollIncidentLog.Content = s_StackPanel;

            //    s = new SolidColorBrush();
            //    s.Color = Windows.UI.Color.FromArgb(255, 255, 255, 255);

            //    alert = new SolidColorBrush();
            //    alert.Color = Windows.UI.Color.FromArgb(255, 0, 255, 0);

            //    brushTxtBlock = new SolidColorBrush();
            //    brushTxtBlock.Color = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            //}
        }


        public static void CreateLogEntry(Incident _inNew)
        {

            // Next, the data. . .
            DateTime dt = new DateTime();
            int iHeartRate = 0;
            eClassification eClass = eClassification.INVALID;

            _inNew.GetAll(ref dt, ref iHeartRate, ref eClass);


            s = new SolidColorBrush();
            s.Color = Windows.UI.Color.FromArgb(255, 255, 255, 255);

            alert = new SolidColorBrush();
            alert.Color = Windows.UI.Color.FromArgb(255, 0, 255, 0);

            if (eClass == eClassification.LOW)
            {
                alert.Color = Windows.UI.Color.FromArgb(255, 0, 255, 0);
            }
            else if (eClass == eClassification.MEDIUM)
            {

                alert.Color = Windows.UI.Color.FromArgb(255, 255, 102, 0);
            }
            else if (eClass == eClassification.HIGH)
            {
                
            alert.Color = Windows.UI.Color.FromArgb(255, 255, 0, 0);
            }

            brushTxtBlock = new SolidColorBrush();
            brushTxtBlock.Color = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            Grid grid = new Grid();

            grid.Background = s; grid.Height = 108; grid.Width = 400;

            // Create Rect
            Windows.UI.Xaml.Shapes.Rectangle r = new Windows.UI.Xaml.Shapes.Rectangle();

            // Fill Color in Rect
            r.Fill = alert;
            r.Stretch = Stretch.Fill;
            r.Margin = new Thickness(0, 10, 252, 10);
            r.RenderTransformOrigin = new Point(0.5, 0.5);

            // Finally, add
            grid.Children.Add(r);


            string strDate = string.Format("Date: {0}/{1}/{2} at {3}",
                dt.Day.ToString("00"), dt.Month.ToString("00"), dt.Year.ToString("00"), dt.Hour + ":" + dt.Minute + " " + dt.ToString("tt"));
            string strHeartRate = string.Format("Heart Rate: {0}", iHeartRate);
            string strClassification = string.Format("Classification: {0}", eClass);

            TextBlock txtDate = new TextBlock();
            txtDate.Text = strDate;
            txtDate.Foreground = brushTxtBlock;
            txtDate.FontSize = 18;

            TextBlock txtHeartRate = new TextBlock();
            txtHeartRate.Text = strHeartRate;
            txtHeartRate.Foreground = brushTxtBlock;
            txtHeartRate.FontSize = 18;

            TextBlock txtClassification = new TextBlock();
            txtClassification.Text = strClassification;
            txtClassification.Foreground = brushTxtBlock;
            txtClassification.FontSize = 18;

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Vertical;

            SolidColorBrush brushIncidentBlock = new SolidColorBrush();
            brushIncidentBlock.Color = Windows.UI.Color.FromArgb(255, 240, 224, 224);
            stack.Background = brushIncidentBlock;

            stack.Margin = new Thickness(45, 10, 0, 10);

            stack.Children.Add(txtDate);
            stack.Children.Add(txtHeartRate);
            stack.Children.Add(txtClassification);

            grid.Children.Add(stack);


            s_StackPanel.Children.Add(grid);
        }

        private void scrollIncidentLog_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }


      
    }
}
