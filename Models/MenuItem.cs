using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EP._6._2A_Assignment.Models
{
    public class MenuItem
    {

        public Guid Id { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public int restarauntId { get; set; }

        public string Status { get; set; }
    }
}
