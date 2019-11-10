using Godot;

public interface IInputHandledMenu {
  void Select();
  void Pause();
  void Back();
  void Move(Vector2 direction);
}