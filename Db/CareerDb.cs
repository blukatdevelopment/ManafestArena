/*
  Responsible for abstracting away file I/O for career data.
*/
using Godot;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CareerDb {

  public static bool CareerExists(){
    return false;
  }

  public static Career LoadCareer(){
    GD.Print("CareerDb.LoadCareer");
    return null;
  }

  public static void SaveCareer(Career career){
    GD.Print("CareerDb.SaveCareer");
  }

  public static void ClearCareer(){
    GD.Print("CareerDb.ClearCareer");
  }

}