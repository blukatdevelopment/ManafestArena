using Godot;
using System;
using System.Collections.Generic;

public class PressEventEncounter : IEncounter {
  string pressEventName;

  public PressEventEncounter(){}

  public PressEventEncounter(string pressEventName){
    this.pressEventName = pressEventName;
  }

  public string GetDisplayName(){
    return "Press Event";
  }

  public void StartEncounter(){
    GD.Print("PressEventEncounter -info " + pressEventName);
    //pressEvent = CareerDb.LoadPressEvent(info);
    
    Session.ChangeMenu("PressEventMenu");
    // TODO: Load this info into menu
  }
  
  public IEncounter GetRandomEncounter(){
    return new PressEventEncounter(GetRandomPressEventName());
  }

  public static string GetRandomPressEventName(){
    return "Implement this, dog";
  }
}