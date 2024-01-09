using System.Security.Principal;

namespace Launcher
{
    public
        class CurrentProcess
    {
        public static bool IsElevated() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
