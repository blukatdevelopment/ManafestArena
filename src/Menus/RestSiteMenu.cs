using Godot;
using System;
using System.Collections.Generic;

public class RestSiteMenu : Container, IMenu {
  public Career career;
  public Button restButton;
  public Button upgradeButton;

  public List<Button> upgradeButtons;
  public TextEdit descriptionLabel;
  public Button confirmButton;
  public TextEdit background;
  public Node submenu;

  public List<string> upgrades;

  string selection = "";

  public void Init(){
    if(Session.DebugMenu.Equals("RestSiteMenu")){
      LoadDebugData();
    }
    else{
      LoadData();
    }

    career = Career.GetActiveCareer();
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }

  public void ChangeSubmenu(string menuName = null){
    Menu.ChangeSubmenu(submenu, menuName);
  }

  public void InitControls(){}

  public void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(background, width, height, 0, 0);
    if(restButton != null){
      Menu.ScaleControl(restButton, 2 * wu, 2 * hu, wu, 2 * hu);
      if(upgradeButton != null){
        Menu.ScaleControl(upgradeButton, 2 * wu, 2 * hu, width - 3 * wu, 2 * hu);
      }
    }
    if(upgradeButtons != null){
      if(upgradeButtons.Count > 0){
        Menu.ScaleControl(upgradeButtons[0], 2 * wu, 2 * hu, wu, 2 * hu);  
      }
      if(upgradeButtons.Count > 1){
        Menu.ScaleControl(upgradeButtons[1], 2 * wu, 2 * hu, 4 * wu, 2 * hu);  
      }
      if(upgradeButtons.Count > 2){
        Menu.ScaleControl(upgradeButtons[2], 2 * wu, 2 * hu, 7 * wu, 2 * hu);
      }
      Menu.ScaleControl(descriptionLabel, 4 * wu, 4 * hu, 3 * wu, 4 * hu);
    }
    if(confirmButton != null){
      Menu.ScaleControl(confirmButton, 2 * wu, 2 * hu, 4 * wu, 8 * hu); 
    }
  }
}