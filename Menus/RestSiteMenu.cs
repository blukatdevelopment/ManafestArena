using Godot;
using System;
using System.Collections.Generic;

public class RestSiteMenu : Container, IMenu {
  public Button restButton;
  public Button upgradeButton;

  public List<Button> upgradeButtons;
  public TextEdit descriptionLabel;
  public Button confirmButton;
  public TextEdit background;

  string selection = "";

  public void Init(float minX, float minY, float maxX, float maxY){
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){
    ScaleControls();
  }

  public bool IsSubMenu(){
    return false;
  }

  public void Clear(){
    this.QueueFree();
  }

  void InitControls(){
    background = Menu.BackgroundBox();
    AddChild(background);

    restButton = Menu.Button("Rest", () => { 
      HealPlayer1();
      Session.session.career.CompleteEncounter();
    });
    AddChild(restButton);

    // upgradeButton = Menu.Button("Upgrade", DisplayUpgrades);
    // AddChild(upgradeButton);
  }

  void DisplayUpgrades(){
    GD.Print("DisplayUpgrades");
    restButton.QueueFree();
    upgradeButton.QueueFree();

    upgradeButtons = new List<Button>();
    List<string> upgrades = Career.RestSiteUpgrades();

    foreach(string upgrade in upgrades){
      Button button = Menu.Button(upgrade, () => {
        SelectUpgrade(upgrade);
      });
      upgradeButtons.Add(button);
      AddChild(button);
    }

    descriptionLabel = Menu.TextBox("Select an option");
    AddChild(descriptionLabel);
    ScaleControls();

  }

  void SelectUpgrade(string upgrade){
    GD.Print("Selected " + upgrade);
    selection = upgrade;
    descriptionLabel.Text = Career.UpgradeDescription(upgrade);

    if(confirmButton == null){
      confirmButton = Menu.Button("Confirm", Confirm);
      AddChild(confirmButton);
      ScaleControls();
    }
  }

  void Confirm(){
    GD.Print("Confirming selection of " + selection);
    Session.session.career.CompleteEncounter();
  }

  void HealPlayer1(){
    GD.Print("Healing player1");
    StatsManager stats = Career.GetPlayerStats();
    int healing = stats.GetStat(StatsManager.Stats.HealthMax) / 3;
    Career.HealPlayer(healing);
  }


  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(background, width, height, 0, 0);
    if(restButton != null){
      Menu.ScaleControl(restButton, 2 * wu, 2 * hu, wu, 2 * hu);
      //Menu.ScaleControl(upgradeButton, 2 * wu, 2 * hu, width - 3 * wu, 2 * hu);
    }
    if(upgradeButtons != null && upgradeButtons.Count > 2){
      Menu.ScaleControl(upgradeButtons[0], 2 * wu, 2 * hu, wu, 2 * hu);
      Menu.ScaleControl(upgradeButtons[1], 2 * wu, 2 * hu, 4 * wu, 2 * hu);
      Menu.ScaleControl(upgradeButtons[2], 2 * wu, 2 * hu, 7 * wu, 2 * hu);
      Menu.ScaleControl(descriptionLabel, 4 * wu, 4 * hu, 3 * wu, 4 * hu);
    }
    if(confirmButton != null){
      Menu.ScaleControl(confirmButton, 2 * wu, 2 * hu, 4 * wu, 8 * hu); 
    }
  }
}