using Godot;
using System;
using System.Collections.Generic;

public class EndGameMenu : Container, IMenu {
  public Button mainMenuButton;
  public Godot.TextEdit titleLabel;
  public Godot.TextEdit creditsLabel;
  public TextEdit background;

  public void Init(){
    Sound.PlayRandomSong(Sound.GetPlaylist(Sound.Playlists.Menu));
    InitControls();
    ScaleControls();
    CareerDb.ClearCareer();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
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
    //int victory = 0;

    // if(Session.session.career != null){
    //   victory = 1;Session.session.career.stats.GetStat(StatsManager.Stats.Victory);
    // }
    
    // if(victory == 1){
    //   return "Congratulations!";
    // }
    // return "Better luck next time!";
    return "Thanks for playing!";
  }


  public string CreditsCopy(){
    string ret = "";
    ret += "              Credits\n";
    ret += "\n";
    ret += " Programming - Blukat\n";
    ret += "\n\n\n\n\n\n\n Thanks for playing!\n";
    ret += " Music - Halley Labs\n";
    ret += " lapfoxtrax.com/music\n";
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
    Session.ChangeMenu("MainMenu");
  }
}