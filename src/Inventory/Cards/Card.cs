/*
  An individual card that plays a specific effect
*/
using System;
using System.Collections.Generic;
using Godot;

public class Card {
  public string name, icon;

  public bool IsUpgraded(){
    return name.Contains("+");
  }

  public string GetUpgrade(){
    if(IsUpgraded()){
      return "";
    }
    return name + "+";
  }

  public virtual void LoadData(Dictionary<string, string> data){}

  public virtual void Play(){}

  // Assuming cards are configured properly. 
  // We can add a check elsewhere if invalid cards become an issue.
  public static Card Factory(Dictionary<string, string> data){
    object menuObj = Activator.CreateInstance(Type.GetType(data["name"]));
    Card card = menuObj as Card;

    card.LoadData(data);

    return card;
  }
}