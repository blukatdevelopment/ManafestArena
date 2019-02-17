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
    
    if(hostAi.enemies.Count == 0){
      hostAi.ChangeState(StateAi.States.Roaming);
      return;
    }

    // TODO: Make this choose the closest enemy
    enemy = hostAi.enemies[0];
  }

  public void Update(float delta){
    DetermineAttack();
  }

  public void DetermineAttack(){
    List<Item> items = hostAi.GetItems();

    foreach(Item item in items){
      ProjectileWeapon pw = item as ProjectileWeapon;
      if(pw != null && pw.ViewAmmoCount() > 0){
        hostAi.ChangeState(StateAi.States.Ranged);
        return;
      }
    }

    hostAi.ChangeState(StateAi.States.Melee);
  }


}