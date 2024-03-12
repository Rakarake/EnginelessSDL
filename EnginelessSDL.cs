using System;
using Engineless;
using Engineless.Utils;
using SDL2;

namespace EnginelessSDL {
    public class Sprite {
        public Sprite(String path) {
            this.path = path;
        }
        public String path;
        public nint texture = -1;
    }

    public static class EnginelessSDL {

        private class RenderState {
            public nint renderer = 0;
            public nint window = 0;
        }

        // TODO: free all textures
        static void Exit(IECS ecs, Res<RenderState> r) {
            var renderer = r.hit.renderer;
            var window = r.hit.window;

            // Clean up the resources that were created.
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
            ecs.UnsetResource(typeof(RenderState));
        }

        // Initialize sprites before rendering if they are not already
        static void InitializeSprites(Query<Sprite> q, Res<RenderState> r) {
            foreach (var s in q.hits) {
                if (s.Value.texture == -1) {
                    s.Value.texture = SDL2.SDL_image.IMG_LoadTexture(r.hit.renderer, s.Value.path);
                }
            }
        }

        static void Render(IECS ecs, Res<RenderState> r, Query<(Transform2D, Sprite)> q) {
            var renderer = r.hit.renderer;

            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        Exit(ecs, r);
                        break;
                }
            }
            
            // Sets the color that the screen will be cleared with.
            SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255);
            
            // Clears the current render surface.
            SDL.SDL_RenderClear(renderer);
            
            //// Set the color to red before drawing our shape
            //SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            //
            //// Draw a line from top left to bottom right
            //SDL.SDL_RenderDrawLine(renderer, 0, 0, 640, 480);

            // Draw all sprites
            foreach (var t in q.hits) {
                var (transform, sprite) = t.Value;
                int textureWidth;
                int textureHeight;
                uint format;
                int access;
                SDL.SDL_QueryTexture(t.Value.Item2.texture, out format, out access, out textureWidth, out textureHeight);
                SDL.SDL_Rect rect = new SDL.SDL_Rect()
                    {x = transform.position.Item1, y = transform.position.Item2,
                    w = (int) (textureWidth * transform.scale.Item1), h = (int) (textureHeight * transform.scale.Item2)};
                SDL.SDL_RenderCopy(renderer, sprite.texture, IntPtr.Zero, ref rect);
            }
            
            // Switches out the currently presented render surface with the one we just did work on.
            SDL.SDL_RenderPresent(renderer);
        }

        public static void Initialize(IECS ecs) {
            // Initilizes SDL.
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
            }
            
            // Create a new window given a title, size, and passes it a flag indicating it should be shown.
            var window = SDL.SDL_CreateWindow("Engineless",
                    SDL.SDL_WINDOWPOS_UNDEFINED,
                    SDL.SDL_WINDOWPOS_UNDEFINED,
                    640,
                    480,
                    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
            );
            
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
            }
            
            // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
            var renderer = SDL.SDL_CreateRenderer(window, 
                                                    -1, 
                                                    SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | 
                                                    SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
            }
            
            // Initilizes SDL_image for use with png files.
            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
            }

            ecs.SetResource(new RenderState() { renderer = renderer, window = window, });
            ecs.AddSystem(Event.Update, InitializeSprites);
            ecs.AddSystem(Event.Update, Render);
        }
    }
}
