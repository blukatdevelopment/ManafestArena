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

  public IItem activeItem;
  public IWeapon activeWeapon;
  public IRangedWeapon activeRangedWeapon;

  public RangedCombatState(Actor actor){
    hostActor = actor;
    attackTimer = 0f;
    aimDelayActive = false;
    aimDelayTimer = 0;
  }

  public void Init(StateAi hostAi){
    this.hostAi = hostAi;
    activeItem = hostActor.hotbar.GetActiveSlot();
    activeWeapon = activeItem as IWeapon;
    activeRangedWeapon = activeItem as IRangedWeapon;

    bool weaponNull = activeItem == null || activeWeapon == null || activeRangedWeapon == null;   
    if(hostAi.enemies.Count == 0 || hostAi.enemies[0] == null || weaponNull){
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

    aimed = hostAi.IsAimedAt(enemySpat.Transform.origin, RangedAimMargin);
    currentRange = hostAi.DistanceToActor(enemy);
    inRange = currentRange <= RangedRange;

    if(!aimDelayActive){
      Strafe(delta);
      MaintainDistance(delta, enemySpat);
    }
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
    strafeDirection = Util.RandInt(-1, 1);
    strafeTimer = (float)Util.RandInt(0, 6) * 0.5f;
  }

  public void MaintainDistance(float delta, Spatial enemySpat){
    if(!inRange && aimed){
      // Move up into range
      hostAi.Hold(FPSInputHandler.Inputs.MoveForward);
    }
    else if(aimed && currentRange < RangedRange){
      // Back up to keep out of melee range
      hostAi.Hold(FPSInputHandler.Inputs.MoveBackward);
    }
    hostAi.AimAt(enemySpat.Transform.origin);
  }

  public void Attack(float delta){
    attackTimer += delta;

    if(activeRangedWeapon.GetAmmo() == 0){
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
        hostAi.Hold(FPSInputHandler.Inputs.PrimaryUse);
      }

    }
  }

  // public void Attack(float delta){
  //   attackTimer += delta;
    
  //   ProjectileWeapon pw = hostActor.PrimaryItem() as ProjectileWeapon;

  //   if(pw == null || pw.GetAmmoCount() == 0){
  //     hostAi.ChangeState(StateAi.States.Fighting);
  //   }


  //   if(!aimDelayActive && attackTimer >= AttackInterval && aimed){
  //     aimDelayTimer = 0f;
  //     aimDelayActive = true;
  //   }

  //   if(aimDelayActive){
  //     aimDelayTimer += delta;
  //     if(aimDelayTimer >= AimDelay){
  //       aimDelayActive = false;
  //       attackTimer = 0f;
  //       aimDelayTimer = 0f;
  //       hostActor.Use(Item.Uses.A);
  //     }

  //   }
  // }

}