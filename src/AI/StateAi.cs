/*
  An AI that uses objects implementing IBehaviorState as states in a state machine.

  In addition to managing the state transitions, this class holds some methods 
  and variables to be used by active state.
*/
using Godot;
using System;
using System.Collections.Generic;

public class StateAi : IInputSource {
  public enum States {
    Idling,
    Roaming,
    Pursuing,
    Fighting,
    Ranged,
    Melee
  };

  public const States DefaultState = States.Roaming;
  public const float MaxTurnRate = 50f;
  public const float TurnModifier = 5f; // 1 degree != 1f movement value
  public IBehaviorState activeState;
  public Actor host;
  public List<MappedInputEvent> inputEvents;

  public List<Actor> enemies;

  public StateAi(Actor actor){
    this.host = actor;
    this.enemies = new List<Actor>();
  }

  public List<MappedInputEvent> GetInputs(float delta){
    if(host == null){
      return new List<MappedInputEvent>();
    }
    if(activeState == null){
      ChangeState(DefaultState);
    }
    inputEvents = new List<MappedInputEvent>();

    activeState.Update(delta);

    return inputEvents;
  }

  public void ChangeState(States state){
    GD.Print("Changing state to " + state);
    activeState = StateFactory(state);
  }

  public IBehaviorState StateFactory(States state){
    IBehaviorState ret = null;
    switch(state){
      case States.Idling:
        break;
      case States.Roaming:
        ret = new RoamingState(host) as IBehaviorState;
        break;
      case States.Pursuing:
        ret = new PursuingState(host) as IBehaviorState;
        break;
      case States.Fighting:
        ret = new FightingState(host) as IBehaviorState;
        break;
      case States.Ranged:
        ret = new RangedCombatState(host) as IBehaviorState;
        break;
      case States.Melee:
        ret = new MeleeCombatState(host) as IBehaviorState;
        break;
    }

    if(ret != null){
      ret.Init(this);
    }

    return ret;
  }

  //############################################################################
  //#     State convenience methods                                            #
  //############################################################################
  
  public void Press(FPSInputHandler.Inputs input, float val = 1f){
    inputEvents.Add(MappedInputEvent.Press(val, (int)input));
  }

  public void Hold(FPSInputHandler.Inputs input, float val = 1f){
    inputEvents.Add(MappedInputEvent.Hold(val, (int)input));
  }

  public List<Actor> ActorsInSight(){
    if(host.body == null){
      return new List<Actor>();
    }
    return host.body.ActorsInSight();
  }

  public List<Actor> EnemiesInSight(){
    List<Actor> actorsInSight = ActorsInSight();
    List<Actor> ret = new List<Actor>();

    foreach(Actor actor in actorsInSight){
      if(ActorIsEnemy(actor)){
        ret.Add(actor);
      }
    }

    return ret;
  }

  // TODO: Make this rely on StatsManager
  public bool ActorIsEnemy(Actor actor){
    return true;
  }

  // TODO: Replace hardcoding with logic
  public float GetCombatRange(){
    return 20f;
  }

  public float DistanceToActor(Actor actor){
    Spatial hostSpat = host.GetNode() as Spatial;
    Spatial actorSpat = actor.GetNode() as Spatial;

    if(hostSpat == null || actorSpat == null){
      GD.Print("Cannot calculate distance between nulls");
      return 0.0f;
    }

    Vector3 hostPosition = hostSpat.GetGlobalTransform().origin;
    Vector3 actorPosition = actorSpat.GetGlobalTransform().origin;
    return hostPosition.DistanceTo(actorPosition);
  }

  public void AimAt(Vector3 point){
    // This is local to the parent, which is not an ideal solution.
    Spatial hostSpat = host.GetNode() as Spatial;
    IBody body = host.GetBody();
    if(hostSpat == null || body == null){
      GD.Print("need spatial and IBody for actor");
      return;
    }

    Transform hostTrans = hostSpat.Transform;
    Transform lookingAt = hostTrans.LookingAt(point, Util.TUp(hostTrans));
    
    Vector3 hostRot = body.LookingDegrees();
    
    Vector3 lookingRot = lookingAt.basis.GetEuler();
    lookingRot = Util.ToDegrees(lookingRot);
    Vector3 turnRot = (lookingRot - hostRot);
    turnRot.x = PickShorterDirection(turnRot.x);
    turnRot.y = PickShorterDirection(turnRot.y);


    float xMag = Math.Min(Math.Abs(turnRot.x), MaxTurnRate) * TurnModifier;
    FPSInputHandler.Inputs xDir = turnRot.x < 0 ? FPSInputHandler.Inputs.LookDown : FPSInputHandler.Inputs.LookUp;
    Hold(xDir, xMag);

    float yMag = Math.Min(Math.Abs(turnRot.y), MaxTurnRate) * TurnModifier;
    FPSInputHandler.Inputs yDir = turnRot.y < 0 ? FPSInputHandler.Inputs.LookLeft : FPSInputHandler.Inputs.LookRight;
    Hold(yDir, yMag);
  }

  // Why turn 181 degrees when you can turn -179 ?
  public float PickShorterDirection(float val){
    if(val > -180 && val < 180){
      return val;
    }

    float direction = val < 0 ? 1f : -1f;
    val = Math.Abs(val);
    val -= 180;
    val = 180 - val;

    return val * direction;
  }

  public bool IsAimedAt(Vector3 point, float aimMargin){
    Spatial hostSpat = host.GetNode() as Spatial;
    if(hostSpat == null){
      GD.Print("Need a host spatial");
      return false;
    }


    Vector3 hostRot = hostSpat.GetRotationDegrees();
    Vector3 up = Util.TUp(hostSpat.Transform);
    Transform aimedTrans = hostSpat.Transform.LookingAt(point, up);
    
    Vector3 aimedRot = aimedTrans.basis.GetEuler();
    aimedRot = Util.ToDegrees(aimedRot);
    
    float aimAngle = hostRot.DistanceTo(aimedRot);
    
    return aimAngle < aimMargin;
  }
}