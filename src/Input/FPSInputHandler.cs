/*
  Control an actor in the capacity of 3D first-person shooter gameplay.
*/
using Godot;
using System;
using System.Collections.Generic;

public class FPSInputHandler : IInputHandler {
  public Actor actor;
  private IInputSource source;
  private float speed;
  public const float SpeedBase = 3f;

  public FPSInputHandler(){}

  public FPSInputHandler(Actor actor){
    this.actor = actor;
    SetSpeed(false);
  }

  public void RegisterInputSource(IInputSource source){
    this.source = source;
  }
  public void Update(float delta){
    if(actor.body == null){
      GD.Print("No body, therefore no input handling ¯\\_(ツ)_/¯");
      return;
    }
    List<MappedInputEvent> inputEvents = source.GetInputs(delta);
    foreach(MappedInputEvent inputEvent in inputEvents){
      HandleSingleInput(inputEvent, delta);
    }
  }

  public void SetSpeed(bool sprinting){
    GD.Print("Set speed " + sprinting);
    if(actor != null && actor.stats != null){
      speed = (float)actor.stats.GetStat("speed") / 100f;
      speed += SpeedBase;
      GD.Print("Stat speed = " + speed);
      if(sprinting){
        speed += ((float)actor.stats.GetStat("sprintbonus"))/ 100f;
      }
    }
    else{
      if(sprinting){
        speed = SpeedBase * 2;
      }
      else{
        speed = SpeedBase;
      }
    }
    GD.Print("Speed set to " + speed);

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
        break;
      case Inputs.SecondaryUse: 
        break;
      case Inputs.Reload: 
        break;
      case Inputs.Interact: 
        break;
      case Inputs.Sprint: 
        if(inputEvent.inputType == MappedInputEvent.Inputs.Release){
          SetSpeed(false);
        }
        else if(inputEvent.inputType == MappedInputEvent.Inputs.Press){
          SetSpeed(true);
        }
        break;
      case Inputs.Crouch: 
        break;
      case Inputs.Jump: 
        break;
      case Inputs.Inventory: 
        break;
      case Inputs.NextItem: 
        break;
      case Inputs.PreviousItem: 
        break;
      case Inputs.Pause: 
        break;
      case Inputs.LookUp:

        break;
      case Inputs.LookDown: 
        break;
      case Inputs.LookLeft: 
        break;
      case Inputs.LookRight: 
        break;
    }
  }

  private void HandleMovement(MappedInputEvent input, float delta, Vector3 direction){
    if(input.inputType != MappedInputEvent.Inputs.Press && input.inputType != MappedInputEvent.Inputs.Hold){
      return;
    }
    direction *= (speed * SpeedBase);
    direction *= input.inputValue;
    actor.body.Move(direction, delta);
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
    LookRight
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
      InputMapping.Inputs.KeyboardKey,
      82,
      (int)Inputs.Reload,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      69,
      (int)Inputs.Interact,
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
      (int)Inputs.NextItem,
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
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      1,
      (int)Inputs.LookDown,
      0f,
      -1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      0,
      (int)Inputs.LookLeft,
      0f,
      -1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseAxis,
      0,
      (int)Inputs.LookRight,
      0f,
      1f
    ));

    
    return mappings; 
  }
}