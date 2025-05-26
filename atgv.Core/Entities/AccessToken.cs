using System.ComponentModel.DataAnnotations;

namespace atgv.Core.Entities
{
    public class AccessToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}
