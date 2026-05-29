namespace ApiMiniPrj.Api.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override  Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            bool hasPermission = context.User.Claims.Any(c => c.Type == "Permission" && c.Value == requirement.Permission);
            if (hasPermission) context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
