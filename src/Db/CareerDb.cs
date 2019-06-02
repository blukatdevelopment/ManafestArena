/*
  Responsible for abstracting away file I/O for career data.
*/
using Godot;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CareerDb {
  const string StatsFile = "Saves/stats.csv";
  const string CareerFile = "Saves/career_tree.csv";


  public static bool CareerExists(){
    return false;
  }

  public static Career LoadCareer(){
    //GD.Print("CareerDb.LoadCareer");
    // List<CareerNode> nodes = CareerNode.FromRows(CSV.ReadRows(CareerFile));
    // StatsManager stats = StatsManager.FromRows(CSV.ReadRows(StatsFile));
    // return Career.Factory(nodes, stats);
    return null;
  }

  public static void SaveCareer(Career career){
    //GD.Print("CareerDb.SaveCareer");
    //CSV.WriteToFile(StatsFile, career.stats.GetRows());
    //CSV.WriteToFile(CareerFile, CareerNode.ToRows(career.careerNodes));
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

  public static PressEvent LoadPressEvent(string name){
    return null;
    //return new PressEvent(CSV.ReadRows(name));
  }

  public static List<string> PressEventFiles(){
    string[] filePaths = System.IO.Directory.GetFiles("PressEvents/", "*.csv", SearchOption.TopDirectoryOnly);
    return new List<string>(filePaths);
  }

  public static List<string> Arenas(){
    return new List<string>{"Arena1", "Arena2", "Arena3", "Arena4"};
  }

}