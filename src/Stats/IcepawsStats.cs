using Godot;
using System;
using System.Collections.Generic;
/*
  Base class for stats handlers that use ICEPAWS stats.
*/
public class IcepawsStats : IStats, IReceiveDamage {
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
      "health", "healthmax", "healthregen", "healthregenstored",
      "stamina", "staminamax", "staminaregen", "staminaregenstored",
      "mana", "manamax", "manaregen", "manaregenstored",
      "block",
      "intelligence", "intelligencebonus",
      "charisma", "charismabonus",
      "endurance", "endurancebonus",
      "perception", "perceptionbonus",
      "agility", "agilitybonus",
      "willpower", "willpowerbonus",
      "strength", "strengthmax",
      "jumpcost", 
      "sprintcost", "sprintbonus",
      "speed"
    };
  }

  // TODO: Update skills when they are actually implemented, maybe.
  protected List<string> SkillsStatList(){
    return new List<string>{
      "alchemy", "conjuration", "mysticism",      // intelligence
      "mercantile", "speechcraft", "illusion",    // charisma
      "armorer", "heavy armor",          // endurance
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
    int agility, strength, endurance;
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
        break;
      case "sprintcost":
        break;
      case "sprintbonus": // out of 100
        strength = GetStat("strength");
        agility = GetStat("agility");
        return (agility * 5) + (strength * 5); 
        break;
      case "speed": // Out of 100
        agility = GetStat("agility");
        return agility * 10;
        break;
      case "healthregen": // In .1/second increments
        return GetStat("endurance") / 5;
        break;
      case "staminaregen":
        return (GetStat("endurance") * 5) + (GetStat("willpower") * 2);
        break;
      case "manaregen":
        return (GetStat("willpower") * 10) + (GetStat("endurance") * 2);
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

    if(stat == "stamina"){
      return ConsumeStamina(amount);
    }

    int statVal = GetStat(stat);
    int newVal = statVal - amount;
    if(newVal < 0){
      return false;
    }

    stats[stat] = newVal;

    return true;
  }

  // Convert to consume 0.1 increments
  public bool ConsumeStamina(int amount){
    int fullPointsConsumed = amount/10;
    int partialPointsConsumed = amount%10;
    bool breakFullPoint = false;

    if(partialPointsConsumed > stats["staminaregenstored"]){
      breakFullPoint = true;
      fullPointsConsumed += 1;
    }

    if(fullPointsConsumed == 0){
      stats["staminaregenstored"] -= partialPointsConsumed;
      return true;
    }
    else if(fullPointsConsumed < stats["stamina"]){
      stats["stamina"] -= fullPointsConsumed;

      if(breakFullPoint){
        stats["staminaregenstored"] = 10 - partialPointsConsumed;
      }
      return true;
    }
    else{
      return false;
    }
  }
  
  public void ReceiveDamage(Damage damage){
    damage.health -= GetStat("block");
    if(damage.health < 0){
      SetStat("block", -damage.health);
      damage.health = 0;
    }

    stats["health"] = GetStat("health") - damage.health;
    stats["stamina"] = GetStat("stamina") - damage.stamina;
    stats["mana"] = GetStat("mana") - damage.mana;

    EnforceMaxValues();
  }

  public int GetHealth(){
    return GetStat("health");
  }
  
  public void Update(float delta){
    updateTimer -= delta;
    if(updateTimer <= 0){
      updateTimer = UpdateTime;
      UpdateStats();
    }
  }

  // Converts regen into 0.1 increments
  protected void UpdateStats(){

    int healthStored = stats["healthregenstored"] + GetStat("healthregen");
    int healthGained = healthStored/10;
    stats["healthregenstored"] = healthStored % 10;
    stats["health"] += healthGained;

    int staminaStored = stats["staminaregenstored"] + GetStat("staminaregen");
    int staminaGained = staminaStored/10;
    stats["staminaregenstored"] = staminaStored % 10;
    stats["stamina"] += staminaGained;

    int manaStored = stats["manaregenstored"] + GetStat("manaregen");
    int manaGained = manaStored/10;
    stats["manaregenstored"] = manaStored % 10;
    stats["mana"] += manaGained;


    EnforceMaxValues();
  }

  protected virtual void EnforceMaxValues(){
    if(stats["health"] > GetStat("healthmax")){
      stats["health"] = GetStat("healthmax");
    }
    if(stats["stamina"] > GetStat("staminamax")){
      stats["stamina"] = GetStat("staminamax");
    }
    if(stats["mana"] > GetStat("manamax")){
      stats["mana"] = GetStat("manamax");
    }
  }

  public void RestoreCondition(){
    SetStat("health", GetStat("healthmax"));
    SetStat("stamina", GetStat("staminamax"));
    SetStat("mana", GetStat("manamax"));
  }
}