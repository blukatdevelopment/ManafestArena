using Godot;
using System;
using System.Collections.Generic;

public class NewGameMenu : Container, IMenu {
  public Button mainMenuButton;
  public Button startGameButton;
  public TextEdit descriptionLabel;
  public string selectedChampion;

  public TextureButton firstCharacterButton;
  public TextureButton secondCharacterButton;
  public TextureButton thirdCharacterButton;
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

    descriptionLabel = Menu.TextBox("Select a champion.", true, true);
    AddChild(descriptionLabel);

    firstCharacterButton = Menu.TexturedButton("res://Textures/beast_grey.jpg", "res://Textures/beast.jpg", () => { SelectChampion("beast"); });
    AddChild(firstCharacterButton);

    secondCharacterButton = Menu.TexturedButton("res://Textures/mage_grey.jpg", "res://Textures/mage.jpg", () => { SelectChampion("mage"); });
    AddChild(secondCharacterButton);

    thirdCharacterButton = Menu.TexturedButton("res://Textures/soldier_grey.jpg", "res://Textures/soldier.jpg", () => { SelectChampion("soldier"); });
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
        return "A beast man of the great plains, wields\na spear and deadly claws.\n";
        break;
      case "mage":
        return "A royal Persian mage with a staff\nwith many spells.";
        break;
      case "soldier":
        return "A mercenary and former Han royal guard.\npacks a crossbow, flintlock pistol, and knife.";
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
    Menu.ScaleControl(firstCharacterButton, 2 * wu, 4 * hu, wu, 0);
    Menu.ScaleControl(secondCharacterButton, 2 * wu, 4 * hu, 4 * wu, 0);
    Menu.ScaleControl(thirdCharacterButton, 2 * wu, 4 * hu, 7 * wu, 0);
    Menu.ScaleControl(descriptionLabel, 4 * wu, 4 * hu, 3 * wu, 4 * hu);
    
    if(startGameButton != null){
      Menu.ScaleControl(startGameButton, 2 * wu, 2 * hu, 4 * wu, 8 * hu);
    }
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }

}