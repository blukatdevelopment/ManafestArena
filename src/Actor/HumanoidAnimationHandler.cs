/*
  Handles animation state logic for arms and legs using two different AnimationPlayers
*/
using Godot;
using System;
using System.Collections.Generic;

public class HumanoidAnimationHandler {
  const float StopWalkingDelay = 0.05f;
  IncrementTimer stopWalkingTimer;
  bool walking, crouching, holding;

  AnimationPlayer arms, legs;
  
  // Arms animations
  const string AtRest   = "Both_Rest";
  const string Hold     = "Single_Hold";
  const string Punch    = "Single_Punch";
  const string Stab     = "Single_Stab";
  const string Slash    = "Single_Slash";

  // Legs animations
  const string Stand      = "Standing";
  const string Crouch     = "Crouching";
  const string Walk       = "Standing_Walk";
  const string WalkCrouch = "Crouching_Walk";

  public HumanoidAnimationHandler(AnimationPlayer arms, AnimationPlayer legs){
    this.arms = arms;
    this.legs = legs;
    stopWalkingTimer = new IncrementTimer(StopWalkingDelay);
    walking = false;
    crouching = false;
    arms.Play(AtRest);
    legs.Play(Stand);
    arms.PlaybackDefaultBlendTime = 0.25f;
    legs.PlaybackDefaultBlendTime = 0.25f;
  }

  public void HandleMovement(){
    stopWalkingTimer.currentTime = 0f;
    if(walking){
      return;
    }
    walking = true;
    if(crouching){
      legs.Play(WalkCrouch);
    }
    else{
      legs.Play(Walk);
    }
  }

  public void HandleCrouch(bool val){
    if(val == crouching){
      return;
    }
    crouching = val;
    if(crouching){
      if(walking){
        legs.Play(WalkCrouch);
      }
      else{
        legs.Play(Crouch);
      }
    }
    else{
      if(walking){
        legs.Play(Walk);
      }
      else{
        legs.Play(Stand);
      }
    }

  }

  public void HandleHold(bool val){
    holding = val;
    if(holding){
      arms.Play(Hold);
    }
    else{
      arms.Play(AtRest);
    }
  }

  public void AnimationTrigger(string triggerName){
    if(triggerName.ToLower() == "dead"){
      arms.Play(AtRest);
      legs.Play(Stand);
      return;
    }

    string animation = "";
    if(holding){
      switch(triggerName.ToLower()){
        case "punch":
          animation = Punch;
        break;
        case "stab":
          animation = Stab;
        break;
        case "slash":
          animation = Slash;
        break;
      }
      if(animation != ""){
        arms.Play(animation);
        arms.Queue(Hold);
        return;
      }
    }
  }

  public void Update(float delta){
    if(!walking || !stopWalkingTimer.CheckTimer(delta)){
      return;
    }
    walking = false;
    if(crouching){
      legs.Play(Crouch);
    }
    else{
      legs.Play(Stand);
    }
  }
}