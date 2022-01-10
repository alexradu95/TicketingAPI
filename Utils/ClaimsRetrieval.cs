using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WatersTicketingAPI.Utils;

public static class ClaimsRetrieval
{
    public static string ExtractUsernameFromClaim(this ControllerBase controller)
    {
        //First get user claims    
        var claims = controller.User.Claims.ToList();
        //Filter specific claim    
        return claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))?.Value;
    }
}