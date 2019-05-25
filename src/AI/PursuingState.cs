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

  public const float DefaultCombatRange = 10f;
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
    
    if(DefaultCombatRange > hostAi.DistanceToActor(enemy)){
      return true;
    }

    return false;
  }

  public void Pursue(float delta){
    Spatial enemySpat = enemy.GetNode() as Spatial;
    if(enemySpat == null){
      GD.Print("Enemy has no spatial");
      return;
    }
    Vector3 targetPos = enemySpat.Translation;

    hostAi.AimAt(targetPos);
    hostAi.Hold(FPSInputHandler.Inputs.MoveForward);
  }

}