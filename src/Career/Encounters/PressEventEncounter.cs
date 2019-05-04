using Godot;
using System;
using System.Collections.Generic;

public class PressEventEncounter : IEncounter {
  string info;

  public string GetDisplayName(){
    return "";
  }

  public void StartEncounter(){
    GD.Print("PressEventEncounter -info " + info);
    //pressEvent = CareerDb.LoadPressEvent(info);
    
    Session.ChangeMenu(Menu.Menus.PressEvent);
    // TODO: Load this info into menu
  }
  
  public IEncounter GetRandomEncounter(){
    return null;
  }
}