namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    public interface IRoleManagment
    {
        bool IsAdmin();

       bool IsInRole(string roleName);
    }
}