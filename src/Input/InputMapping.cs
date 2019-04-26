/*
  Reads device state to create MappedInputEvents

  inputType and inputId are used to register and listen to a DeviceState
  
  The mappedEventId is an int that can be converted to one of many enums
  specific to a particular input handler.  

  The deadzone and sensitivity only apply to axes, whereby they 
  respectively filter out noise and magnify or reduce input value.
  A negative sensitivity inverts an axis.
  
*/
using Godot;
using System;
using System.Collections.Generic;

public class InputMapping {
  public enum Inputs {
    JoyButton,
    JoyAxis,
    MouseButton,
    MouseAxis,
    KeyboardKey
  };

  public const float LongHoldTime = 1f;

  public Inputs inputType;
  public int inputId;
  public int mappedEventId;
  public float lastValue;
  public float deadZone;
  public float sensitivity;
  public float heldTime;
  public bool longPressed;

  public InputMapping(
    Inputs inputType, 
    int inputId, 
    int mappedEventId, 
    float deadZone = 0f,
    float sensitivity = 1f
  ){
    this.inputType = inputType;
    this.inputId = inputId;
    this.mappedEventId = mappedEventId;
    this.deadZone = deadZone;
    this.sensitivity = sensitivity;
    
    lastValue = 0f;
    heldTime = 0f;
    longPressed = false;
  }

  public void RegisterDevice(DeviceState device){
    switch(inputType){
      case Inputs.JoyButton:    device.AddJoyButton(inputId); break;
      case Inputs.JoyAxis:      device.AddJoyAxis(inputId); break;
      case Inputs.MouseButton:  device.AddMouseButton(inputId); break;
      case Inputs.KeyboardKey:  device.AddKey(inputId); break;
    }
  }

  public List<MappedInputEvent> GetEvents(DeviceState device, float delta){
    float currentValue = GetCurrentValue(device);
    List<MappedInputEvent> ret = new List<MappedInputEvent>();

    if(currentValue != 0){
      heldTime += delta;
    }

    if(longPressed && currentValue == 0){
      longPressed = false;
      heldTime = 0f;
      ret.Add(MappedInputEvent.LongRelease(currentValue, mappedEventId));
    }
    else if(!longPressed && currentValue != 0 && heldTime > LongHoldTime){
      longPressed = true;
      ret.Add(MappedInputEvent.LongPress(currentValue, mappedEventId));
    }
    else if(currentValue == 0f && lastValue != 0f){
      ret.Add(MappedInputEvent.Release(currentValue, mappedEventId));
      heldTime = 0f;
    }
    else if(currentValue != 0f && lastValue == 0f){
      ret.Add(MappedInputEvent.Press(currentValue, mappedEventId));
    }
    else if(currentValue != 0f){
      ret.Add(MappedInputEvent.Hold(currentValue, mappedEventId));
    }
    lastValue = currentValue;

    return ret;
  }

  public float GetCurrentValue(DeviceState device){
    float ret = 0f;
    switch(inputType){
      case Inputs.JoyButton:
        ret = device.GetJoyButton(inputId); 
        break;
      case Inputs.JoyAxis:
        ret = device.GetJoyAxis(inputId);
        ret = ApplyDeadZone(ret);
        ret = ApplySensitivity(ret);
        break;
      case Inputs.MouseButton:
        ret = device.GetMouseButton(inputId); 
        break;
      case Inputs.MouseAxis:
        ret = device.GetMouseAxis(inputId);
        ret = ApplyDeadZone(ret);
        ret = ApplySensitivity(ret);
        break;
      case Inputs.KeyboardKey:
        ret = device.GetKey(inputId);
        break;
    }
    return ret;
  }

  public float ApplyDeadZone(float val){
    if(val < deadZone && val > -deadZone){
      return 0f;
    }
    return val;
  }

  public float ApplySensitivity(float val){
    return sensitivity * val;
  }

  // Convert to and from string represantation
  public string Flatten(){
    string ret = "";
    ret += (int)inputType + ";";
    ret += inputId + ";";
    ret += mappedEventId + ";";
    ret += deadZone + ";";
    ret += sensitivity;
    return ret;
  }

  public static InputMapping Unflatten(string flat){
    GD.Print("Unflattening " + flat);
    string[] values = flat.Split(';');
    if(values.Length != 5){
      GD.Print("Invalid flat mapping: " + flat);
    }
    Inputs inputType = (Inputs)Util.ToInt(values[0]);
    int inputId = Util.ToInt(values[1]);
    int mappedEventId = Util.ToInt(values[2]);
    float deadZone = Util.ToFloat(values[3]);
    float sensitivity = Util.ToFloat(values[4]);
    return new InputMapping( inputType, inputId, mappedEventId, deadZone, sensitivity);
  }

}