using Microsoft.AspNetCore.Authorization;

public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    public AuthorizePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}
