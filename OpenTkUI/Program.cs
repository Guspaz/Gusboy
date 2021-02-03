namespace OpenTkUI
{
    using System;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.Desktop;

    public class Program
    {
        [STAThread]
        private static void Main()
        {
            var settings = new GameWindowSettings()
            {
                RenderFrequency = 59.7275,
                UpdateFrequency = 59.7275 * 2,
            };

            // Pretty much all the OpenTK GL code is borrowed from https://github.com/BluestormDNA/ProjectPSX and modified for my needs.
            var nativeWindow = new NativeWindowSettings()
            {
                API = ContextAPI.OpenGL,
                Size = new Vector2i(160 * 3, 144 * 3),
                Title = "Gusboy",
                Profile = ContextProfile.Compatability,
                WindowBorder = WindowBorder.Fixed,
            };

            var window = new Window(settings, nativeWindow)
            {
                // V-sync is super broken in windowed mode?
                VSync = VSyncMode.Off,
            };

            window.Run();
        }
    }
}