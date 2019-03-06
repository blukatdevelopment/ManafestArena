using Godot;
using System;
using System.Collections.Generic;

public class EndGameMenu : Container, IMenu {
  public Button mainMenuButton;
  public Godot.TextEdit titleLabel;
  public Godot.TextEdit creditsLabel;
  public TextEdit background;

  public void Init(float minX, float minY, float maxX, float maxY){
    InitControls();
    ScaleControls();
    CareerDb.ClearCareer();
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
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

    mainMenuButton = Menu.Button("Main Menu", () => { 
      ReturnToMainMenu(); 
    });
    AddChild(mainMenuButton);


    titleLabel = (Godot.TextEdit)Menu.TextBox(EndCopy());
    AddChild(titleLabel);

    creditsLabel = (Godot.TextEdit)Menu.TextBox(CreditsCopy());
    AddChild(creditsLabel);
  }

  public string EndCopy(){
    int victory = 0;

    if(Session.session.career != null){
      victory = Session.session.career.stats.GetStat(StatsManager.Stats.Victory);
    }
    
    if(victory == 1){
      return "Congratulations!";
    }
    return "Too bad!";
  }


  public string CreditsCopy(){
    string ret = "";
    ret += "              Credits\n";
    ret += "\n";
    ret += " Programming - Blukat\n";
    ret += "\n\n\n\n\n\n\n Thanks for playing!\n";
    return ret; 
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(background, width, height, 0, 0);
    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    Menu.ScaleControl(titleLabel, 2 * wu, hu, 4 * wu, 0);
    Menu.ScaleControl(creditsLabel, 8 * wu, 8 * hu, wu, hu);
    
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }
}