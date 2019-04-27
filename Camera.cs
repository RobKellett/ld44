using Godot;
using System;

public class Camera : Camera2D
{
    public float CameraSpeed = 400f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta) {
        Vector2 scrollDir = new Vector2();
        if(Input.IsKeyPressed((int)KeyList.Left)) {
            scrollDir.x -= 1;
        }
        if(Input.IsKeyPressed((int)KeyList.Right)) {
            scrollDir.x += 1;  
        }
        if(Input.IsKeyPressed((int)KeyList.Up)) {
            scrollDir.y -= 1;
        }
        if(Input.IsKeyPressed((int)KeyList.Down)) {
            scrollDir.y += 1;  
        }
        scrollDir = scrollDir.Normalized() * CameraSpeed * delta;
        Translate(scrollDir);
    }
}
