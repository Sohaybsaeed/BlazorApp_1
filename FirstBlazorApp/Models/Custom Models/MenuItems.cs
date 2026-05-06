using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailChannel.Models.CustomModel
{
    public class MenuItems
    {
        public List<Items> menuitemlist { get; set; }
    }
    public class Items
    {
        public string ItemName { get; set; }
        public string Icon { get; set; }
        public List<SubMenuItems> SubMenuItems { get; set; }
    }
    public class SubMenuItems
    {
        public string submenuItem { get; set; }
        public string NavigationLink { get; set; }
    }
}
