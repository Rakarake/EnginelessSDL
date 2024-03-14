using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engineless;
using Engineless.Utils;
using SDL2;

namespace EnginelessSDL {
    public class SDLState {
        public int windowWidth = 300;
        public int windowHeight = 300;
    }
    public class Sprite {
        public Sprite(String path) {
            this.path = path;
        }
        public String path;
        public nint texture = -1;
    }

    public class Square {
        public Square(int width, int height) {
            this.width = width;
            this.height = height;
        }
        public double width;
        public double height;
    }

    // The delta time between frames
    public class Time {
        public double delta = 1/60;
        // Used to get the delta
        public Stopwatch stopwatch = new();
    }

    // Uses string representations
    public class Input {
        public HashSet<Key> keysPressed = new();
    }

    // Only these keys atm
    public enum Key {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        X,
        NONE,
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
            System.Environment.Exit(0);
        }

        // Initialize sprites before rendering if they are not already
        static void InitializeSprites(Query<Sprite> q, Res<RenderState> r) {
            foreach (var s in q.hits) {
                if (s.Value.texture == -1) {
                    s.Value.texture = SDL2.SDL_image.IMG_LoadTexture(r.hit.renderer, s.Value.path);
                }
            }
        }

        static void RenderSprites(Res<RenderState> s, Query<(Transform2D, Sprite)> q) {
            // Draw all sprites
            foreach (var t in q.hits) {
                var (transform, sprite) = t.Value;
                int textureWidth;
                int textureHeight;
                uint format;
                int access;
                SDL.SDL_QueryTexture(sprite.texture, out format, out access, out textureWidth, out textureHeight);
                SDL.SDL_Rect rect = new SDL.SDL_Rect()
                    {x = (int) (transform.x + (textureWidth/2) - (transform.scaleX * textureWidth/2)),
                        y = (int) (transform.y + (textureHeight/2) - (transform.scaleY * textureHeight/2)),
                    w = (int) (textureWidth * transform.scaleX), h = (int) (textureHeight * transform.scaleY)};
                SDL.SDL_RenderCopy(s.hit.renderer, sprite.texture, IntPtr.Zero, ref rect);
            }
        }

        static void RenderSquares(Res<RenderState> s, Query<(Transform2D, Square, Color)> q) {
            foreach (var t in q.hits) {
                var (transform, square, color) = t.Value;
                var rect = new SDL.SDL_Rect {
                    x = (int) (transform.x + (square.width/2) - (transform.scaleX * square.width / 2)),
                    y = (int) (transform.y + (square.width/2) - (transform.scaleY * square.height / 2)),
                    w = (int) (transform.scaleX * square.width),
                    h = (int) (transform.scaleY * square.height),
                };

                SDL.SDL_SetRenderDrawColor
                    (s.hit.renderer, color.r, color.g, color.b, color.a);
                
                // Draw a filled in rectangle.
                SDL.SDL_RenderFillRect(s.hit.renderer, ref rect);
            }
        }

        static private Key ConvertKeyboardInput(SDL.SDL_Keycode input) {
            Key k;
            if (input == SDL.SDL_Keycode.SDLK_LEFT) {
                k = Key.LEFT;
            }
            else if (input == SDL.SDL_Keycode.SDLK_RIGHT) {
                k = Key.RIGHT;
            }
            else if (input == SDL.SDL_Keycode.SDLK_UP) {
                k = Key.UP;
            }
            else if (input == SDL.SDL_Keycode.SDLK_DOWN) {
                k = Key.DOWN;
            }
            else if (input == SDL.SDL_Keycode.SDLK_x) {
                k = Key.X;
            }
            else
                k = Key.NONE;
            return k;
        }

        static void PreRender(IECS ecs, Res<RenderState> r, Res<Input> i, Query<(Transform2D, Sprite)> q) {
            var renderer = r.hit.renderer;

            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        Exit(ecs, r);
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        var k = ConvertKeyboardInput(e.key.keysym.sym);
                        if (k != Key.NONE) {
                            i.hit.keysPressed.Add(k);
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        var k2 = ConvertKeyboardInput(e.key.keysym.sym);
                        if (k2 != Key.NONE) {
                            i.hit.keysPressed.Remove(k2);
                        }
                        break;
                }
            }
            
            // Sets the color that the screen will be cleared with.
            SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255);
            
            // Clears the current render surface.
            SDL.SDL_RenderClear(renderer);
        }

        static void Render(IECS ecs, Res<RenderState> r, Res<Time> time, Query<(Transform2D, Sprite)> q) {
            //// Set the color to red before drawing our shape
            //SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            //
            //// Draw a line from top left to bottom right
            //SDL.SDL_RenderDrawLine(renderer, 0, 0, 640, 480);

            // Switches out the currently presented render surface with the one we just did work on.
            SDL.SDL_RenderPresent(r.hit.renderer);
            time.hit.stopwatch.Stop();
            time.hit.delta = (double) time.hit.stopwatch.Elapsed.Milliseconds / 1000;
            time.hit.stopwatch.Restart();
        }

        public static void Initialize(Res<SDLState> sdlState, IECS ecs) {
            // Initilizes SDL.
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
            }
            
            // Create a new window given a title, size, and passes it a flag indicating it should be shown.
            var window = SDL.SDL_CreateWindow("Engineless",
                    SDL.SDL_WINDOWPOS_UNDEFINED,
                    SDL.SDL_WINDOWPOS_UNDEFINED,
                    sdlState.hit.windowWidth,
                    sdlState.hit.windowHeight,
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

            Stopwatch stopwatch = new();
            stopwatch.Start();

            ecs.SetResource(new RenderState() { renderer = renderer, window = window, });
            ecs.SetResource(new Time());
            ecs.SetResource(new Input());
            ecs.AddSystem(Event.Update, InitializeSprites);
            ecs.AddSystem(Event.Update, PreRender);
            ecs.AddSystem(Event.Update, RenderSprites);
            ecs.AddSystem(Event.Update, RenderSquares);
            ecs.AddSystem(Event.Update, Render);
        }
    }
}
