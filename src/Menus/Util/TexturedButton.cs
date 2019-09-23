using Godot;
using System;

public class TexturedButton : Godot.TextureButton
{
    Action onClick;
    
    public override void _Ready() {}
    
    public void SetOnClick(Action onClick){
      this.onClick = onClick;
    }
    
    public override void _Pressed(){
      if(onClick != null){ onClick(); }
    }
}
