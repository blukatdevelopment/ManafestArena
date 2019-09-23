/* A menu that is manipulated by MenuInputHandler
*/
using Godot;
using System;
using System.Collections.Generic;
public interface IInputHandledMenu {
  void Select();
  void Pause();
  void Back();
  void Move(Vector2 direction);
}