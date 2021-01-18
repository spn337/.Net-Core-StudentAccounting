using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StudentAccounting.Server.Helpers
{
    public class JwtBearerTokenSettings
    {
        public string SecretKey { get; set; }
        public int TokenLifetime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}
