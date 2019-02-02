using Godot;
using System;
using System.Collections.Generic;

public class NewGameMenu : Container, IMenu {
  public Button mainMenuButton;
  public Button startGameButton;
  public TextEdit descriptionLabel;
  public string selectedChampion;
  public Button firstCharacterButton;
  public Button secondCharacterButton;
  public Button thirdCharacterButton;


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

    descriptionLabel = Menu.TextBox("Choose a champion.");
    AddChild(descriptionLabel);

    firstCharacterButton = Menu.Button("Fred", () => {
      SelectChampion("fred");
    });
    AddChild(firstCharacterButton);

    secondCharacterButton = Menu.Button("Velma", () => {
      SelectChampion("velma");
    });
    AddChild(secondCharacterButton);

    thirdCharacterButton = Menu.Button("Scoob", () => {
      SelectChampion("scoob");
    });
    AddChild(thirdCharacterButton);

  }

  void SelectChampion(string characterName){
    if(startGameButton == null){
      startGameButton = Menu.Button("Start Game", StartGame);
      AddChild(startGameButton);
      ScaleControls();
    }

    descriptionLabel.Text = CharacterDescription(characterName);
    selectedChampion = characterName;
  }

  string CharacterDescription(string characterName){
    switch(characterName.ToLower()){
      case "fred":
        return "We're up to our ascotts in a mystery!";
        break;
      case "velma":
        return "Jinkies!";
        break;
      case "scoob":
        return "Roobie doobie doo!";
        break;
    }
    return "NULL";
  }

  void StartGame(){
    Career.StartNewCareer(selectedChampion);
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    Menu.ScaleControl(firstCharacterButton, 2 * wu, 2 * hu, wu, 2 * hu);
    Menu.ScaleControl(secondCharacterButton, 2 * wu, 2 * hu, 4 * wu, 2 * hu);
    Menu.ScaleControl(thirdCharacterButton, 2 * wu, 2 * hu, 7 * wu, 2 * hu);
    Menu.ScaleControl(descriptionLabel, 4 * wu, 4 * hu, 3 * wu, 4 * hu);
    
    if(startGameButton != null){
      Menu.ScaleControl(startGameButton, 2 * wu, 2 * hu, 4 * wu, 8 * hu);
    }
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }

}