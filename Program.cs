using System;
using System.Collections.Generic;
using Engineless;
using Engineless.Utils;
using EnginelessSDL;

void PlayerMovement(IECS ecs, Res<Time> time, Res<Input> i, Query<(Transform2D, Player)> q) {
    var delta = time.hit.delta;
    foreach (var t in q.hits) {
        foreach (var k in i.hit.keysPressed) {
            if (k == Key.UP) {
                //t.Value.Item1.y -= delta * 300;
            }
            if (k == Key.DOWN) {
                //t.Value.Item1.y += delta * 300;
            }
            if (k == Key.LEFT) {
                //t.Value.Item1.x -= delta * 300;
                t.Value.Item1.rotation -= delta * 4;
            }
            if (k == Key.RIGHT) {
                //t.Value.Item1.x += delta * 300;
                t.Value.Item1.rotation += delta * 4;
            }
        }

        // Move the player
        t.Value.Item1.x += System.Math.Sin(t.Value.Item1.rotation) * delta * 300;
        t.Value.Item1.y += System.Math.Cos(t.Value.Item1.rotation) * delta * 300;

        ecs.AddEntity(new List<Object> {
            new Bullet(),
            new Square(10, 10),
            new Color() { r = 20, g = 20, b = 20, a = 20 },
            new Transform2D() { x = t.Value.Item1.x, y = t.Value.Item1.y, rotation = t.Value.Item1.rotation },
        });
    }
}

void MoveBullet(Res<Time> time, Query<(Transform2D, Bullet)> q) {
    foreach (var t in q.hits) {
        t.Value.Item1.x += System.Math.Sin(t.Value.Item1.rotation) * time.hit.delta * 300;
        t.Value.Item1.y += System.Math.Cos(t.Value.Item1.rotation) * time.hit.delta * 300;
    }
}


Engine ecs = new();
// Player
ecs.AddEntity(new List<Object>() {
    new Player(),
    new Transform2D() { x = 40, y = 40 },
    new Sprite("splooty.png"),
});

ecs.SetResource(new SDLState() { windowWidth = 1200, windowHeight = 800, });
ecs.AddSystem(Event.Startup, EnginelessSDL.EnginelessSDL.Initialize);
ecs.AddSystem(Event.Update, PlayerMovement);
ecs.AddSystem(Event.Update, MoveBullet);
ecs.Start();


class Player {
    double momentum = 0;
}

class Bullet {}

