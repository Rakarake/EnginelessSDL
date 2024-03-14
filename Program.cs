using System;
using System.Collections.Generic;
using System.Linq;
using Engineless;
using Engineless.Utils;
using EnginelessSDL;

//void ShrinkEverything(Res<Time> t, Query<Transform2D> q) {
//    foreach (var h in q.hits) {
//        h.Value.scaleX -= 0.05 * t.hit.delta;
//        h.Value.scaleY -= 0.05 * t.hit.delta;
//    }
//}

void PlayerMovement(Res<Time> time, Res<Input> i, Query<(Transform2D, Player)> q) {
    var delta = time.hit.delta;
    foreach (var t in q.hits) {
        foreach (var k in i.hit.keysPressed) {
            if (k == Key.UP) {
                t.Value.Item1.y -= delta * 300;
            }
            if (k == Key.DOWN) {
                t.Value.Item1.y += delta * 300;
            }
            if (k == Key.LEFT) {
                t.Value.Item1.x -= delta * 300;
                t.Value.Item1.rotation -= delta;
            }
            if (k == Key.RIGHT) {
                t.Value.Item1.x += delta * 300;
                t.Value.Item1.rotation += delta;
            }
        }
    }
}

//void Shoot(Res<Intput> i, Query<(Transform2D, Player)> q) {
//    foreach (var h in q.hits) {
//        h.Value.
//    }
//}

Engine ecs = new();
//ecs.AddEntity(new List<Object>() {
//    new Transform2D() { x = 3, y = 3, scaleX = 1, scaleY = 1 },
//    new Square(400, 400),
//    new Sprite("splooty.png"),
//});
//
//ecs.AddEntity(new List<Object>() {
//    new Transform2D() { x = 8, y = 8, scaleX = 1, scaleY = 1 },
//    new Square(400, 400),
//    new Color() { r = 80, g = 200, b = 150, a = 255 },
//});

// Player
ecs.AddEntity(new List<Object>() {
    new Player(),
    new Transform2D() { x = 40, y = 40, scaleX = 0.1, scaleY = 0.1 },
    new Sprite("splooty.png"),
});
ecs.AddEntity(new List<Object>() {
    new Bullet(),
    new Square(10, 10),
    new Transform2D() { x = 20, y = 20 },
    new Color() { r = 20, g = 20, b = 20, a = 20 },
});

ecs.SetResource(new SDLState() { windowWidth = 1200, windowHeight = 800, });
ecs.AddSystem(Event.Startup, EnginelessSDL.EnginelessSDL.Initialize);
//ecs.AddSystem(Event.Update, MoveAllSprites);
//ecs.AddSystem(Event.Update, ShrinkEverything);
ecs.AddSystem(Event.Update, PlayerMovement);
ecs.Start();

class Player {}
class Bullet {}

