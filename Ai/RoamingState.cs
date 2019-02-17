/*
  An AI behavior that roams about until it finds an enemy.
*/
using Godot;
using System;
using System.Collections.Generic;

public class RoamingState : IBehaviorState {
  public Actor hostActor;
  public StateAi hostAi;

  public const float EnemyCheckInterval = 0.25f;
  public float enemyCheckTimer;

  public RoamingState(Actor actor){
    hostActor = actor;
    enemyCheckTimer = 0f;
  }

  public void Init(StateAi hostAi){
    this.hostAi = hostAi;
  }

  public void Update(float delta){
    if(EnemyCheck(delta)){
      hostAi.ChangeState(StateAi.States.Pursuing);
    }
    Wander(delta);
  }

  public bool EnemyCheck(float delta){
    enemyCheckTimer += delta;
    
    if(enemyCheckTimer < EnemyCheckInterval){
      return false;
    }

    enemyCheckTimer = 0f;

    List<Actor> sightedEnemies = hostAi.EnemiesInSight();
    if(sightedEnemies.Count == 0){
      return false;
    }

    hostAi.enemies = sightedEnemies;
    return true;
  }

  // TODO: Make this less dumb
  public void Wander(float delta){
    hostActor.Turn(1f, 0f);
    hostActor.Move(new Vector3(0, 0, -hostActor.GetMovementSpeed()), delta);
  }

}