// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using AlwaysTooLate.CVars;

/*
 * Config class assumes that all of its fields are static.
 * Doesn't support saving and ConfigGroups.
 */
[ConfigClass("cheats")]
public class CheatsClass
{
    [ConfigVariable("enabled", "Enables or disables cheats.", false, ConfigAccess.Server, ConfigFlags.Replicate)]
    public static bool Enabled;

    [ConfigVariable("fly", "", false)]
    public static bool Fly;
}