using System.Collections.Generic;
using System.Linq;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Ship
    {
        public List<Weapon> Weapons { get; set; }
        public List<Cooler> Coolers { get; set; }
        public List<Reactor> Reactors { get; set; }
        public List<Shield> Shields { get; set; }

        public int NumberOfPips { get; set; }
        public decimal AlphaDamage { get; set; }
        public decimal DamagePerSecond { get; set; }

        public decimal PowerOutput { get; set; }

        public decimal CoolingPerSecond { get; set; }

        public void Recalculate()
        {
            NumberOfPips = Weapons.Select(x => x.Speed).Distinct().Count();
            AlphaDamage = 0;
            DamagePerSecond = 0;
            foreach (var item in Weapons)
            {
                AlphaDamage += item.DamageTotal;
                DamagePerSecond += item.DamageTotal * item.Firerate;
            }
        }

        public Ship()
        {
            Weapons = new List<Weapon>();
            Coolers = new List<Cooler>();
            Reactors = new List<Reactor>();
            Shields = new List<Shield>();
        }

        //public Ship(List<Weapon> Weapons, List<Cooler> Coolers, List<Reactor> Reactors, List<Shield> Shields)
        //{
        //    this.Weapons = Weapons;
        //    this.Coolers = Coolers;
        //    this.Reactors = Reactors;
        //    this.Shields = Shields;
        //    Recalculate();
        //}
    }
}
