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
        public String PictureURL { get; set; }
        public Int32 AmountOfItemsInCategory { get; set; }
        public List<CategoryChildrenInfo> Childrens { get; set; }
    }
}
