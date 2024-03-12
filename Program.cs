using System;
using System.Collections.Generic;
using Engineless;
using Engineless.Utils;
using EnginelessSDL;
using SDL2;

// Load a texture

void MoveAllSprites(Query<(Transform2D, Sprite)> q) {
    foreach (var h in q.hits) {
        h.Value.Item1.position.Item1 += 1;
        h.Value.Item1.position.Item2 += 1;
    }
}

Engine ecs = new();
ecs.AddEntity(new List<Object>() {
    new Transform2D() { position = (4, 4), scale = (1, 1) },
    new Sprite("splooty.png"),
});
ecs.AddSystem(Event.Startup, EnginelessSDL.EnginelessSDL.Initialize);
ecs.AddSystem(Event.Update, MoveAllSprites);
ecs.Start();

//// Initilizes SDL.
//if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
//{
//    Console.WriteLine($"There was an issue initilizing SDL. {SDL.SDL_GetError()}");
//}
//
//// Create a new window given a title, size, and passes it a flag indicating it should be shown.
//var window = SDL.SDL_CreateWindow("SDL .NET 6 Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, 640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
//
//if (window == IntPtr.Zero)
//{
//    Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
//}
//
//// Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
//var renderer = SDL.SDL_CreateRenderer(window, 
//                                        -1, 
//                                        SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | 
//                                        SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
//
//if (renderer == IntPtr.Zero)
//{
//    Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
//}
//
//// Initilizes SDL_image for use with png files.
//if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
//{
//    Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
//}
//
//var running = true;
//
//// Main loop for the program
//while (running)
//{
//    // Check to see if there are any events and continue to do so until the queue is empty.
//    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
//    {
//        switch (e.type)
//        {
//            case SDL.SDL_EventType.SDL_QUIT:
//                running = false;
//                break;
//        }
//    }
//
//    // Sets the color that the screen will be cleared with.
//    SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255);
//
//    // Clears the current render surface.
//    SDL.SDL_RenderClear(renderer);
//
//    // Set the color to red before drawing our shape
//    SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
//
//    // Draw a line from top left to bottom right
//    SDL.SDL_RenderDrawLine(renderer, 0, 0, 640, 480);
//
//    // Switches out the currently presented render surface with the one we just did work on.
//    SDL.SDL_RenderPresent(renderer);
//}
//
//// Clean up the resources that were created.
//SDL.SDL_DestroyRenderer(renderer);
//SDL.SDL_DestroyWindow(window);
//SDL.SDL_Quit();
