/*
  Records raw device state. 
*/
using Godot;
using System;
using System.Collections.Generic;

public class DeviceState : Node {

  public int joypad; // Keyboard/Mouse input is the same for every DeviceState
  private Dictionary<int, float> joyButtons;
  private Dictionary<int, float> joyAxes;


  private Dictionary<int, float> mouseButtons;
  private Dictionary<int, float> keys;
  
  private Vector2 mousePosition, mouseMovement;

  public DeviceState(int joypad){
    this.joypad = joypad;
    joyButtons = new Dictionary<int, float>();
    joyAxes = new Dictionary<int, float>();
    mouseButtons = new Dictionary<int, float>();
    keys = new Dictionary<int, float>();
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
    joyButtons = new Dictionary<int, float>();
    joyAxes = new Dictionary<int, float>();
    mouseButtons = new Dictionary<int, float>();
    keys = new Dictionary<int, float>();
  }

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
    InputEventMouseButton btn = evt as InputEventMouseButton;
    if(btn!=null){
      int btnIndex = btn.ButtonIndex;
      
      if(mouseButtons.ContainsKey(btnIndex)){
        mouseButtons[btnIndex] = 1;
      }
      else{
        mouseButtons[btnIndex] = 0;
      }
    }
  }

}