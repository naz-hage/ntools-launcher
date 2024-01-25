using System.Security.Principal;

namespace Ntools
{
    public static class CurrentProcess
    {
        public static bool IsElevated() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
