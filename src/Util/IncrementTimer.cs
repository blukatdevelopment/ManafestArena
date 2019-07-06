public class IncrementTimer {
  public float delay, currentTime;

  public IncrementTimer(float delay){
    this.delay = delay;
  }

  public bool CheckTimer(float delta){
    currentTime += delta;
    if(currentTime >= delay){
      currentTime = 0f;
      return true;
    }
    return false;
  }
}