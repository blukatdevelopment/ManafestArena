// Creates a list of MappedInputEvent objects
// Plug AI and input mapping classes in this way
// to decouple the IInput Handlers
using Godot;
using System;
using System.Collections.Generic;

public interface IInputSource {
  List<MappedInputEvent> GetInputs(); 
}