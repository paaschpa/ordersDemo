using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrdersDemo.Models
{
    public class ArcherItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Score { get; set; }
    }

    public class ArcherItems
    {
        public static IEnumerable<ArcherItem> Get()
        {
            return new List<ArcherItem>
                {
                    new ArcherItem {Id = 1, Name = "Archer Hat", Price = 18.99m, Score = 2},
                    new ArcherItem {Id = 2, Name = "Isis Mug", Price = 13.95m, Score = 1},
                    new ArcherItem {Id = 3, Name = "Cardboard Stand-Up", Price = 34.95m, Score = 5},
                    new ArcherItem {Id = 4, Name = "Suit Shirt", Price = 29.95m, Score = 3},
                    new ArcherItem {Id = 5, Name = "Excelsior Shirt", Price = 29.95m, Score = 3},
                    new ArcherItem {Id = 6, Name = "Isis Sticker", Price = 4.95m, Score = 4},
                };
        }
    }
}