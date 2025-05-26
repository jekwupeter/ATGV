using System.ComponentModel.DataAnnotations;

namespace atgv.Core.Models.Common
{
    public class UserModel
    {
        private string _email = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email
        {
            get => _email;
            set
            {
                _email = value.ToLowerInvariant();
            }
        }
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
