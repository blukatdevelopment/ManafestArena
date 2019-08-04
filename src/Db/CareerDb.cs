/*
  Responsible for abstracting away file I/O for career data.
*/
using Godot;
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


  public static bool CareerExists(){
    return false;
  }


  public static void ClearCareer(){
    GD.Print("CareerDb.ClearCareer");
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

  public static List<System.Collections.Generic.Dictionary<string, string>> LoadCardsConfig(){
    return ReadHashesListFromFile(CardsFile);
  }

  public static List<System.Collections.Generic.Dictionary<string, string>> LoadRelicsConfig(){
    return ReadHashesListFromFile(RelicsFile);
  }

  public static List<System.Collections.Generic.Dictionary<string, string>> LoadPotionsConfig(){
    return ReadHashesListFromFile(PotionsFile);
  }

  private static List<System.Collections.Generic.Dictionary<string, string>> ReadHashesListFromFile(string filePath){
    if(!System.IO.File.Exists(filePath)){
      return null;
    }

    string json = System.IO.File.ReadAllText(filePath);
    return Util.FromJson<List<System.Collections.Generic.Dictionary<string, string>>>(json); 
  }

  public static void TestConfigs(){
    List<System.Collections.Generic.Dictionary<string, string>> configs;
    GD.Print("Cards config");
    configs = LoadCardsConfig();
    GD.Print(Util.ToJson(configs));
    GD.Print("Relics config");
    configs = LoadRelicsConfig();
    GD.Print(Util.ToJson(configs));
    configs = LoadPotionsConfig();
    GD.Print(Util.ToJson(configs));
  }

}