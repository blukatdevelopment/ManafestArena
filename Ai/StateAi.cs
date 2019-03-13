/*
  An AI that uses objects implementing IBehaviorState as states in a state machine.

  In addition to managing the state transitions, this class holds some methods 
  and variables to be used by active state.
*/
using Godot;
using System;
using System.Collections.Generic;

public class StateAi : Brain {
  public enum States {
    Idling,
    Roaming,
    Pursuing,
    Fighting,
    Ranged,
    Melee
  };

  public const States DefaultState = States.Roaming;
  public IBehaviorState activeState;
  public Actor host;

  public List<Actor> enemies;

  public StateAi(Actor actor) : base (actor){
    host = actor;
    enemies = new List<Actor>();
  }

  public override void Update(float delta){
    if(host == null){
      return;
    }

    if(host.IsPaused()){
      return;
    }
    
    if(activeState == null){
      ChangeState(DefaultState);
    }

    activeState.Update(delta);
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

  public List<Item> GetItems(){
    return actor.GetHotbarItems();
  }
  
  public List<Actor> ActorsInSight(){
    Vector3 start = host.GlobalHeadPosition();
    Vector3 end = host.Pointer();
    World world = host.GetWorld();

    List<object> objects = Util.GridCast(start, end, world, 3, 5f);
    List<Actor> ret = new List<Actor>();
    foreach(object obj in objects){
      Actor sighted = obj as Actor;
      
      if(sighted != null){
        ret.Add(sighted);
      }
    }

    return ret;
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
    return actor.brainType == Actor.Brains.Player1;
  }

  // TODO: Replace hardcoding with logic
  public float GetCombatRange(){
    return 20f;
  }

  public float DistanceToActor(Actor actor){
    if(actor == null){
      GD.Print("Actor.DistanceToActor: actor null");
      return 0.0f;
    }
    Vector3 hostPosition = host.GetGlobalTransform().origin;
    Vector3 actorPosition = actor.GetGlobalTransform().origin;
    return hostPosition.DistanceTo(actorPosition);
  }

  public void AimAt(Vector3 point){
    // This is local to the parent, which is not an ideal solution.
    
    Transform hostTrans = host.Transform;    
    Transform lookingAt = hostTrans.LookingAt(point, host.Up());
    
    // Get horizontal rotation based on host body.
    Vector3 hostRot = host.GetRotationDegrees();
    
    // Get vertical Rotation based on head when possible.
    hostRot.x = host.RotationDegrees().x;
    
    Vector3 lookingRot = lookingAt.basis.GetEuler();
    lookingRot = Util.ToDegrees(lookingRot);
    Vector3 turnRot = (lookingRot - hostRot);
    host.Turn(turnRot.y, turnRot.x);
  }

  public bool IsAimedAt(Vector3 point, float aimMargin){
    Vector3 hostRot = host.GetRotationDegrees();
    Transform aimedTrans = host.Transform.LookingAt(point, host.Up());
    
    Vector3 aimedRot = aimedTrans.basis.GetEuler();
    aimedRot = Util.ToDegrees(aimedRot);
    
    float aimAngle = hostRot.DistanceTo(aimedRot);
    
    return aimAngle < aimMargin;
  }
}