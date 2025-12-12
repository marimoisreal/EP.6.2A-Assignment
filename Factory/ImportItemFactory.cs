using System.Text.Json;
using EP._6._2A_Assignment.Interfaces;
using EP._6._2A_Assignment.Models;

namespace EP._6._2A_Assignment.Factories
{
    public class ImportItemFactory
    {
        public List<IItemValidating> Create(string json)
        {
            var items = new List<IItemValidating>();

            var rawItems = JsonSerializer.Deserialize<List<ImportDTO>>(json);

            foreach (var item in rawItems)
            {
                if (item.Type == "Restaurant")
                {
                    items.Add(new Restaurant
                    {
                        Id = item.GuidId ?? Guid.NewGuid(),
                        Name = item.Name,
                        ownerEmailAdress = item.ownerEmailAdress,
                        Status = "Pending"
                    });
                }
                else if (item.Type == "MenuItem")
                {
                    items.Add(new MenuItem
                    {
                        Id = item.GuidId ?? Guid.NewGuid(),
                        Title = item.Title,
                        Price = item.Price ?? 0,
                        RestaurantId = item.RestaurantId ?? 0,
                        Status = "Pending"
                    });
                }
            }

            return items;
        }
    }

    // TDO for reading import Json
    public class ImportDTO
    {
        public string Type { get; set; }

        public int? Id { get; set; }
        public Guid? GuidId { get; set; }

        public string Name { get; set; }
        public string ownerEmailAdress { get; set; } 

        public string Title { get; set; }
        public double? Price { get; set; }
        public int? RestaurantId { get; set; }
    }
}
