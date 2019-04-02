/*
  Abstract away the namespaced variables and methods used for one behavior
  of an AI.
*/
public interface IBehaviorState {
  void Init(StateAi hostAi);
  void Update(float delta);
}