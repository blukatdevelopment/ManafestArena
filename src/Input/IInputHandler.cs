// A class that maps inputs to particular actions when controlling
// some entity such as an actor, vehicle, or menu
public interface IInputHandler {
  void RegisterInputSource(IInputSource source);
  void Update(float delta);
}