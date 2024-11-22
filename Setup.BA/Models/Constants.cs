using JetBrains.Annotations;

namespace Setup.BA.Models
{
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal static class Constants
  {
    public const string PerUserLocation = "[LocalAppDataFolder]Programs\\FractalFlair";
    public const string PerMachineLocation = "[ProgramFiles6432Folder]FractalFlair";
    public const string BundleLogName = "WixBundleLog";
    public const string VersionVariable = "WixBundleVersion";
    public const string BundleNameVariable = "WixBundleName";
    public const string RebootMessage = "This machine has other pending updates that require a reboot before the app can be installed. Please restart, then try installing again.";
    public const string Line = "========================================================================================";
  }
}