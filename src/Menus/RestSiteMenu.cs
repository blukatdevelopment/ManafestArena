using Godot;
using System;
using System.Collections.Generic;

public class RestSiteMenu : MenuBase {
  public Button restButton;
  public Button upgradeButton;
  public TextEdit background;

  public override void InitControls(){
    background = Menu.BackgroundBox(this);
    restButton = Menu.Button(this, "Rest", Rest);
    upgradeButton = Menu.Button(this, "Upgrade", Upgrade);
  }

  public override void ScaleControls(){
    ScaleControl(background, screenWidth, screenHeight, 0, 0);
    ScaleControl(restButton, 2 * widthUnit, 2 * heightUnit, widthUnit, 2 * heightUnit);
    ScaleControl(upgradeButton, 2 * widthUnit, 2 * heightUnit, 7 * widthUnit, 2 * heightUnit);
  }

  public void Rest(){
    if(Session.DebugMenu.Equals("RestSiteMenu")){
      GD.Print("Selected rest option");
      return;
    }
    Career career = Career.GetActiveCareer();
    // IMPLEMENT ME
  }

  public void Upgrade(){
    ChangeSubmenu("UpgradeMenu");
  }
}