using Godot;
using System;
using System.Collections.Generic;

public class UpgradeMenu : MenuBase {
  public List<Dictionary<string, string>> cards;
  public TextEdit background;
  public List<Button> upgradeButtons;
  public const int CardsPerRow = 4;
  public const int CardWidth = 2; // in width units
  public const int CardHeight = 4; // in height units
  public const float CardRowMargin = 0.1f;
  public const float CardRowOffset = 1; // in width units

  public override void InitData(){
    List<string> cardNames = null;
    if(Session.DebugMenu == "RestSiteMenu"){
      cardNames = new List<string>{
        "sword", "sword", "sword", "sword", "sword", "sword", "sword", 
        "crossbow", "crossbow", "crossbow", "crossbow", "crossbow", "crossbow"
      };
    }
    else{
      // IMPLEMENT ME
    }
    cards = new List<Dictionary<string, string>>();
    foreach(string cardName in cardNames){
      Dictionary<string, string> card = CareerDb.GetCard(cardName);
      if(card != null){
        cards.Add(card);
      }
    }    
  }

  public override void InitControls(){
    background = Menu.BackgroundBox(this);

    upgradeButtons = new List<Button>();
    foreach(Dictionary<string, string> card in cards){
      string upgradeName = card["name"] + "+"; 
      if(CareerDb.GetCard(upgradeName) == null){
        GD.Print("Card " + card["name"] + " has no upgrade.");
        continue;
      }
      Button button = Menu.Button(this, card["name"], () => {
        SelectUpgrade(upgradeName);
      });
      upgradeButtons.Add(button);
    }    
  }

  public override void ScaleControls(){
    int i = 0;
    int j = 0;
    foreach(Button button in upgradeButtons){
      float x = (widthUnit * CardWidth * i) + (widthUnit / 2.0f) + (widthUnit * CardWidth * i * CardRowMargin);
      float y = (heightUnit * CardHeight * j);

      Menu.ScaleControl(button, CardWidth * widthUnit, CardHeight * heightUnit, x, y);
      i++;
      if(i >= CardsPerRow){
        i = 0;
        j++;
      }
    }

    Menu.ScaleControl(background, screenWidth, screenHeight, 0, 0);
  }

  public void SelectUpgrade(string upgradeName){
    GD.Print("Display upgrade: " + upgradeName);
  }

}