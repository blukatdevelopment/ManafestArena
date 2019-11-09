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
      return;
    }
    IItem bestRanged = SelectRangedWeapon(hostActor.hotbar.Getitems());
    if(bestRanged != null){
      hostAi.SelectHotBarItem(bestRanged);
      hostAi.ChangeState(StateAi.States.Ranged);
      return;
    }

    IItem best = SelectMeleeWeapon(hostActor.hotbar.Getitems());
    if(best == null){
      GD.Print("No weapons to use!");
      return;
    }
    hostAi.SelectHotBarItem(best);
    hostAi.ChangeState(StateAi.States.Melee);
  }

  public IItem SelectRangedWeapon(List<IItem> items){
    IItem ret = null;
    IRangedWeapon retWeapon = null;
    foreach(IItem item in items){
      IRangedWeapon weapon = item as IRangedWeapon;
      if(weapon == null){
        continue;
      }
      if(ret == null && weapon.GetAmmo() != 0){
        ret = item;
        retWeapon = weapon;
        continue;
      }

      if(CompareRangedWeapons(retWeapon, weapon) == Util.Greater){
        ret = item;
        retWeapon = weapon;
      }
    }
    
    return ret;
  }

  public int CompareRangedWeapons(IRangedWeapon first, IRangedWeapon second){
    if(first == null && second != null){
      if(second.GetAmmo() == 0){
        return Util.Equal;
      }
      return Util.Greater;
    }
    else if(first != null && second == null){
      if(first.GetAmmo() == 0){
        return Util.Equal;
      }
      return Util.Lesser;
    }
    else if(first == null && second == null){
      return Util.Equal;
    }

    int firstAmmo = first.GetAmmo();
    int secondAmmo = second.GetAmmo();
    if(firstAmmo == 0 && secondAmmo != 0){
      return Util.Greater;
    }
    else if(firstAmmo != 0 && secondAmmo == 0){
      return Util.Lesser;
    }
    else if(firstAmmo == 0 && secondAmmo == 0){
      return Util.Equal;
    }

    if(firstAmmo > secondAmmo){
      return Util.Lesser;
    }
    else if(firstAmmo < secondAmmo){
      return Util.Greater;
    }
    return Util.Equal;
  }

  public IItem SelectMeleeWeapon(List<IItem> items){
    IItem ret = null;
    IWeapon retWeapon = null;
    foreach(IItem item in items){
      IWeapon weapon = item as IWeapon;
      IRangedWeapon ranged = weapon as IRangedWeapon;
      if(weapon == null || ranged != null){
        continue;
      }
      if(ret == null){
        ret = item;
        retWeapon = weapon;
        continue;
      }

      if(CompareMeleeWeapons(retWeapon, weapon) == Util.Greater){
        ret = item;
        retWeapon = weapon;
      }
    }
    
    return ret;
  }

  public int CompareMeleeWeapons(IWeapon first, IWeapon second){
    IRangedWeapon firstRanged = first as IRangedWeapon;
    IRangedWeapon secondRanged = second as IRangedWeapon;

    if(firstRanged != null && second != null){
      return Util.Greater;
    }
    if(first != null && secondRanged != null){
      return Util.Lesser;
    }
    if(firstRanged != null && secondRanged != null){
      return Util.Equal;
    }

    if(first == null && second != null){
      return Util.Greater;
    }
    else if(first != null && second == null){
      return Util.Lesser;
    }
    else if(first == null && second == null){
      return Util.Equal;
    }

    Damage firstDamage = first.GetDamage();
    Damage secondDamage = second.GetDamage();

    if(firstDamage == null && secondDamage != null){
      return Util.Greater;
    }
    else if(firstDamage != null && secondDamage == null){
      return Util.Lesser;
    }
    else if(firstDamage == null && secondDamage == null){
      return Util.Equal;
    }

    float firstDps = Util.DPS((float)firstDamage.health, first.AttackDelay());
    float secondDps = Util.DPS((float)secondDamage.health, second.AttackDelay());
    if(firstDps == secondDps){
      return Util.Equal;
    }
    else if(firstDps < secondDps){
      return Util.Greater;
    }
    return Util.Lesser;
  }



}