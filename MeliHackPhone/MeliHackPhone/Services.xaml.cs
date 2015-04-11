using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Services.Maps;
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MeliHackPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Services : Page
    {
        public Services()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Geolocator locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

            Geoposition postition = null;
            try
            {
                postition = await locator.GetGeopositionAsync();
                Geopoint currentPosition = postition.Coordinate.Point;
                //await mapServices.TrySetViewAsync(currentPosition, 15D);

                SetLocations(currentPosition);

                Geocode(currentPosition);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize map");
            }
        }

        private void SetLocations(Geopoint currentPosition)
        {
            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = currentPosition;
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = "Space Needle";
            mapServices.MapElements.Add(MapIcon1);
        }

        private async void Geocode(Geopoint currentPosition)
        {
            // Address or business to geocode.
            string addressToGeocode = "colombes 1366";

            // Nearby location to use as a query hint.
            BasicGeoposition queryHint = new BasicGeoposition();
            queryHint.Latitude = 47.643;
            queryHint.Longitude = -122.131;
            Geopoint hintPoint = new Geopoint(queryHint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than 3 results.
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAsync(
                                    addressToGeocode,
                                    currentPosition,
                                    3);

            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                SetLocations(result.Locations[0].Point);
                await mapServices.TrySetViewAsync(result.Locations[0].Point, 15D);
            }
        }


    }
}
