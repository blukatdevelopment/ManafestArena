public class IncrementTimer {
  public float delay, currentTime;

  public IncrementTimer(float delay){
    this.delay = delay;
  }

  public void StartTimer(float currentTime=0){
    this.currentTime = currentTime;
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