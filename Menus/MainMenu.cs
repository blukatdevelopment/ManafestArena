using Godot;
using System;

public class MainMenu : Container, IMenu {
    
    public Godot.Button newGameButton;
    public Godot.Button continueGameButton;
    public Godot.Button settingsButton;
    public Godot.Button quitButton;
    public TextEdit background;

    public void Init(float minX, float minY, float maxX, float maxY){
      InitControls();
      ScaleControls();
      GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
    }
    
    public void Resize(float minX, float minY, float maxX, float maxY){

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

      newGameButton = Menu.Button(text : "New", onClick : NewGame);
      AddChild(newGameButton);

      if(CareerDb.SaveExists()){
        continueGameButton = Menu.Button(text : "Continue", onClick : ContinueGame);
        AddChild(continueGameButton);
      }

      quitButton = Menu.Button(text : "Quit", onClick : Quit);
      AddChild(quitButton);
      
      settingsButton = (Godot.Button)Menu.Button(text : "Settings", onClick : Settings);
      AddChild(settingsButton);
      
      Sound.PlaySong(Sound.Songs.FloatingHorizons);
    }

    void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width/10; // relative height and width units
      float hu = height/10;
      
      Menu.ScaleControl(background, width, height, 0, 0);
      Menu.ScaleControl(newGameButton, 2 * wu, 2 * hu, 0, 2 * hu);
      if(continueGameButton != null){
        Menu.ScaleControl(continueGameButton, 2 * wu, 2 * hu, 0, 0);
      }
      Menu.ScaleControl(settingsButton, 2 * wu, 2 * hu, 0, 4 * hu);
      Menu.ScaleControl(quitButton, 2 * wu, 2 * hu, 0, 8 * hu);
    }
    
    public void NewGame(){
      Session.ChangeMenu(Menu.Menus.NewGame);
    }

    public void ContinueGame(){
      GD.Print("ContinueGame");
      Session.session.career = CareerDb.LoadCareer();
      Session.ChangeMenu(Menu.Menus.Career);
     
    }

    public void Settings(){
      GD.Print("Settings menu");
      Session.ChangeMenu(Menu.Menus.Settings);
    }
    
    public void Quit(){
        Session.session.Quit();
    }
    
    
}
