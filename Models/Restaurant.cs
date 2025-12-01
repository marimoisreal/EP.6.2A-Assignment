using EP._6._2A_Assignment.Interfaces;

namespace EP._6._2A_Assignment.Models
{
    public class Restaurant : IItemValidating
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ownerEmailAdress {  get; set; }

        public string Status { get; set; }

        public List<string> GetValidators()
        {
            return new List<string>
            {

                "admin@site@.com"

            };
        }
        public string GetCardPartial()
        {
            return "_RestaurantCard";
        }
    }
}
