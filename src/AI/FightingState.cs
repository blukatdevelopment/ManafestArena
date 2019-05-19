/*
  An AI behavior that decides how to attack an enemy.
*/
using Godot;
using System;
using System.Collections.Generic;

public class FightingState : IBehaviorState {
  public Actor hostActor;
  public StateAi hostAi;
  public Actor enemy;

  public FightingState(Actor actor){
    hostActor = actor;
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
    DetermineAttack();
  }

  public void DetermineAttack(){
    if(hostActor.hotbar == null){
      GD.Print("Host has no hotbar");
      return;
    }
    IItem best = SelectWeapon(hostActor.hotbar.Getitems());
    if(best == null){
      GD.Print("No weapons to use!");
      return;
    }
    int slot = hostActor.hotbar.GetSlotByItem(best);
    int activeSlot = hostActor.hotbar.GetEquippedSlot();
    for(int i = 0; i < hostActor.hotbar.SlotDistance(activeSlot, slot); i++){
      hostAi.Press(FPSInputHandler.Inputs.NextItem);
    }
    hostAi.ChangeState(StateAi.States.Melee);
  }

  public IItem SelectWeapon(List<IItem> items){
    IItem ret = null;
    IWeapon retWeapon = null;
    foreach(IItem item in items){
      IWeapon weapon = item as IWeapon;
      if(weapon == null){
        continue;
      }
      if(ret == null){
        ret = item;
        retWeapon = weapon;
        continue;
      }

      if(CompareWeapons(retWeapon, weapon) == 1){
        ret = item;
        retWeapon = weapon;
      }
    }
    
    return ret;
  }

  /*
    -1 first
    0  equal
    1  second
  */
  public int CompareWeapons(IWeapon first, IWeapon second){
    Damage firstDamage = first.GetDamage();
    Damage secondDamage = second.GetDamage();

    if(firstDamage == null && secondDamage != null){
      return 1;
    }
    else if(firstDamage != null && secondDamage == null){
      return -1;
    }
    else if(firstDamage == null && secondDamage == null){
      return 0;
    }

    float firstDps = Util.DPS((float)firstDamage.health, first.AttackDelay());
    float secondDps = Util.DPS((float)secondDamage.health, second.AttackDelay());
    if(firstDps == secondDps){
      return 0;
    }
    else if(firstDps < secondDps){
      return 1;
    }
    return -1;
  }

}