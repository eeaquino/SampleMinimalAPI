using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SampleMinimalAPI.Helpers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
       
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
										ILoggerFactory logger,
										UrlEncoder encoder,
										ISystemClock clock) : base(options, logger, encoder, clock)
        {
            
        }
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "me", ClaimValueTypes.String, "SampleAPI")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
			return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
		}
		
		


	}
}
