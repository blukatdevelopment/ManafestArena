/*
  Dispense loot based on configured random chance.

*/
using System;
using System.Collections.Generic;

public class LootTable {
  public const string DefaultLootConfigFile = "Configs/loot.json";
  Dictionary<string, List<LootDrop>> actions;
  Dictionary<string, int> availableLoot;

  public LootTable(string filePath = DefaultLootConfigFile){
    string json = System.IO.File.ReadAllText(filePath);
    actions = Util.FromJson<Dictionary<string, List<LootDrop>>>(json);
    
  }

  public LootTable(Dictionary<string, List<LootDrop>> actions){
    this.actions = actions;
  }

  // Return loot earned from that action
  public Dictionary<string, int> HandleAction(string action){
    if(!actions.ContainsKey(action) || actions[action].Count == 0){
      return new Dictionary<string, int>();
    }
    if(availableLoot == null){
      availableLoot = new Dictionary<string, int>();
    }

    Dictionary<string, int> earned = new Dictionary<string, int>();
    List<LootDrop> drops = actions[action];

    foreach(LootDrop drop in drops){
      if(Util.RandInt(1, 100, true) <= drop.chance){
        int quantity = Util.RandInt(drop.minLoot, drop.maxLoot, true);
        if(earned.ContainsKey(drop.lootName)){
          earned[drop.lootName] += quantity;
        }
        else{
          earned.Add(drop.lootName, quantity);
        }
        if(availableLoot.ContainsKey(drop.lootName)){
          availableLoot[drop.lootName] += quantity;
        }
        else{
          availableLoot.Add(drop.lootName, quantity);
        }

      }
    }
    return earned;
  }

  public Dictionary<string, int> AvailableLoot(){
    if(availableLoot == null){
      return new Dictionary<string, int>();
    }
    return availableLoot;
  }

  public void ClearLoot(){
    availableLoot = new Dictionary<string, int>();
  }

  public string ToString(){
    string ret = "LootTable:\n";
    foreach(string action in actions.Keys){
      ret += "\t" + action + "\n";
      List<LootDrop> drops = actions[action];
      foreach(LootDrop drop in drops){
        ret += "\t lootName: " + drop.lootName;
        ret += ", chance: " + drop.chance;
        ret += ", minLoot " + drop.minLoot;
        ret += ", maxLoot " + drop.maxLoot + "\n";
      }
    }
    return ret;
  }
}
