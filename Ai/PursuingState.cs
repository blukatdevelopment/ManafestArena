/*
  An AI behavior that chases an enemy until it is in range finds an enemy.
*/
using Godot;
using System;
using System.Collections.Generic;

public class PursuingState : IBehaviorState {
  public Actor hostActor;
  public StateAi hostAi;
  public Actor enemy;

  public const float RangeCheckInterval = 0.1f;
  public float rangeCheckTimer;

  public PursuingState(Actor actor){
    hostActor = actor;
  }

  public void Init(StateAi hostAi){
    this.hostAi = hostAi;
    if(hostAi.enemies.Count == 0 || hostAi.enemies[0] == null){
      hostAi.ChangeState(StateAi.States.Roaming);
      return;
    }

    // TODO: Make this choose the closest enemy
    enemy = hostAi.enemies[0];
  }

  public void Update(float delta){
    if(enemy == null){
      hostAi.enemies = new List<Actor>();
      hostAi.ChangeState(StateAi.States.Roaming);
    }
    if(EnemyInRangeCheck(delta)){
      hostAi.ChangeState(StateAi.States.Fighting);
    }

    Pursue(delta);
  }

  public bool EnemyInRangeCheck(float delta){
    rangeCheckTimer += delta;
    if(rangeCheckTimer < RangeCheckInterval){
      return false;
    }
    
    if(hostAi.GetCombatRange() > hostAi.DistanceToActor(enemy)){
      return true;
    }

    return false;
  }

  public void Pursue(float delta){
    Vector3 pos = hostActor.Translation;
    Vector3 potentialPos = hostActor.Pointer(1f);
    Vector3 targetPos = enemy.Translation;

    hostAi.AimAt(targetPos);

    float currentDist = pos.DistanceTo(targetPos);
    float potentialDist = potentialPos.DistanceTo(targetPos);
    
    if(currentDist > potentialDist){
      hostActor.Move(new Vector3(0, 0, -hostActor.GetMovementSpeed()), delta);
    }
    
  }

}