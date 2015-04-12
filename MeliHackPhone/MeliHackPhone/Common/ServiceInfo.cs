using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeliHackPhone.Common
{
    public class ServiceInfo
    {
        public String ID { get; set; }
        public String Title { get; set; }
        public String Subtitle { get; set; }
        public String Price { get; set; }
        public String PricePrint { 
            get{
                if (String.IsNullOrWhiteSpace(Price))
                    return "Precio: A convenir";
                else
                    return "Precio: $" + Price;
        } }
        public String Permalink { get; set; }
        public String Thumbnail { get; set; }
        public SellerInfo Seller { get; set; }
        public SellerAddress Seller_Address { get; set; }
        public Location Location { get; set; }
    }
}
