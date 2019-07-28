using Godot;
using System;
using System.Collections.Generic;

public class MappedInputEvent {

  public enum Inputs{
    Release,
    Press,
    Hold,
    LongPress,
    LongRelease
  };

  public float inputValue;
  public int mappedEventId;
  public Inputs inputType;

  public MappedInputEvent(float inputValue, int mappedEventId, Inputs inputType){
    this.inputValue = inputValue;
    this.mappedEventId = mappedEventId;
    this.inputType = inputType;
  }

  new public string ToString(){
    string output = "[ID: " + mappedEventId + ", ";
    output += "type: " + inputType + ", ";
    output += "value: " + inputValue + "]";
    return output;
  }

  public static MappedInputEvent Release(float inputValue, int mappedEventId){
    return new MappedInputEvent(inputValue, mappedEventId, Inputs.Release);
  }

  public static MappedInputEvent Press(float inputValue, int mappedEventId){
    return new MappedInputEvent(inputValue, mappedEventId, Inputs.Press);
  }

  public static MappedInputEvent Hold(float inputValue, int mappedEventId){
    return new MappedInputEvent(inputValue, mappedEventId, Inputs.Hold);
  }

  public static MappedInputEvent LongPress(float inputValue, int mappedEventId){
    return new MappedInputEvent(inputValue, mappedEventId, Inputs.LongPress);
  }

  public static MappedInputEvent LongRelease(float inputValue, int mappedEventId){
    return new MappedInputEvent(inputValue, mappedEventId, Inputs.LongRelease);
  }
}