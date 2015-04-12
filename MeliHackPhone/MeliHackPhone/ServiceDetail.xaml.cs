using MeliHackPhone.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MeliHackPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServiceDetail : Page
    {
        public ServiceDetail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var selectedService = e.Parameter as ServiceInfo;
            stackServiceDetail.DataContext = selectedService;
            
            if (!String.IsNullOrWhiteSpace(selectedService.Location.Latitude) && !String.IsNullOrWhiteSpace(selectedService.Location.Longitude))
            {
                mapDetail.Visibility = Windows.UI.Xaml.Visibility.Visible;
                double latitud = Double.Parse(selectedService.Location.Latitude);
                double longitude = Double.Parse(selectedService.Location.Longitude);
                Geopoint geo = new Geopoint(new BasicGeoposition { Latitude = latitud, Longitude = longitude });
                SetLocations(geo);
            }
            else
            {
                mapDetail.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void SetLocations(Geopoint servicePosition)
        {
            MapIcon mapLocation = new MapIcon();

            mapLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/service.png"));
            mapLocation.Location = servicePosition;
            mapLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);
            
            //await mapDetail.TrySetViewAsync(servicePosition, 15D);
            Geolocator locator = new Geolocator();
            Geoposition currentPostition = await locator.GetGeopositionAsync();
            Geopoint currentPoint = new Geopoint(new BasicGeoposition { Latitude = currentPostition.Coordinate.Latitude, Longitude = currentPostition.Coordinate.Longitude });

            MapIcon currentLocation = new MapIcon();
            currentLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/own.png"));
            currentLocation.Location = currentPoint;
            currentLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);

            


            // Get the route between the points.
            MapRouteFinderResult routeResult =
            await MapRouteFinder.GetDrivingRouteAsync(
              currentPoint,
              servicePosition,
              MapRouteOptimization.Time,
              MapRouteRestrictions.None,
              290);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                MapRouteView routeView = new MapRouteView(routeResult.Route);
                mapDetail.Routes.Add(routeView);

                await mapDetail.TrySetViewBoundsAsync(routeResult.Route.BoundingBox, null, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
            }

            mapDetail.MapElements.Add(mapLocation);
            mapDetail.MapElements.Add(currentLocation);
        }
    }
}
