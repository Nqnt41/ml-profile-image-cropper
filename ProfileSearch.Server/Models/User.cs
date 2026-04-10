using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileSearch.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public ICollection<Image> Images { get; set; } = null!;
        public ICollection<Search> Searches { get; set; } = null!;
    }
}
