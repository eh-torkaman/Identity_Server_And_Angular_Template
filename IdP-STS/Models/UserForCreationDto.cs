using IdP;

namespace STS.Models
{
    public class UserForCreationDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ClaimDto
    {
        private string type;
        private string value;

        public string ClaimType { get => type; set => type = value.TrimEvelNull(); }
        public string ClaimValue { get => value; set => this.value = value.TrimEvelNull(); }
    }

    public class ChangeUserPassDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    
}
