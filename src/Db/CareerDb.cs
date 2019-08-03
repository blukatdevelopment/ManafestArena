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
    if(!System.IO.File.Exists(CardsFile)){
      return null;
    }

    string json = System.IO.File.ReadAllText(CardsFile);
    return Util.FromJson<List<System.Collections.Generic.Dictionary<string, string>>>(json);
  }

}