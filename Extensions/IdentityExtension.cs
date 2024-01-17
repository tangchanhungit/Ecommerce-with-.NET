using System.Security.Claims;

namespace Ecommerce_web_app.Extentions
{
	public static class IdentityExtension
	{
		public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimType) 
		{
			var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
			return (claim != null) ? claim.Value : string.Empty;	
		}
	}
}
