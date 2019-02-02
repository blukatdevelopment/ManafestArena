/*
  ActorInputHandler collects input from a device manager and causes its actor to behave accordingly. 
*/

using Godot;
using System;
using System.Collections.Generic;

public class ActorInputHandler : Brain {
  private DeviceManager device;
  private System.Collections.Generic.Dictionary<InputEvent.Buttons, bool> held; 
  private float delta = 1f;
  private float syncTimer = 0f;
  public const float syncRate = 0.05f;


  public ActorInputHandler(Actor actor) : base (actor){
    InitHeld();
    device = new DeviceManager(Session.session.player1Device, 0);
    GD.Print("Initialized ActorInputHandler with " + device.device);
  }

  
  private void InitHeld(){
    held = new System.Collections.Generic.Dictionary<InputEvent.Buttons, bool>();
    foreach(InputEvent.Buttons button in Enum.GetValues(typeof(InputEvent.Buttons))){
      held.Add(button, false);
    }
  }
  
  public override void Update(float delta){
    this.delta = delta;
    
    List<InputEvent> events = device.GetInputEvents();
    
    if(actor.menuActive == false){
      for(int i = 0; i < events.Count; i++){
          if(events[i].IsButton()){ HandleButton(events[i]); }
          else{ HandleAxis(events[i]); }
      }
      
      HandleMovement();
    }
    else{
      HandleMenuInput(events);
    }
    
    if(Session.NetActive()){
      NetUpdate();
    }
  }

  public void NetUpdate(){
    syncTimer += delta;
    
    if(syncTimer >= syncRate){
      syncTimer = 0f;
      actor.SyncPosition();
      actor.SyncAim();
    }
  }
  
  private void HandleMenuInput(List<InputEvent> events){
    for(int i = 0; i < events.Count; i++){
      if(events[i].button == InputEvent.Buttons.Esc){ HandleButton(events[i]); }
      if(events[i].button == InputEvent.Buttons.Tab){ HandleButton(events[i]); }
    }
  }
  
  bool IsN64(){
    return device.device == DeviceManager.Devices.N64;
  }

  bool IsKbm(){
    return device.device == DeviceManager.Devices.MouseAndKeyboard;
  }

  bool HeldRight(){
    if(IsKbm() && held.ContainsKey(InputEvent.Buttons.D) && held[InputEvent.Buttons.D]){
      return true;
    }

    if(IsN64() && held.ContainsKey(InputEvent.Buttons.CRight) && held[InputEvent.Buttons.CRight]){
      return true;
    }

    return false;
  }

  bool HeldLeft(){
    if(IsKbm() && held.ContainsKey(InputEvent.Buttons.A) && held[InputEvent.Buttons.A]){
      return true;
    }

    if(IsN64() && held.ContainsKey(InputEvent.Buttons.CLeft) && held[InputEvent.Buttons.CLeft]){
      return true;
    }

    return false;
  }

  bool HeldUp(){
    if(IsKbm() && held.ContainsKey(InputEvent.Buttons.W) && held[InputEvent.Buttons.W]){
      return true;
    }

    return false;
  }

  bool HeldDown(){
    if(IsKbm() && held.ContainsKey(InputEvent.Buttons.S) && held[InputEvent.Buttons.S]){
      return true;
    }

    return false;
  }

  private void HandleMovement(){
    int dx = 0;
    int dz = 0;
    
    if(HeldUp()){ dz--; }
    if(HeldLeft()){ dx--; }
    if(HeldDown()){ dz++; }
    if(HeldRight()){ dx++; }

    if(dx != 0 || dz != 0){
      Vector3 movement = new Vector3(dx, 0, dz);
      movement *= actor.GetMovementSpeed(); 
      actor.Move(movement, this.delta); 
    }
  }
  
  
  private void HandleButton(InputEvent evt){
    GD.Print("Handling button " + evt.ToString());
    if(evt.action == InputEvent.Actions.Down){
      held[evt.button] = true;
      Press(evt);
    }
    else if(evt.action == InputEvent.Actions.Up){
      held[evt.button] = false;
      
      if(evt.button == InputEvent.Buttons.Shift){
        actor.SetSprint(false);
      }
    }
  }

  private void Press(InputEvent evt){    
    switch(device.device){
      case DeviceManager.Devices.MouseAndKeyboard:
        PressKeyboard(evt);
        break;
      case DeviceManager.Devices.N64:
        PressN64(evt);
        break;
      case DeviceManager.Devices.Nes:
        PressNes(evt);
        break;
    }
  }

  private void PressKeyboard(InputEvent evt){
    switch(evt.button){
      case InputEvent.Buttons.Esc: 
        Session.Event(SessionEvent.PauseEvent());
        break;
      case InputEvent.Buttons.Tab: actor.ToggleInventory(); break;
      case InputEvent.Buttons.Space: actor.Jump(); break;
      case InputEvent.Buttons.Shift: actor.SetSprint(true); break;
      case InputEvent.Buttons.M1: actor.Use(Item.Uses.A); break;
      case InputEvent.Buttons.M2: actor.Use(Item.Uses.B); break;
      case InputEvent.Buttons.M3: actor.Use(Item.Uses.C); break;
      case InputEvent.Buttons.R: actor.Use(Item.Uses.D); break;
      case InputEvent.Buttons.E: actor.InitiateInteraction(); break;
    }
  }

  private void PressNes(InputEvent evt){
    switch(evt.button){
      case InputEvent.Buttons.Select: 
        Session.Event(SessionEvent.PauseEvent());
        break;
      case InputEvent.Buttons.B: actor.Jump(); break;
      case InputEvent.Buttons.A: actor.Use(Item.Uses.A); break;
    }
  }

  private void PressN64(InputEvent evt){
    switch(evt.button){
      case InputEvent.Buttons.Start: 
        Session.Event(SessionEvent.PauseEvent());
        break;
      case InputEvent.Buttons.L: actor.SetSprint(true); break;
      case InputEvent.Buttons.Z: actor.Use(Item.Uses.A); break;
      case InputEvent.Buttons.R: actor.Use(Item.Uses.B); break;
      case InputEvent.Buttons.CUp: actor.Use(Item.Uses.C); break;
      case InputEvent.Buttons.CDown: actor.Jump(); break;
      case InputEvent.Buttons.B: actor.Use(Item.Uses.D); break;
      case InputEvent.Buttons.A: actor.InitiateInteraction(); break;
    }
  }

  private void HandleAxis(InputEvent evt){
    switch(device.device){
      case DeviceManager.Devices.MouseAndKeyboard:
        HandleAxisKeyboard(evt);
        break;
      case DeviceManager.Devices.N64:
        HandleAxisN64(evt);
        break;
      case DeviceManager.Devices.Nes:
        HandleAxisNes(evt);
        break;
    }
  
  }

  private void HandleAxisKeyboard(InputEvent evt){
    float wx = Session.session.mouseSensitivityX;
    float wy = Session.session.mouseSensitivityY;

    if(evt.axis == InputEvent.Axes.Mouse){
      actor.Turn(evt.x * wx, evt.y * wy);
    }

  }

  private void HandleAxisN64(InputEvent evt){
    float wx = Session.session.mouseSensitivityX;
    float wy = Session.session.mouseSensitivityY;

    Vector3 movement = new Vector3(0, 0, -evt.y);
    movement *= actor.GetMovementSpeed(); 
    actor.Move(movement, this.delta);

    if(evt.x != 0f){
      actor.Turn(evt.x * -wx, 0f);
    }
  }

  private void HandleAxisNes(InputEvent evt){
    float wx = Session.session.mouseSensitivityX;
    float wy = Session.session.mouseSensitivityY;

    Vector3 movement = new Vector3(0, 0, -evt.y);
    movement *= actor.GetMovementSpeed(); 
    actor.Move(movement, this.delta);

    if(evt.x != 0f){
      actor.Turn(evt.x * -wx, 0f);
    }
  }
}
