/*
  An AI behavior that decides how to attack an enemy.
*/
using Godot;
using System;
using System.Collections.Generic;

public class MeleeCombatState : IBehaviorState {
  public Actor hostActor;
  public StateAi hostAi;
  public Actor enemy;
  public bool aimed;
  public bool inRange;


  public float attackTimer;
  public const float AttackInterval = 1f;
  public const float MeleeRange = 3f;
  public const float MeleeAimMargin = 0.5f;

  public float strafeTimer;
  public int strafeDirection;

  public MeleeCombatState(Actor actor){
    hostActor = actor;
    attackTimer = 0f;
  }

  public List<MappedInputEvent> GetInputs(float delta){
    return new List<MappedInputEvent>();
  }

  public void Init(StateAi hostAi){
    this.hostAi = hostAi;
    
    if(hostAi.enemies.Count == 0 || hostAi.enemies[0] == null){
      hostAi.ChangeState(StateAi.States.Roaming);
      return;
    }

    enemy = hostAi.enemies[0];
  }

  public void Update(float delta){
    if(enemy == null){
      hostAi.ChangeState(StateAi.States.Roaming);
    }
    Spatial hostSpat = hostActor.GetNode() as Spatial;
    Spatial enemySpat = enemy.GetNode() as Spatial;
    if(hostSpat == null || enemySpat == null){
      GD.Print("Both enemy and host need spatials to fight");
      return;
    }
    
    aimed = hostAi.IsAimedAt(enemySpat.Transform.origin, MeleeAimMargin);
    inRange = hostAi.DistanceToActor(enemy) <= MeleeRange;
    Strafe(delta);
    CloseDistance(enemySpat);
    Attack(delta);
  }

  public void Strafe(float delta){
    strafeTimer -= delta;
    if(strafeTimer > 0){
      if(strafeDirection == -1){
        hostAi.Hold(FPSInputHandler.Inputs.MoveLeft);
      }
      else if(strafeDirection == 1){
        hostAi.Hold(FPSInputHandler.Inputs.MoveRight);
      }
      return;
    }
    strafeDirection = Util.RandInt(-1, 1,true);
    strafeTimer = (float)Util.RandInt(0, 6,true) * 0.5f;
  }

  public void CloseDistance(Spatial enemySpat){
    if(!inRange && aimed){
      hostAi.Hold(FPSInputHandler.Inputs.MoveForward);
    }
    hostAi.AimAt(enemySpat.Transform.origin);
  }

  public void Attack(float delta){
    attackTimer += delta;
    
    if(attackTimer >= AttackInterval && aimed && inRange){
      hostAi.Press(FPSInputHandler.Inputs.PrimaryUse);
      attackTimer = 0f;
    }
  }

}