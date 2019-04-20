/*
  Reads device state to create MappedInputEvents

  inputType and inputId are used to register and listen to a DeviceState
  
  The mappedEventId is an int that can be converted to one of many enums
  specific to a particular input handler.  

  The deadzone and sensitivity only apply to axes, whereby they 
  respectively filter out noise and magnify or reduce input value.
  A negative sensitivity inverts an axis.
  
*/
using System.Collections.Generic;

public class InputMapping {
  public enum Inputs {
    JoyButton,
    JoyAxis,
    MouseButton,
    MouseAxis
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
        ret = device.GetKey(inputId);
        ret = ApplyDeadZone(ret);
        ret = ApplySensitivity(ret);
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

}