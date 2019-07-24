/*
  Records raw device state. 
*/
using Godot;
using System;
using System.Collections.Generic;

public class DeviceState : Node {

  public int joypad; // Keyboard/Mouse input is the same for every DeviceState
  private System.Collections.Generic.Dictionary<int, float> joyButtons;
  private System.Collections.Generic.Dictionary<int, float> joyAxes;


  private System.Collections.Generic.Dictionary<int, float> mouseButtons;
  private System.Collections.Generic.Dictionary<int, float> keys;
  
  private Vector2 mousePosition, mouseMovement;

  public DeviceState(int joypad){
    this.joypad = joypad;
    joyButtons = new System.Collections.Generic.Dictionary<int, float>();
    joyAxes = new System.Collections.Generic.Dictionary<int, float>();
    mouseButtons = new System.Collections.Generic.Dictionary<int, float>();
    keys = new System.Collections.Generic.Dictionary<int, float>();
    mousePosition = new Vector2();
    mouseMovement = new Vector2();
  }

  public void AddJoyButton(int button){
    if(joyButtons.ContainsKey(button)){
      return;
    }
    joyButtons.Add(button, 0f);
  }

  public void AddJoyAxis(int axis){
    if(joyAxes.ContainsKey(axis)){
      return;
    }
    joyAxes.Add(axis, 0f);
  }

  public void AddMouseButton(int button){
    if(mouseButtons.ContainsKey(button)){
      return;
    }
    mouseButtons.Add(button, 0f);
  }

  public void AddKey(int key){
    if(keys.ContainsKey(key)){
      return;
    }
    keys.Add(key, 0f);
  }

  public void ClearInputs(){
    joyButtons = new System.Collections.Generic.Dictionary<int, float>();
    joyAxes = new System.Collections.Generic.Dictionary<int, float>();
    mouseButtons = new System.Collections.Generic.Dictionary<int, float>();
    keys = new System.Collections.Generic.Dictionary<int, float>();
  }

  // Will fail loudly if you listen for an invalid button/axis/key
  public float GetJoyButton(int button){
    return joyButtons[button];
  }

  public float GetJoyAxis(int axis){
    return joyAxes[axis];
  }

  public float GetMouseButton(int button){
    return mouseButtons[button];
  }

  public float GetKey(int key){
    return keys[key];
  }

  // You don't need to add  mouseAxes. They are listened to automagically.
  public float GetMouseAxis(int axis){
    if(axis == (int)FPSInputHandler.MouseAxis.X){
      return mouseMovement.x;
    }
    return mouseMovement.y;
  }

  public Vector2 GetMousePosition(){
    return mousePosition;
  }

  public Vector2 GetMouseMovement(){
    return mouseMovement;
  }

  public override void _Process(float delta){
    List<int> joyButtonKeys = new List<int>(joyButtons.Keys);
    foreach(int button in joyButtonKeys){
      joyButtons[button] = Input.IsJoyButtonPressed(joypad, button) ? 1f : 0f;
    }

    List<int> joyAxisKeys = new List<int>(joyAxes.Keys);
    foreach(int axis in joyAxisKeys){
      joyAxes[axis] = Input.GetJoyAxis(joypad, axis);
    }

    List<int> mouseButtonKeys = new List<int>(mouseButtons.Keys);
    foreach(int button in mouseButtonKeys){
      mouseButtons[button] = Input.IsMouseButtonPressed(button) ? 1f : 0f;
    }

    List<int> keysKeys = new List<int>(keys.Keys);
    foreach(int key in keysKeys){
      keys[key] = Input.IsKeyPressed(key) ? 1f : 0f;
    }
    mouseMovement = new Vector2();
  }

  public override void _Input(Godot.InputEvent evt){
    InputEventMouseMotion mot = evt as InputEventMouseMotion;
    if(mot != null){
      mousePosition = mot.GlobalPosition;
      mouseMovement = mot.Relative;
    }
  }

}