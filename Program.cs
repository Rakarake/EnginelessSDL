using System;
using System.Collections.Generic;
using System.Linq;
using Engineless;
using Engineless.Utils;
using EnginelessSDL;

void ShrinkEverything(Res<Time> t, Query<Transform2D> q) {
    foreach (var h in q.hits) {
        h.Value.scaleX -= 0.05 * t.hit.delta;
        h.Value.scaleY -= 0.05 * t.hit.delta;
    }
}

void PlayerMovement(Res<Time> time, Res<Input> i, Query<Transform2D> q) {
    var delta = time.hit.delta;
    foreach (var t in q.hits) {
        foreach (var k in i.hit.keysPressed) {
            if (k == Key.UP) {
                t.Value.y -= delta * 300;
            }
            if (k == Key.DOWN) {
                t.Value.y += delta * 300;
            }
            if (k == Key.LEFT) {
                t.Value.x -= delta * 300;
            }
            if (k == Key.RIGHT) {
                t.Value.x += delta * 300;
            }
        }
    }
}

Engine ecs = new();
ecs.AddEntity(new List<Object>() {
    new Transform2D() { x = 3, y = 3, scaleX = 1, scaleY = 1 },
    new Square(400, 400),
    new Sprite("splooty.png"),
});

ecs.AddEntity(new List<Object>() {
    new Transform2D() { x = 8, y = 8, scaleX = 1, scaleY = 1 },
    new Square(400, 400),
    new Color() { r = 80, g = 200, b = 150, a = 255 },
});

ecs.SetResource(new SDLState() { windowWidth = 1200, windowHeight = 800, });
ecs.AddSystem(Event.Startup, EnginelessSDL.EnginelessSDL.Initialize);
//ecs.AddSystem(Event.Update, MoveAllSprites);
//ecs.AddSystem(Event.Update, ShrinkEverything);
ecs.AddSystem(Event.Update, PlayerMovement);
ecs.Start();

