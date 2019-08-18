/*
  An AI behavior that roams about until it finds an enemy.
*/
using Godot;
using System;
using System.Collections.Generic;

public class RoamingState : IBehaviorState {
  public Actor hostActor;
  public StateAi hostAi;
  
  public RayCast rightRay;
  public RayCast leftRay;
  public const float RayDist = 7;
  public const float rayAngle = 0.5f;//maybe try different values
  public const float rayCastHeight = 2;
  public const float dirInterval = 3;
  public float dirTimer;
  public Dir curDir;
  public const float EnemyCheckInterval = 0.25f;
  public float enemyCheckTimer;

  public RoamingState(Actor actor){
    hostActor = actor;
    enemyCheckTimer = 0f;
    dirTimer = 0;
    rightRay = new RayCast();
    leftRay = new RayCast();

    rightRay.CastTo = Vector3.Forward*RayDist;
    leftRay.CastTo = Vector3.Forward*RayDist;

    hostActor.body.GetNode().AddChild(rightRay);
    hostActor.body.GetNode().AddChild(leftRay);

    rightRay.Translate(new Vector3(0, rayCastHeight, 0f));
    leftRay.Translate(new Vector3(0, rayCastHeight, 0f));

    rightRay.RotateY(-rayAngle);
    leftRay.RotateY(rayAngle);
  }

  public void Init(StateAi hostAi){
    rightRay.Enabled = true;
    leftRay.Enabled = true;
    this.hostAi = hostAi;
    hostAi.enemies = new List<Actor>();
  }

  public void Fin(){
    rightRay.QueueFree();
    leftRay.QueueFree();
  }

  public void Update(float delta){
    if(dirTimer>0){
      dirTimer-=delta;
    }
    else{
      dirTimer = 0;
    }
    if(EnemyCheck(delta)){
      GD.Print("Enemies sighted");
      Fin();
      hostAi.ChangeState(StateAi.States.Pursuing);
    }
    Wander();
  }

  public bool EnemyCheck(float delta){
    //return false;
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

  // TODO: Improve this, maybe use navmesh
  public void Wander(){
    if(rightRay.IsColliding() && leftRay.IsColliding()){
      if(dirTimer==0){
        dirTimer = dirInterval;
        curDir = Util.RandInt(0,1,true)==0 ?Dir.Right:Dir.Left;
      }
      hostAi.Hold(InputFromDir(curDir),90);
    }
    else if(rightRay.IsColliding()){
      if(dirTimer==0){
        curDir = Dir.Right;
      }
      hostAi.Hold(InputFromDir(curDir),90);
    }
    else if(leftRay.IsColliding()){
      if(dirTimer==0){
        curDir = Dir.Left;
      }
      hostAi.Hold(InputFromDir(curDir),90);
    }
    else{
      hostAi.Hold(FPSInputHandler.Inputs.MoveForward);
    }
  }

  public enum Dir{
    Right,
    Left
  }

  public FPSInputHandler.Inputs InputFromDir(Dir dir){
    if(dir==Dir.Right){
      return FPSInputHandler.Inputs.LookRight;
    }
    else{
      return FPSInputHandler.Inputs.LookLeft;
    }
  }
}