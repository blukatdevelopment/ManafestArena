/*
  Restores health or fatigue
*/
using Godot;

public class RestorationSpell : Item {
  public int health, stamina, mana;
  public float coolDown, coolDownTimer;
  public bool busy;

  public override void Use(Item.Uses use, bool released = false){
    if(busy){
      return;
    }

    switch(use){
      case Uses.A: Restore(); break;
    }
  }

  public override void _Process(float delta){
    if(busy){
      coolDownTimer += delta;
      if(coolDownTimer >= coolDown){
        busy = false;
        coolDownTimer = 0f;
      }
    }
  }

  public void Restore(){
    if(wielder == null){
      GD.Print("Can't heal with no wielder");
      return;
    }

    StatsManager stats = GetStats();

    if(stats == null){
      return;
    }

    if(manaCost != 0 && !stats.ConsumeStat(StatsManager.Stats.Mana, manaCost)){
      GD.Print("Not enough mana, friend.");
      return;
    }

    stats.ReceiveDamage(GetDamage());
    busy = true;
  }

  public Damage GetDamage(){
    Damage dmg = new Damage();
    dmg.health = -health;
    dmg.stamina = -stamina;
    dmg.mana = -mana;
    return dmg;
  }

}