public class IncrementTimer {
  public float delay, currentTime;
  public bool ready;

  public IncrementTimer(float delay){
    this.delay = delay;
  }

  // Used for countdown
  public bool CheckTimer(float delta){
    currentTime += delta;
    if(currentTime >= delay){
      currentTime = 0f;
      return true;
    }
    return false;
  }

  // Used for cooldown
  public void UpdateTimerReady(float delta){
    if(ready){
      return;
    }
    currentTime += delta;
    if(currentTime >= delay){
      ready = true;
    }
  }

  public bool CheckTimerReady(){
    if(ready){
      ready = false;
      currentTime = 0f;
      return true;
    }
    return false;
  }

  public void Clear(){
    currentTime = 0f;
  }

}