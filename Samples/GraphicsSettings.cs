// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using AlwaysTooLate.CVars;

[ConfigClass("graphics", true)]
public class GraphicsSettings
{
    [ConfigClass("display")]
    public class Window
    {
        [ConfigVariable("width", 1920)]
        public static int Width;

        [ConfigVariable("height", 1080)]
        public static int Height;

        [ConfigVariable("mode", 1)]
        public static int Mode;

        [ConfigVariable("screen", 0)]
        public static int Display;

        [ConfigVariable("vsync", true)]
        public static bool VSync;
    }

    [ConfigClass("graphics")]
    public class Graphics
    {
    }
}
