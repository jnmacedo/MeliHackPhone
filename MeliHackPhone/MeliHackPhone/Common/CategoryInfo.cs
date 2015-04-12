using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeliHackPhone.Common
{
    public class CategoryInfo
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String Picture { get; set; }
        public Int32 Total_items_in_this_category { get; set; }
        public List<CategoryChildrenInfo> Children_categories { get; set; }
    }
}
