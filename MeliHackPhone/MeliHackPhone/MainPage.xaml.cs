using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MeliHackPhone.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Phone.UI.Input;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MeliHackPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public List<CategoryChildrenInfo> categories { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            // Get all Initial service categories.
            this.getCategoryInformationAndChildrenCategories("MLU1540");
        }

        public void getCategoryInformationAndChildrenCategories(String categoryId)
        {
            String url = "https://api.mercadolibre.com/categories/" + categoryId;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(url));
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
        }


        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {

                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult))
                {
                    using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                    {
                        string results = httpwebStreamReader.ReadToEnd();
                        //execute UI stuff on UI thread.
                        this.parseCategoryJSON(results);
                    }
                }
            }
            catch(Exception exc)
            {
                Debug.WriteLine("Exc: " + exc.Message);
            }
        }

        // Get All items from category
        public void getAllCategoryItems(String categoryId)
        {
            String url = "https://api.mercadolibre.com/sites/MLU/search?category=" + categoryId;
            //String url = "https://api.mercadolibre.com/sites/MLU/search?category=MLU9028";
        

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(url));
            request.BeginGetResponse(new AsyncCallback(ReadWebRequestSearchCallback), request);
        }

        private void ReadWebRequestSearchCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult))
            {
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    string results = httpwebStreamReader.ReadToEnd();
                    //execute UI stuff on UI thread.
                    List<ServiceInfo> si = this.parseServiceInfoJSON(results);
                }
            }
        }

        private void parseCategoryJSON(String categoryJSON)
        {
            CategoryInfo ci = JsonConvert.DeserializeObject<CategoryInfo>(categoryJSON.ToString());
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    this.categoriesListView.ItemsSource = ci.Children_categories;
                    this.progressRing.IsActive = false;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine("Exc: " + exc.Message);
                }
            });
            
            
        }

        private List<ServiceInfo> parseServiceInfoJSON(String serviceInfoJSON)
        {
            List<ServiceInfo> servicesInCategory = new List<ServiceInfo>();

            JObject services = JObject.Parse(serviceInfoJSON);
            IList<JToken> listOfServices = services["results"].Children().ToList();
           
            foreach (JToken result in listOfServices)
            {
                ServiceInfo newServiceInfo = JsonConvert.DeserializeObject<ServiceInfo>(result.ToString());
                servicesInCategory.Add(newServiceInfo);
            }

            return servicesInCategory;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Services), "https://api.mercadolibre.com/sites/MLU/search?category=MLU9028");
        }

        

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        private void categoriesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var category = (CategoryChildrenInfo)e.ClickedItem as CategoryChildrenInfo;

            String url = "https://api.mercadolibre.com/sites/MLU/search?category=" + category.ID;
            Frame.Navigate(typeof(Services), url);
        }

        private void searchImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            String url = "https://api.mercadolibre.com/sites/MLU/search?category=MLU1540&q=" + txbSearch.Text.ToString();
            Frame.Navigate(typeof(Services), url);
        }
    }
}
