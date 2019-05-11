/*
  Use FPS controls to control an IMenu
*/
using Godot;
using System;
using System.Collections.Generic;


public class MenuInputHandler : IInputHandler {
  IInputSource source;
  IInputHandledMenu menu;

  public MenuInputHandler(IInputHandledMenu menu){
    this.menu = menu;
  }

  public void RegisterInputSource(IInputSource source){
    this.source = source;
  }

  public void Update(float delta){
    List<MappedInputEvent> inputEvents = source.GetInputs(delta);
    foreach(MappedInputEvent inputEvent in inputEvents){
      HandleSingleInput(inputEvent);
    }
  }

  private void HandleSingleInput(MappedInputEvent inputEvent){
    if(inputEvent.inputType != MappedInputEvent.Inputs.Press){
      return;
    }
    FPSInputHandler.Inputs input = (FPSInputHandler.Inputs)inputEvent.mappedEventId;
    switch(input){
      case FPSInputHandler.Inputs.Inventory:
        menu.Back();
      break;
      case FPSInputHandler.Inputs.Pause:
        menu.Pause();
      break;
      case FPSInputHandler.Inputs.Jump:
        menu.Select();
      break;
      case FPSInputHandler.Inputs.MoveForward:
        menu.Move(new Vector2(0f, 1f));
      break;
      case FPSInputHandler.Inputs.MoveBackward:
        menu.Move(new Vector2(0f, -1f));
      break;
      case FPSInputHandler.Inputs.MoveRight:
        menu.Move(new Vector2(1f, 0f));
      break;
      case FPSInputHandler.Inputs.MoveLeft:
        menu.Move(new Vector2(1f, 0f));
      break;
    }
  }
}