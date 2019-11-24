/*
  Responsible for abstracting away file I/O for career data.
*/
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CareerDb {
  public const string DefaultCareerSave = "Saves/Career.json";
  public const string StatsFile = "Saves/stats.csv";
  public const string CareerFile = "Saves/career_tree.csv";
  public const string CardsFile = "Configs/cards.json";
  public const string RelicsFile = "Configs/relics.json";
  public const string PotionsFile = "Configs/potions.json";
  public static Dictionary<string,List<Dictionary<string, string>>> configData;
  

  public static void Init(){
    LoadConfigData();
  }

  public static bool CareerExists(){
    return false;
  }

  public static void ClearCareer(){
    System.IO.File.Delete(StatsFile);
    System.IO.File.Delete(CareerFile);
  }

  public static bool SaveExists(){
    if(System.IO.File.Exists(StatsFile) && System.IO.File.Exists(CareerFile)){
      return true;
    }
    return false;
  }

  public static List<string> PressEventFiles(){
    string[] filePaths = System.IO.Directory.GetFiles("PressEvents/", "*.csv", SearchOption.TopDirectoryOnly);
    return new List<string>(filePaths);
  }

  public static void SaveCareerData(CareerData data) {
    if(data.fileName == ""){
      data.fileName = DefaultCareerSave;
    }

    string json = Util.ToJson(data);

    System.IO.File.WriteAllText(data.fileName, json);
  }

  public static CareerData LoadCareerData(string filePath = DefaultCareerSave){
    if(!System.IO.File.Exists(filePath)){
      return null;
    }

    string json = System.IO.File.ReadAllText(filePath);
    return Util.FromJson<CareerData>(json);
  }

  private static List<Dictionary<string, string>> ReadHashesListFromFile(string filePath){
    if(!System.IO.File.Exists(filePath)){
      return null;
    }

    string json = System.IO.File.ReadAllText(filePath);
    return Util.FromJson<List<Dictionary<string, string>>>(json); 
  }

  public static void LoadConfigData(){
    configData = new Dictionary<string,List<Dictionary<string, string>>>();
    configData.Add("cards", ReadHashesListFromFile(CardsFile));
    configData.Add("relics", ReadHashesListFromFile(RelicsFile));
    configData.Add("potions", ReadHashesListFromFile(PotionsFile));
  }

  public static Dictionary<string, string> GetCard(string name){
    return GetHashByKey(configData["cards"], "name", name);
  }

  public static Dictionary<string, string> GetPotion(string name){
    return GetHashByKey(configData["potions"], "name", name);
  }

  public static Dictionary<string, string> GetRelic(string name){
    return GetHashByKey(configData["relics"], "name", name);
  }

  private static Dictionary<string, string> GetHashByKey(List<Dictionary<string, string>> hashes, string keyName, string keyValue){
    foreach(Dictionary<string, string> hash in hashes){
      if(hash.ContainsKey(keyName) && hash[keyName] == keyValue){
        return new Dictionary<string, string>(hash);
      }
    }
    return null;
  }
}