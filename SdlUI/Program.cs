namespace SdlUI
{
    using System;
    using SDL2;

    public class Program
    {
        [STAThread]
        private static void Main()
        {
            _ = SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_EVENTS);
            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

            var windowPointer = SDL.SDL_CreateWindow("Gusboy", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 160 * 3, 144 * 3, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            var rendererPointer = SDL.SDL_CreateRenderer(windowPointer, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            new Window(windowPointer, rendererPointer).Run();
        }
    }
}