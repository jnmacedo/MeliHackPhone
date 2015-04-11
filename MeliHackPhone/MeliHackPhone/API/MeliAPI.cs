using MeliHackPhone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MeliHackPhone.API
{
    public class MeliAPI
    {
        public static String getAllCategoryItems(String categoryId)
        {
            String jsonResponse = "";

            String url = "https://api.mercadolibre.com/sites/MLU/search?category=" + categoryId;

            Task<String> response = MeliAPI.getRestCall(url);

            jsonResponse = response.ToString();

            return jsonResponse;
        }

        public static String getCategoryInformationAndChildrenCategories(String categoryId)
        {
            String jsonResponse = "";

            String url = "https://api.mercadolibre.com/categories/" + categoryId;

            Task<String> response = MeliAPI.getRestCall(url);

            jsonResponse = response.ToString();

            return jsonResponse;
        }

        private static async Task<String> getRestCall(String url)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await client.SendAsync(request);
            String responseData = await response.Content.ReadAsStringAsync();

            return responseData;
        }

        private CategoryInfo parseCategoryJSON(String categoryJSON)
        {
            CategoryInfo ci = new CategoryInfo();
            return ci;
        }

        private List<ServiceInfo> parseServiceInfoJSON(String serviceInfoJSON)
        {
            List<ServiceInfo> servicesInCategory = new List<ServiceInfo>();
            return servicesInCategory;
        }

        
    }
}
