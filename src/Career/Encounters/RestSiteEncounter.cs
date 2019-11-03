using Godot;
using System;
using System.Collections.Generic;

public class RestSiteEncounter : IEncounter {

  public RestSiteEncounter(){}

  public string GetDisplayName(){
    return "Rest Site";
  }


  public void StartEncounter(){
    //Session.ChangeMenu("RestSiteMenu");
    // TODO: Populate menu with rest site upgrades.
    Career career = Career.GetActiveCareer();
    if(career != null){
      career.CompleteEncounter();
      return;
    }
  }
  
  public IEncounter GetRandomEncounter(){
    return new RestSiteEncounter();
  }

  private List<string> RestSiteUpgrades(){
    string archetype = "";//GetPlayerStats().GetFact(StatsManager.Facts.Archetype);
    GD.Print("Archetype " + archetype);
    List<string> ret = new List<string>();
    // switch(archetype){
    //   case "beast": ret = GetBeastUpgrades(); break;
    //   case "mage": ret = GetMageUpgrades(); break;
    //   case "soldier": ret = GetSoldierUpgrades(); break;
    // }

    // Fill in the remaining three.
    int needed = 3 - ret.Count;
    List<int> used = new List<int>();
    List<string> generic = GetGenericUpgrades();

    for(int i = 0; i < needed; i++){
      int choice = 0;
      while(used.IndexOf(choice) != -1){
        choice = Util.RandInt(0, generic.Count);
      }
      used.Add(choice);
      ret.Add(generic[choice]);
    }
    return ret;
  }

    // public static void SelectRestSiteUpgrade(string selection){
  //   GD.Print("Selected " + selection);
  //   StatsManager stats = GetPlayerStats();
    
  //   int intelligenceBuff = stats.GetStatBuff(StatsManager.Stats.Intelligence);
  //   int charismaBuff = stats.GetStatBuff(StatsManager.Stats.Charisma);
  //   int enduranceBuff = stats.GetStatBuff(StatsManager.Stats.Endurance);
  //   int perceptionBuff = stats.GetStatBuff(StatsManager.Stats.Perception);
  //   int agilityBuff = stats.GetStatBuff(StatsManager.Stats.Agility);
  //   int willpowerBuff = stats.GetStatBuff(StatsManager.Stats.Willpower);
  //   int strengthBuff = stats.GetStatBuff(StatsManager.Stats.Strength);
  //   int currentHealth = stats.GetStat(StatsManager.Stats.Health);
  //   int healthBuff = stats.GetStatBuff(StatsManager.Stats.HealthMax);


  //   switch(selection){
  //     case "Extra endurance":
  //       stats.SetStatBuff(StatsManager.Stats.Endurance, enduranceBuff + 2);
  //     break;
  //     case "Extra agility":
  //       stats.SetStatBuff(StatsManager.Stats.Agility, agilityBuff + 2);
  //     break;
  //     case "+50 max health":
  //       stats.SetStatBuff(StatsManager.Stats.HealthMax, healthBuff + 50);
  //       stats.SetBaseStat(StatsManager.Stats.Health, currentHealth + 50);
  //     break;
  //     case "Second spear":
  //       GD.Print("Awarding spear");
  //       stats.SetFact(StatsManager.Facts.Slot2, "spear");
  //       stats.SetFact(StatsManager.Facts.Slot3, "claws");
  //     break;
  //     case "Double crossbow":
  //       stats.SetFact(StatsManager.Facts.Slot1, "doublecrossbow");
  //     break;
  //     case "Rapid crossbow":
  //       stats.SetFact(StatsManager.Facts.Slot1, "rapidcrossbow");
  //     break;
  //     case "Fireball II":
  //       stats.SetFact(StatsManager.Facts.SpellSlot1, "FireballIISpell");
  //     break;
  //     case "Fireball III":
  //       stats.SetFact(StatsManager.Facts.SpellSlot1, "FireballIIISpell");
  //     break;
  //   }
  // }


  public static void HealPlayer(int health){
    Damage dmg = new Damage();
    dmg.health = -health;

    //Session.session.career.playerData.stats.ReceiveDamage(dmg);

    // int playerHealth = Session.session.career.playerData.stats.GetStat(StatsManager.Stats.Health);

    // if(playerHealth <= 0){
    //   GD.Print("Career.HealPlayer just killed the player.");
    //   Session.session.career.FailEncounter();
    // }
    // else{
    //   GD.Print("Player was healed " + health + " health and now has " + playerHealth + " health.");
    // }
  }

  public static string UpgradeDescription(string upgradeName){
    Dictionary<string, string> descriptionMap;
    descriptionMap = new Dictionary<string, string>{
      {"Extra endurance","Improved health, take less damage."},
      {"+50 max health", "Take more damage"},
      {"Extra agility", "Run faster"},
      {"Second spear", "Two spears are better than one."},
      {"Double crossbow", "Fire twice before reloading"},
      {"Rapid crossbow", "Reload rapidly."},
      {"Fireball II", "Stronger projectile attack."},
      {"Fireball III", "Strongest projectile attack."}
    };

    return descriptionMap[upgradeName];
  }

  public static List<string> GetGenericUpgrades(){
    return new List<string>{
      "Extra endurance",
      "Extra agility",
      "+50 max health"
    };
  }

}