using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileSearch.Server.Models
{
    public class Search
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SearchTerm { get; set; } = null!;
        public DateTime SearchDate { get; set; }
        public User User { get; set; } = null!;
    }
}
