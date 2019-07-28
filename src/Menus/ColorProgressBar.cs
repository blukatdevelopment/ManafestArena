using Godot;
using System;

public class ColorProgressBar : Control
{
    public ColorRect underRect;
    public ColorRect progressRect;
    public Color color = new Color();

    public int maxProgress = 100;
    public int progress = 0;

    public ColorProgressBar(Color color,int progress =100,int maxProgress = 100){
        this.color = color;
        updateProgress(progress,maxProgress);
    }

    public override void _Ready()
    {
        underRect = new ColorRect();
        progressRect = new ColorRect();
        underRect.Color = new Color(0.5f,0.5f,0.5f,0.5f);
        progressRect.Color = color;
        AddChild(underRect);
        AddChild(progressRect);
    }

    public void updateProgress(int progress, int maxProgress){//smoothen this maybe
        this.maxProgress = maxProgress;
        this.progress = progress;
        ScaleControls();
    }

    public void ScaleControls(){
        Rect2 rect = GetRect();
        Menu.ScaleControl(underRect,rect.Size.x,rect.Size.y,0,0);
        Menu.ScaleControl(progressRect,rect.Size.x*progress/maxProgress,rect.Size.y,0,0);
    }

}
