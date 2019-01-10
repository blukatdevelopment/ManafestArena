using Godot;
using System;
using System.Collections.Generic;

public class NewGameMenu : Container, IMenu {
  public Button mainMenuButton;
  public Button startGameButton;


  public void Init(float minX, float minY, float maxX, float maxY){
    InitControls();
    ScaleControls();
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
    mainMenuButton = Menu.Button("Main Menu", () => { 
      ReturnToMainMenu(); 
    });
    AddChild(mainMenuButton);

    startGameButton = Menu.Button("Start Game", StartGame);
    AddChild(startGameButton);

  }

  void StartGame(){
    GD.Print("Start game");
    Career.StartNewCareer();
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    Menu.ScaleControl(startGameButton, 2 * wu, hu, 4 * wu, height - hu);
    
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }


}