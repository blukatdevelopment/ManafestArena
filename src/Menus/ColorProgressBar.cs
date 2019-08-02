using Godot;
using System;

public class ColorProgressBar : Control
{
    public ColorRect underRect;
    public ColorRect progressRect;
    public AnimationPlayer animationPlayer;
    public Color color = new Color();
    public Label label = new Label();

    public int maxProgress = 100;
    public int progress = 0;

    public ColorProgressBar(Color color,int progress ,int maxProgress){
        this.color = color;
        SetProgress(progress,maxProgress);
    }

    public override void _Ready()
    {
        underRect = new ColorRect();
        progressRect = new ColorRect();
        underRect.Color = new Color(0.5f,0.5f,0.5f,0.5f);
        progressRect.Color = color;
        label = new Label();
        label.SetValign(Label.VAlign.Center);
        animationPlayer = new AnimationPlayer();
        AddChild(underRect);
        AddChild(progressRect);
        AddChild(label);
        AddChild(animationPlayer);
        InitAnimations();
        SetProgress(progress,maxProgress);
    }

    public void UpdateProgress(int progress, int maxProgress){//smoothen this maybe

        if(maxProgress>this.maxProgress){
            BlinkBright();
        
        }
        else if(maxProgress<this.maxProgress){
            BlinkDark();
        }
        else{
            if(progress>this.progress){
                BlinkBright();
            }
            else if(progress<this.progress){
                BlinkDark();
            }
            else{
                return;
            }
        }
        SetProgress(progress,maxProgress);
    }

    void SetProgress(int progress,int maxProgress){
        this.maxProgress = maxProgress;
        this.progress = progress;
        label.SetText(progress+"|"+maxProgress);
        Rect2 rect = GetRect();
        Menu.ScaleControl(progressRect,rect.Size.x*progress/maxProgress,rect.Size.y,0,0);
    }

    public void ScaleControls(){
        Rect2 rect = GetRect();
        Menu.ScaleControl(underRect,rect.Size.x,rect.Size.y,0,0);
        Menu.ScaleControl(progressRect,rect.Size.x*progress/maxProgress,rect.Size.y,0,0);
        Menu.ScaleControl(label,rect.Size.x,rect.Size.y,0,0);
        label.SetMargin(Margin.Left,5);
    }

    public void InitAnimations(){

        Animation blinkBright  = new Animation();
        int trackIndex = blinkBright.AddTrack(Animation.TrackType.Value);
        blinkBright.TrackSetPath(trackIndex,GetPathTo(progressRect)+":color");
        blinkBright.TrackInsertKey(trackIndex,0,color);
        blinkBright.TrackInsertKey(trackIndex,0.5f,color.Lightened(0.5f));
        blinkBright.TrackInsertKey(trackIndex,1,color);
        animationPlayer.AddAnimation("blink_bright",blinkBright);

        Animation blinkDark = new Animation();
        trackIndex = blinkDark.AddTrack(Animation.TrackType.Value);
        blinkDark.TrackSetPath(trackIndex,GetPathTo(progressRect)+":color");
        blinkDark.TrackInsertKey(trackIndex,0,color);
        blinkDark.TrackInsertKey(trackIndex,0.5f,color.Darkened(0.5f));
        blinkDark.TrackInsertKey(trackIndex,1,color);
        animationPlayer.AddAnimation("blink_dark",blinkDark);
    }

    public void BlinkDark(float customSpeed = 3){
        animationPlayer.Play("blink_dark",-1,customSpeed);
    }

    public void BlinkBright(float customSpeed = 3){
        animationPlayer.Play("blink_bright",-1,customSpeed);
    }

}
