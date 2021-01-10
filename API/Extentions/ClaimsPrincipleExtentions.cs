using System.Security.Claims;

namespace API.Extentions
{
    public static class ClaimsPrincipleExtentions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            int returnid;
             int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out returnid);
             return returnid;
        }
    }
}