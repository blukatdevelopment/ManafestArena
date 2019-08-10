/*
  Control an actor in the capacity of 3D first-person shooter gameplay.
*/
using Godot;
using System;
using System.Collections.Generic;

public class FPSInputHandler : IInputHandler {
  public Actor actor;
  private IInputSource source;
  private float walkSpeed, sprintSpeed;
  private bool activelySprinting, sprintWasPressed;

  public const float SpeedBase = 3f;
  public const float DefaultMouseSensitivity = 4f;

  public FPSInputHandler(){}

  public FPSInputHandler(Actor actor){
    this.actor = actor;
    UpdateSpeed();
  }

  public void RegisterInputSource(IInputSource source){
    this.source = source;
  }
  public void Update(float delta){
    if(actor.stats != null){
      UpdateSpeed();
    }
    if(actor.body == null){
      GD.Print("No body, therefore no input handling ¯\\_(ツ)_/¯");
      return;
    }
    List<MappedInputEvent> inputEvents = source.GetInputs(delta);
    sprintWasPressed = false;
    foreach(MappedInputEvent inputEvent in inputEvents){
      HandleSingleInput(inputEvent, delta);
    }
    if(activelySprinting && !sprintWasPressed){
      activelySprinting = false;
    }
  }

  public void UpdateSpeed(){
    if(actor != null && actor.stats != null){
      walkSpeed = (float)actor.stats.GetStat("speed") / 100f;
      walkSpeed += SpeedBase;
      
      sprintSpeed = walkSpeed + ((float)actor.stats.GetStat("sprintbonus"))/ 100f;
    }
    else{
      walkSpeed = SpeedBase;
      sprintSpeed = SpeedBase * 2;
    }
  }

  public void HandleSingleInput(MappedInputEvent inputEvent, float delta){
    Inputs input = (Inputs)inputEvent.mappedEventId;
    float val = inputEvent.inputValue;
    switch(input){
      case Inputs.MoveForward:
        HandleMovement(inputEvent, delta, new Vector3(0, 0, -1f));
        break;
      case Inputs.MoveBackward:
        HandleMovement(inputEvent, delta, new Vector3(0, 0, 1f));
        break;
      case Inputs.MoveLeft:
        HandleMovement(inputEvent, delta, new Vector3(-1f, 0, 0));
        break;
      case Inputs.MoveRight:
        HandleMovement(inputEvent, delta, new Vector3(1f, 0, 0f));
        break;
      case Inputs.PrimaryUse:
        inputEvent.mappedEventId = (int)Item.ItemInputs.A;
        actor.hotbar.UseEquippedItem(inputEvent);
        break;
      case Inputs.SecondaryUse:
        inputEvent.mappedEventId = (int)Item.ItemInputs.B;
        actor.hotbar.UseEquippedItem(inputEvent);
        break;
      case Inputs.Reload: 
        inputEvent.mappedEventId = (int)Item.ItemInputs.C;
        actor.hotbar.UseEquippedItem(inputEvent);
        break;
      case Inputs.ScrollUp:
        inputEvent.mappedEventId = (int)Item.ItemInputs.F;
        actor.hotbar.UseEquippedItem(inputEvent);
        break;
      case Inputs.ScrollDown:
        inputEvent.mappedEventId = (int)Item.ItemInputs.G;
        actor.hotbar.UseEquippedItem(inputEvent);
        break;
      case Inputs.Interact: 
        break;
      case Inputs.Sprint: 
        if(inputEvent.inputType == MappedInputEvent.Inputs.Release){
          activelySprinting = false;
        }
        else if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          activelySprinting = true;
          sprintWasPressed = true;
        }
        else if(inputEvent.inputType == MappedInputEvent.Inputs.Hold){
          activelySprinting = true; // Protect against missed release events
          sprintWasPressed = true;
        }
        break;
      case Inputs.Crouch: 
        if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          actor.body.ToggleCrouch();
        }
        break;
      case Inputs.Jump:
        if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          actor.body.Jump();
        }
        break;
      case Inputs.Inventory: 
        break;
      case Inputs.NextItem:
        if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          int start = actor.hotbar.GetEquippedSlot();
          actor.hotbar.EquipNext();
          GD.Print("Changed from " + start + " to " + actor.hotbar.GetEquippedSlot());
        }
        break;
      case Inputs.PreviousItem: 
        if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          actor.hotbar.EquipPrevious();
        }
        break;
      case Inputs.Pause:
        if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          Session.Event(SessionEvent.PauseEvent());
        }
        break;
      case Inputs.LookUp:
        HandleLook(new Vector3(0f, inputEvent.inputValue, 0f), delta);
        break;
      case Inputs.LookDown: 
        HandleLook(new Vector3(0f, -inputEvent.inputValue, 0f), delta);
        break;
      case Inputs.LookLeft: 
        HandleLook(new Vector3(-inputEvent.inputValue, 0f, 0f), delta);
        break;
      case Inputs.LookRight:
        HandleLook(new Vector3(inputEvent.inputValue, 0f, 0f), delta);
        break;
    }
  }

  private void HandleLook(Vector3 direction, float delta){
    if(actor.body != null){
      actor.body.Turn(direction, delta);
    } 
  }

  private void HandleMovement(MappedInputEvent input, float delta, Vector3 direction){
    if(input.inputType != MappedInputEvent.Inputs.Press && input.inputType != MappedInputEvent.Inputs.Hold){
      return;
    }
    float currentSpeed = walkSpeed;

    int sprintCost = 1;
    if(actor.stats != null){
      sprintCost = actor.stats.GetStat("sprintcost");
    }

    if(activelySprinting && actor.stats == null){
      currentSpeed = sprintSpeed;
    }
    else if(activelySprinting && actor.stats.ConsumeStat("stamina", sprintCost)){
      currentSpeed = sprintSpeed;
    }

    direction *= (currentSpeed * SpeedBase);
    direction *= input.inputValue;
    actor.body.Move(direction, delta, false, false);
  }

  public enum Inputs{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    PrimaryUse,
    SecondaryUse,
    Reload,
    Interact,
    Sprint,
    Crouch,
    Jump,
    Inventory,
    NextItem,
    PreviousItem,
    Pause,
    LookUp,
    LookDown,
    LookLeft,
    LookRight,
    ScrollUp,
    ScrollDown
  };

  public static List<InputMapping> GetMappings(){
    InitSettings();
    SettingsDb db = new SettingsDb();
    List<InputMapping> mappings = new List<InputMapping>();
    List<InputMapping> defaults = DefaultMappings();

    foreach(InputMapping mapping in defaults){
      string name = "FPS" + (Inputs)mapping.mappedEventId;
      string flatMapping = db.SelectSetting(name);
      InputMapping storedMapping = InputMapping.Unflatten(flatMapping);
      mappings.Add(storedMapping);
    }
    return mappings;
  }

  public static void InitSettings(){
    SettingsDb db = new SettingsDb();
    if(db.SelectSetting("FPSMoveForward") != ""){
      return;
    }

    GD.Print("Initializing default FPS control mappings");
    foreach(InputMapping mapping in DefaultMappings()){
      Inputs input = (Inputs)mapping.mappedEventId;
      db.StoreSetting("FPS" + input, mapping.Flatten());
    }

  }

  public static List<InputMapping> DefaultMappings(){
    List<InputMapping> mappings = new List<InputMapping>();
    
    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      87,
      (int)Inputs.MoveForward,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      65,
      (int)Inputs.MoveLeft,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      83,
      (int)Inputs.MoveBackward,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      68,
      (int)Inputs.MoveRight,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      1,
      (int)Inputs.PrimaryUse,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      2,
      (int)Inputs.SecondaryUse,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      (int)ButtonList.WheelUp,
      (int)Inputs.ScrollUp,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      (int)ButtonList.WheelDown,
      (int)Inputs.ScrollDown,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      82,
      (int)Inputs.Reload,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      69,
      (int)Inputs.ScrollUp,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      16777237,
      (int)Inputs.Sprint,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      16777238,
      (int)Inputs.Crouch,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      32,
      (int)Inputs.Jump,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      73,
      (int)Inputs.Inventory,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      81,
      (int)Inputs.ScrollDown,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      80,
      (int)Inputs.PreviousItem,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      16777217,
      (int)Inputs.Pause,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      1,
      (int)Inputs.LookUp,
      0f,
      -DefaultMouseSensitivity
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      1,
      (int)Inputs.LookDown,
      0f,
      DefaultMouseSensitivity
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      0,
      (int)Inputs.LookLeft,
      0f,
      DefaultMouseSensitivity
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      0,
      (int)Inputs.LookRight,
      0f,
      -DefaultMouseSensitivity
    ));

    
    return mappings; 
  }
}