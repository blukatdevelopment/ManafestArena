/*
  Just there to display what a given InputSource is outputting on the console.

*/
using Godot;
using System;
using System.Collections.Generic;

public class DebugInputHandler : IInputHandler{
  public IInputSource source;
  public long lineNumber; // Differentiate each line

  public DebugInputHandler(){

  }

  public void RegisterInputSource(IInputSource source){
    this.source = source;
  }

  public void Update(float delta){
    List<MappedInputEvent> inputs = source.GetInputs(delta);

    foreach(MappedInputEvent input in inputs){
      GD.Print(lineNumber + input.ToString());
      lineNumber++;
    }
  }
}