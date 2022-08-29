namespace P4Webshop.Extensions.JWT
{
    /// <summary>
    /// Custom made AllowAnonymousAttribute that is overriding the [AllowAnonymous] on controllers
    /// its empty because for now it dont need to to anyting, it just need to exist.
    /// </summary>
    /// <param name="context"></param>
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
