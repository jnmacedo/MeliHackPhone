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
using MeliHackPhone.Common;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Storage.Streams;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(e.Parameter.ToString()));
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            

            Geolocator locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;

            Geoposition postition = null;
            try
            {
                postition = await locator.GetGeopositionAsync();
                Geopoint currentPosition = postition.Coordinate.Point;
                await mapServices.TrySetViewAsync(currentPosition, 15D);

                SetLocations(currentPosition, "", false);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize map");
            }
        }

        private void SetLocations(Geopoint currentPosition, string title, bool serviceLocation)
        {
            MapIcon mapLocation = new MapIcon();
            if (serviceLocation)
                mapLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/service.png"));
            else
                mapLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/own.png"));

            mapLocation.Title = title;
            mapLocation.Location = currentPosition;
            mapLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapServices.MapElements.Add(mapLocation);
        }

        private void LoadItems(List<ServiceInfo> services)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    listServices.ItemsSource = services;
                    foreach (var service in services)
                    {
                        if (!String.IsNullOrWhiteSpace(service.Location.Latitude) && !String.IsNullOrWhiteSpace(service.Location.Longitude))
                        {
                            double latitud = Double.Parse(service.Location.Latitude);
                            double longitude = Double.Parse(service.Location.Longitude);
                            Geopoint geo = new Geopoint(new BasicGeoposition { Latitude = latitud, Longitude = longitude });
                            SetLocations(geo, service.Title, true);
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("Failed to load items");
                }
            });
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var selectedService = (ServiceInfo)e.ClickedItem as ServiceInfo;
            Frame.Navigate(typeof(ServiceDetail), selectedService);
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult))
            {
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    string results = httpwebStreamReader.ReadToEnd();
                    //execute UI stuff on UI thread.
                    this.parseServiceInfoJSON(results);
                }
            }
        }

        private void parseServiceInfoJSON(String serviceInfoJSON)
        {
            List<ServiceInfo> servicesInCategory = new List<ServiceInfo>();

            JObject services = JObject.Parse(serviceInfoJSON);
            IList<JToken> listOfServices = services["results"].Children().ToList();

            foreach (JToken result in listOfServices)
            {
                ServiceInfo newServiceInfo = JsonConvert.DeserializeObject<ServiceInfo>(result.ToString());
                servicesInCategory.Add(newServiceInfo);
            }

            LoadItems(servicesInCategory);
        }

        
    }
}
