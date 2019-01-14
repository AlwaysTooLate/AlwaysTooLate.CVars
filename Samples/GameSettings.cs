// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using AlwaysTooLate.CVars;

[ConfigFile("Settings"), ConfigGroup("settings")]
public class GameSettings : ConfigFile<GameSettings>
{
    [ConfigGroup("display")]
    public class DisplayClass
    {
        [ConfigVariable("width", 1920)]
        public int Width;

        [ConfigVariable("height", 1080)]
        public int Height;

        [ConfigVariable("mode", 1)]
        public int Mode;

        [ConfigVariable("monitor", 0)]
        public int Monitor;

        [ConfigVariable("vsync", true)]
        public bool VSync;
    }

    [ConfigGroup("graphics")]
    public class GraphicsClass
    {
    }

    [ConfigGroup("cheats")]
    public class CheatsClass
    {
        [ConfigVariable("enabled", false, ConfigAccess.Server)]
        public bool Enabled;

        [ConfigVariable("fly", false)]
        public bool Fly;
    }

    public DisplayClass Display { get; } = new DisplayClass();
    public GraphicsClass Graphics { get;} = new GraphicsClass();
    public CheatsClass Cheats { get; } = new CheatsClass();
}
