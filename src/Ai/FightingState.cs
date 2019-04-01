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
    List<Item> items = hostAi.GetItems();


    int rangedSlot = SelectWeaponSlot(true);

    if(rangedSlot != -1){
      hostActor.EquipHotbarItem(rangedSlot);
      hostAi.ChangeState(StateAi.States.Ranged);      
      return;
    }

    int meleeSlot = SelectWeaponSlot(false);

    if(meleeSlot != -1){
      hostActor.EquipHotbarItem(meleeSlot);
    }

    hostAi.ChangeState(StateAi.States.Melee);
  }

  public int SelectWeaponSlot(bool ranged){
    int topCandidate = -1;
    int maxDamage = -1;

    Item[] items = hostActor.hotbar.items;
    for(int i = 0; i < items.Length; i++){
      ProjectileWeapon pw = items[i] as ProjectileWeapon;
      if(pw != null && ranged){
        int dmg = pw.GetBaseDamage().health;
        int ammo = pw.GetAmmoCount();
        if(maxDamage < dmg && ammo > 0){
          maxDamage = dmg;
          topCandidate = i;
        }
      }

      MeleeWeapon mw = items[i] as MeleeWeapon;
      if(mw != null && !ranged){
        int dmg = mw.GetBaseDamage().health;
        if(maxDamage < dmg){
          maxDamage = dmg;
          topCandidate = i;
        }
      }

    }

    return topCandidate;
  }

}