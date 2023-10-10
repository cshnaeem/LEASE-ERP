using Microsoft.Owin.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Text;

[assembly: OwinStartupAttribute(typeof(AGEERP.Startup))]
namespace AGEERP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "*******",   
                        ValidAudience = "*******",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("W03hEb0Hk4Vk1jfU3pBuIcg"))
                    }
                });
        }
    }
}
