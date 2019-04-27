
using System.Text.RegularExpressions;

namespace Star_Citizen_Item_Viewer.Classes
{
    public enum Types
    {
        Weapon,
        Gun,
        Helmet,
        Torso,
        Arms,
        Legs,
    }

    public enum ItemGrades
    {
        A,
        B,
        C,
        D,
    }

    public enum ItemClassifications
    {
        Military,
        Civilian,
        Stealth,
        Industrial,
        Competition,
    }

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

        public Types Type { get; set; }
        public ItemGrades Grade { get; set; }
        public ItemClassifications Classification { get; set; }
    }
}
