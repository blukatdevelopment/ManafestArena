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
    
    aimed = hostAi.IsAimedAt(enemy.Transform.origin, MeleeAimMargin);
    inRange = hostAi.DistanceToActor(enemy) <= MeleeRange;
    Strafe(delta);
    CloseDistance(delta);
    Attack(delta);
  }

  public void Strafe(float delta){
    strafeTimer -= delta;
    if(strafeTimer > 0){
      hostActor.Move(new Vector3(strafeDirection * hostActor.GetMovementSpeed(), 0, 0), delta);
      return;
    }
    strafeDirection = Util.RandInt(-1, 1);
    strafeTimer = (float)Util.RandInt(0, 6) * 0.5f;
  }

  public void CloseDistance(float delta){
    if(!inRange && aimed){
      hostActor.Move(new Vector3(0, 0, -hostActor.GetMovementSpeed()), delta);
    }
    hostAi.AimAt(enemy.Transform.origin);
  }

  public void Attack(float delta){
    attackTimer += delta;
    
    if(attackTimer >= AttackInterval && aimed && inRange){
      hostActor.Use(Item.ItemInputs.APress);
      attackTimer = 0f;
    }
  }

}