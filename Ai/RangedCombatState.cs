/*
  An AI behavior for combat using ranged weapons.
*/
using Godot;
using System;
using System.Collections.Generic;

public class RangedCombatState : IBehaviorState {
  public Actor hostActor;
  public StateAi hostAi;
  public Actor enemy;
  public bool aimed;
  public bool inRange;
  public float currentRange;

  // A delay after aiming at the enemy, but before firing to make shots 
  // easier to dodge.
  public bool aimDelayActive;
  public float aimDelayTimer;
  public const float AimDelay = 0.5f;


  public float attackTimer;
  public const float AttackInterval = 1f;
  public const float RangedRange = 10f;
  public const float RangedAimMargin = 0.5f;

  public float strafeTimer;
  public int strafeDirection;

  public RangedCombatState(Actor actor){
    hostActor = actor;
    attackTimer = 0f;
    aimDelayActive = false;
    aimDelayTimer = 0;
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

    aimed = hostAi.IsAimedAt(enemy.Transform.origin, RangedAimMargin);
    currentRange = hostAi.DistanceToActor(enemy);
    inRange = currentRange <= RangedRange;

    Strafe(delta);
    MaintainDistance(delta);
    Attack(delta);
  }

  public void Strafe(float delta){
    strafeTimer -= delta;
    if(strafeTimer > 0){
      hostActor.Move(new Vector3(strafeDirection * hostActor.GetMovementSpeed(), 0, 0), delta);
      return;
    }
    strafeDirection = Util.RandInt(-1, 1);
    strafeTimer = 0.5f; // Quick strafing
  }

  public void MaintainDistance(float delta){
    if(!inRange && aimed){
      // Move up into range
      hostActor.Move(new Vector3(0, 0, -hostActor.GetMovementSpeed()), delta);
    }
    else if(aimed && currentRange < RangedRange){
      // Back up to keep out of melee range
      hostActor.Move(new Vector3(0, 0, hostActor.GetMovementSpeed()), delta);
    }
    hostAi.AimAt(enemy.Transform.origin);
  }

  public void Attack(float delta){
    attackTimer += delta;
    
    ProjectileWeapon pw = hostActor.PrimaryItem() as ProjectileWeapon;

    if(pw == null || pw.GetAmmoCount() == 0){
      hostAi.ChangeState(StateAi.States.Fighting);
    }


    if(!aimDelayActive && attackTimer >= AttackInterval && aimed){
      aimDelayTimer = 0f;
      aimDelayActive = true;
    }

    if(aimDelayActive){
      aimDelayTimer += delta;
      if(aimDelayTimer >= AimDelay){
        aimDelayActive = false;
        attackTimer = 0f;
        aimDelayTimer = 0f;
        hostActor.Use(Item.Uses.A);
      }

    }
  }

}