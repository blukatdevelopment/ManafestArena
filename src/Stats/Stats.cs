/*
  Manages state and some formulas for character stats.
*/
using System;
using System.Collections.Generic;

public class Stats : IReceiveDamage {
  
  public enum Attributes {
    Intelligence,
    Charisma,
    Endurance,
    Perception,
    Agility,
    Willpower,
    Strength
  }

  private IncrementTimer updateTimer;
  private const float UpdateDelay = 1f;
  private Dictionary<string, int> stats;

  public int Id { get; set; }

  public int Intelligence {
    get => stats["Intelligence"];
  }
  public int Charisma {
    get => stats["Charisma"];
  }
  public int Endurance {
    get => stats["Endurance"];
  }
  public int Perception {
    get => stats["Perception"];
  }
  public int Agility {
    get => stats["Agility"];
  }
  public int Willpower {
    get => stats["Willpower"];
  }
  public int Strength {
    get => stats["Strength"];
  }

  public enum Conditions {
    Health,
    Stamina,
    Mana
  }

  public int Health {
    get => stats["Health"];
  }
  public int Stamina {
    get => stats["Stamina"];
  }
  public int Mana {
    get => stats["Mana"];
  }

  public int HealthMax {
    get => 50 + (Endurance * 10);
  }
  public int StaminaMax {
    get => 10 * (Endurance + Agility);
  }
  public int ManaMax {
    get => (Intelligence + Willpower) * 10;
  }
  public int HealthRegen {
    get => Endurance / 5;
  }
  public int StaminaRegen {
    get => (Endurance / 2) + (Willpower / 2) + 1;
  }
  public int ManaRegen {
    get => (Willpower) + (Endurance / 2);
  }

  public int JumpCost {
    get => 20;
  }
  public int SprintCost {
    get => 5;
  }
  public int SprintBonus {
    get => (Agility * 10) + 100;
  }
  public int Speed {
    get => Agility * 10;
  }

  public Stats(Dictionary<string, int> stats){
    this.stats = stats;
    ValidateStats(this.stats);
    updateTimer = new IncrementTimer(UpdateDelay);
    RestoreCondition();
  }

  private void ValidateStats(Dictionary<string, int> stats){

    List<string> missing = new List<string>();

    foreach(string statName in GetAllStatNames()){
      if(!stats.ContainsKey(statName)){
        missing.Add(statName);
      }
    }

    if(missing.Count != 0){
      throw new System.ArgumentException("Missing arguments: " + Util.ToJson(missing));
    }

  }

  private List<string> GetAllStatNames(){
    List<string> ret = new List<string>();

    foreach(var attribute in Util.GetEnumValues<Attributes>()){
      ret.Add("" + attribute);
    }
    foreach(var condition in Util.GetEnumValues<Conditions>()){
      ret.Add("" + condition);
    }

    return ret;
  }

  public void Update(float delta){
    if(!updateTimer.CheckTimer(delta)){
      return;
    }

    stats["Health"] += HealthRegen;
    stats["Stamina"] += StaminaRegen;
    stats["Mana"] += ManaRegen;

    EnforceMaxValues();
  }

  public void ReceiveDamage(Damage damage){
    stats["Health"]   -= damage.health;
    stats["Stamina"]  -= damage.stamina;
    stats["Mana"]     -= damage.mana;

    EnforceMaxValues();
  }


  private void EnforceMaxValues(){
    stats["Health"]   = Health > HealthMax ? HealthMax : Health;
    stats["Stamina"]  = Stamina > StaminaMax ? StaminaMax : Stamina;
    stats["Mana"]     = Mana > ManaMax ? ManaMax : Mana;
  }

  public void RestoreCondition(){
    stats["Health"]   = HealthMax;
    stats["Stamina"]  = StaminaMax;
    stats["Mana"]     = ManaMax;
  }

  public int GetHealth(){
    return Health;
  }

  public bool ConsumeCondition(Conditions condition, int amount){
    if(condition == Conditions.Health && Health - amount >= 0){
      stats["Health"] -= amount;
      return true;
    }

    if(condition == Conditions.Stamina && Stamina - amount >= 0){
      stats["Stamina"] -= amount;
      return true;
    }
    
    if(condition == Conditions.Mana && Mana - amount >= 0){
      stats["Mana"] -= amount;
      return true;
    }
    
    return false;
  }

  public bool StatCheck(Attributes attribute, int difficulty){
    string attributeString = "" + attribute;
    int score = stats[attributeString];
    int bonus = (score - 10) / 2;
    int d20 = 20;
    int roll = Util.RandInt(1, 20, true);
    roll += bonus;
    return roll >= difficulty;
  }

  public static Dictionary<string, int> EmptyStats(int val = 0){
    return new Dictionary<string, int>{
      {"Intelligence", val},
      {"Charisma", val},
      {"Endurance", val},
      {"Perception", val},
      {"Agility", val},
      {"Willpower", val},
      {"Strength", val},
      {"Health", val},
      {"Stamina", val},
      {"Mana", val}
    };
  }
}