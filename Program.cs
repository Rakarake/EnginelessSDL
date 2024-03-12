using System;
using System.Collections.Generic;
using Engineless;
using Engineless.Utils;
using EnginelessSDL;

void MoveAllSprites(Query<(Transform2D, Sprite)> q) {
    foreach (var h in q.hits) {
        h.Value.Item1.position.Item1 += 1;
        h.Value.Item1.position.Item2 += 1;
    }
}

void ShrinkEverything(Query<Transform2D> q) {
    foreach (var h in q.hits) {
        h.Value.scale.Item1 -= 0.005;
        h.Value.scale.Item2 -= 0.005;
    }
}

Engine ecs = new();
ecs.AddEntity(new List<Object>() {
    new Transform2D() { position = (4, 4), scale = (1, 1) },
    new Square(400, 400),
    new Sprite("splooty.png"),
});

ecs.AddEntity(new List<Object>() {
    new Transform2D() { position = (4, 4), scale = (1, 1) },
    new Square(400, 400),
    new Color() { r = 80, g = 200, b = 150, a = 255 },
});

ecs.AddSystem(Event.Startup, EnginelessSDL.EnginelessSDL.Initialize);
ecs.AddSystem(Event.Update, MoveAllSprites);
ecs.AddSystem(Event.Update, ShrinkEverything);
ecs.Start();

