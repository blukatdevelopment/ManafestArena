/*
  An IInputSource that connects some InputMapping objects to a DeviceState
  in order to return all current inputs.
*/
using System.Collections.Generic;
  
public class MappedInputSource : IInputSource {
  public DeviceState device;
  public List<InputMapping> mappings;

  public MappedInputSource(DeviceState device, List<InputMapping> mappings){
    this.device = device;
    this.mappings = mappings;

    foreach(InputMapping mapping in mappings){
      mapping.RegisterDevice(device);
    }
  }

  public List<MappedInputEvent> GetInputs(float delta){
    List<MappedInputEvent> events = new List<MappedInputEvent>();

    foreach(InputMapping mapping in mappings){
      events.AddRange(mapping.GetEvents(device, delta));
    }

    return events;
  }
}