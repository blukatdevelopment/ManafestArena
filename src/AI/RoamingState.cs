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
    hostAi.enemies = new List<Actor>();
  }

  public void Update(float delta){
    if(EnemyCheck(delta)){
      GD.Print("Enemies sighted");
      hostAi.ChangeState(StateAi.States.Pursuing);
    }
    //Wander();
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
  public void Wander(){
    
    for(int i = 0; i < 15; i++){
      hostAi.Hold(FPSInputHandler.Inputs.LookRight);
    }
    hostAi.Hold(FPSInputHandler.Inputs.MoveForward);
  }
}