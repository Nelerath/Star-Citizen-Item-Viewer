
using System.Text.RegularExpressions;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Item
    {
        public string Id { get; set; }
        private string _name { get; set; }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = Regex.Replace(value, @"[^\u0000-\u007F]+", string.Empty);
            }
        }
        public int Size { get; set; }
        public string Filename { get; set; }
    }
}
