using Godot;
using System;

public class Camera : Camera2D
{
    public float CameraSpeed = 800f;
    public float CameraZoomSpeed = .7f;
    public float MinZoom = 0.5f;
    public float MaxZoom = 2.04f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta) {
        if(Input.IsKeyPressed((int)KeyList.Escape)) {
            GetTree().Quit();
        }
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

        float zoomFactor = 0f;
        if(Input.IsKeyPressed((int)KeyList.Minus)) {
            zoomFactor = 1;
        } else if(Input.IsKeyPressed((int)KeyList.Equal)) {
            zoomFactor = -1;
        }
        Zoom = Zoom * (1 + zoomFactor * CameraZoomSpeed * delta);
        if(Zoom.x < MinZoom) {
            Zoom = new Vector2(MinZoom, MinZoom);
        }
         if(Zoom.x > MaxZoom) {
            Zoom = new Vector2(MaxZoom, MaxZoom);
        }
    }
}
