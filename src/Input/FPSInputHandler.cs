/*
  Control an actor in the capacity of 3D first-person shooter gameplay.
*/
using Godot;
using System;
using System.Collections.Generic;

public class FPSInputHandler : IInputHandler {
  public Actor actor;
  private IInputSource source;

  public FPSInputHandler(){}

  public FPSInputHandler(Actor actor){
    this.actor = actor;
  }

  public void RegisterInputSource(IInputSource source){
    this.source = source;
  }
  public void Update(float delta){
    List<MappedInputEvent> inputEvents = source.GetInputs(delta);
    foreach(MappedInputEvent inputEvent in inputEvents){
      HandleSingleInput(inputEvent, delta);
    }
  }

  public void HandleSingleInput(MappedInputEvent inputEvent, float delta){
    Inputs input = (Inputs)inputEvent.mappedEventId;
    float val = inputEvent.inputValue;
    switch(input){
      case Inputs.MoveForward:
        actor.Move(new Vector3(0, 0, -val), delta);
        break;
      case Inputs.MoveBackward:
        actor.Move(new Vector3(0, 0, val), delta);
        break;
      case Inputs.MoveLeft:
        actor.Move(new Vector3(-val, 0, 0), delta); 
        break;
      case Inputs.MoveRight:
        actor.Move(new Vector3(val, 0, 0), delta);
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
      (int)Inputs.MoveBackward,
      0f,
      1f
    ));

    mappings.Add(new InputMapping(
      InputMapping.Inputs.KeyboardKey,
      83,
      (int)Inputs.MoveLeft,
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