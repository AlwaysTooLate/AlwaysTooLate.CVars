// AlwaysTooLate.CVars (c) 2018-2019 Always Too Late.

using System;
using AlwaysTooLate.CVars;

/*
 * Config file assumes that all of its group classes are serializable and non-static.
 * Supports saving, look:
 * GameSettings.Current.Save(),
 * GameSettings.Current.Load(),
 * GameSettings.Current.ResetToDefault().
 */
[ConfigFile("Settings"), Serializable]
public class GameSettings : ConfigFile<GameSettings>
{
    [ConfigGroup("display"), Serializable]
    public class DisplayClass : ConfigGroup
    {
        // Note: Only 1-level deep ConfigGroups are currently supported.
        // So, you cannot make another one here.

        [ConfigVariable("width", "", 1920)]
        public int Width;

        [ConfigVariable("height", "", 1080)]
        public int Height;

        [ConfigVariable("mode", "The display mode.", 1)]
        public int Mode;

        [ConfigVariable("monitor", "", 0)]
        public int Monitor;

        [ConfigVariable("vsync", "", true)]
        public bool VSync;
    }

    [ConfigGroup("graphics"), Serializable]
    public class GraphicsClass : ConfigGroup
    {
        // ...
    }

    public DisplayClass Display = new DisplayClass();
    public GraphicsClass Graphics = new GraphicsClass();
}
