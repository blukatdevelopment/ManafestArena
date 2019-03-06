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
  public TextEdit background;


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

    mainMenuButton = Menu.Button("Main Menu", () => { 
      ReturnToMainMenu(); 
    });
    AddChild(mainMenuButton);

    descriptionLabel = Menu.TextBox("Choose a champion.", true, true);
    AddChild(descriptionLabel);

    firstCharacterButton = Menu.Button("Beast", () => {
      SelectChampion("beast");
    });
    AddChild(firstCharacterButton);

    secondCharacterButton = Menu.Button("Mage", () => {
      SelectChampion("mage");
    });
    AddChild(secondCharacterButton);

    thirdCharacterButton = Menu.Button("Soldier", () => {
      SelectChampion("soldier");
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
      case "beast":
        return "Extracted from the great plains, this contestant is equal parts bark and bite.\n A beast man of the long house confederacy, he wields a spear and deadly claws.\n If he wins, he'll turn his homeland into a nature preserve so that his\n people can continue to live off the land.";
        break;
      case "mage":
        return "A mage of royal Persian blood, this contestant wields a powerful\n magic staff and a lifetime of training in the arcane arts.\n If he wins, he'll open a trade portal in the heart of Persia.";
        break;
      case "soldier":
        return "A mercenary defected from the Han royal guard, this contestant wields\n a crossbow and keeps a black powder pistol on his hip.\n If he wins, he'll use his \nendorsements to expand his fledgeling mercenary business.";
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

    Menu.ScaleControl(background, width, height, 0, 0);
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