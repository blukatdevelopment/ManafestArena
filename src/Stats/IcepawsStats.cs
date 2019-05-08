using Godot;
using System;
using System.Collections.Generic;
/*
  Base class for stats handlers that use ICEPAWS stats.
*/
public class IcepawsStats : IStats {
  System.Collections.Generic.Dictionary<string, int> stats;
  System.Collections.Generic.Dictionary<string, string> facts;
  private float updateTimer;
  private const float UpdateTime = 1f;

  public IcepawsStats(){
    stats = new System.Collections.Generic.Dictionary<string, int>();
    foreach(string stat in GetStatList()){
      stats.Add(stat, 0);
    }

    facts = new System.Collections.Generic.Dictionary<string, string>();

  }

  public bool HasStat(string stat){
    return stats.ContainsKey(stat.ToLower());
  }
  
  public virtual List<string> GetStatList(){
    List<string> statList = new List<string>();
    statList.AddRange(IcepawsStatList());
    statList.AddRange(SkillsStatList());
    statList.AddRange(MiscStatList());
    return statList;
  }

  protected List<string> IcepawsStatList(){
    return new List<string>{
      "health", "healthmax", "healthregen",
      "stamina", "staminamax", "staminaregen",
      "mana", "manamax", "manaregen",
      "intelligence", "intelligencebonus",
      "charisma", "charismabonus",
      "endurance", "endurancebonus",
      "perception", "perceptionbonus",
      "agility", "agilitybonus",
      "willpower", "willpowerbonus",
      "strength", "strength",
      "jumpcost", 
      "sprintcost"
    };
  }

  // TODO: Update skills when they are actually implemented, maybe.
  protected List<string> SkillsStatList(){
    return new List<string>{
      "alchemy", "conjuration", "mysticism",      // intelligence
      "mercantile", "speechcraft", "illusion",    // charisma
      "armorer", "block", "heavy armor",          // endurance
      "security", "sneak", "marksman",            // perception
      "athletics", "acrobatics", "light armor",   // agility
      "alteration", "destruction", "restoration", // willpower
      "blade", "blunt", "unarmed"                 // strength
    };
  }

  protected List<string> MiscStatList(){
    return new List<string>{
      "id"
    };
  }

  public virtual int GetStat(string stat){
    stat = stat.ToString();
    if(IcepawsStatList().IndexOf(stat) != -1){
      return GetIcepawsStat(stat);
    }
    return 0;
  }

  protected virtual int GetIcepawsStat(string stat){
    stat = stat.ToLower();
    switch(stat){
      case "healthmax":
        return 50 + (GetStat("endurance") * 10);
        break;
      case "staminamax":
        return (GetStat("endurance") * 10) + (GetStat("agility") * 10); 
        break;
      case "manamax":
        return (GetStat("intelligence") + (GetStat("willpower") * 10));
        break;
      case "jumpcost":
        int agility = GetStat("agility");
        if(agility == 0){
          return 100;
        }
        return 100 / agility;
        break;
      case "sprintcost":
        break;
    }
    if(stats.ContainsKey(stat)){
      return stats[stat];
    }
    return 0;
  }
  
  public void SetStat(string stat, int val){
    stat = stat.ToLower();
    if(stats.ContainsKey(stat)){
      stats[stat] = val;
      return;
    }
    if(GetStatList().IndexOf(stat) != -1){
      stats.Add(stat, val);
    }
  }

  public string GetFact(string fact){
    fact = fact.ToLower();
    if(facts.ContainsKey(fact)){
      return facts[fact];
    }
    return "";
  }

  public void SetFact(string fact, string val){
    fact = fact.ToLower();
    if(facts.ContainsKey(fact)){
      facts[fact] = val;
    }
  }

  public bool StatCheck(string stat, int difficulty){
    stat = stat.ToLower();
    // TODO: Do a dice roll?
    return true;
  }

  public bool ConsumeStat(string stat, int amount){
    stat = stat.ToLower();
    int statVal = GetStat(stat);
    int newVal = statVal - amount;
    if(newVal < 0){
      return false;
    }

    stats[stat] = newVal;

    return true;
  }
  
  public void ReceiveDamage(Damage damage){
    stats["health"] = GetStat("health") - damage.health;
    stats["stamina"] = GetStat("stamina") - damage.stamina;
    stats["mana"] = GetStat("mana") - damage.mana;

    EnforceMaxValues();
  }
  
  public void Update(float delta){
    updateTimer -= delta;
    if(updateTimer <= 0){
      updateTimer = UpdateTime;
      UpdateStats();
    }
  }

  protected void UpdateStats(){
    stats["health"] = stats["health"] + stats["healthregen"];
    stats["stamina"] = stats["stamina"] + stats["staminaregen"];
    stats["mana"] = stats["mana"] + stats["manaregen"];

    EnforceMaxValues();
  }

  protected virtual void EnforceMaxValues(){
    if(stats["health"] > stats["healthmax"]){
      stats["health"] = stats["healthmax"];
    }
    if(stats["stamina"] > stats["staminamax"]){
      stats["stamina"] = stats["staminamax"];
    }
    if(stats["mana"] > stats["manamax"]){
      stats["mana"] = stats["manamax"];
    }
  }
}