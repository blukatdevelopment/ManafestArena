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
    foreach(int joypad in ConnectedJoypads()){
      GD.Print("Joypad " + joypad + "is connected");
    }
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

  public static List<InputMapping> GetMappings(){
    List<InputMapping> mappings = new List<InputMapping>();
    for(int i = 0; i< 200; i++){
      mappings.Add(new InputMapping(
        InputMapping.Inputs.KeyboardKey,
        i,
        i
      ));
      mappings.Add(new InputMapping(
        InputMapping.Inputs.JoyButton,
        i,
        i
      ));
      mappings.Add(new InputMapping(
        InputMapping.Inputs.JoyButton,
        i,
        i
      ));
      mappings.Add(new InputMapping(
        InputMapping.Inputs.JoyAxis,
        i,
        i,
        1
      ));
    }
    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      0,
      0
    ));
    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      1,
      1
    ));
    mappings.Add(new InputMapping(
      InputMapping.Inputs.MouseButton,
      2,
      2
    ));
    return mappings;
  }

  public static List<int> ConnectedJoypads(){
    List<System.Object> joypads = Util.ArrayToList(Input.GetConnectedJoypads());
    List<int> ret = new List<int>();
    foreach(System.Object obj in joypads){
      ret.Add((int)obj);
    }
    return ret;
  }

  public static bool JoypadConnected(int joyId){
    return ConnectedJoypads().Contains(joyId);
  }

}