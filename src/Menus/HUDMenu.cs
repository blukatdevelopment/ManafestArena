using Godot;
using System;

public class HUDMenu : MenuBase{

  public float delay = 0.0f;
  public Godot.Label statBox;
  public ColorProgressBar staminaBar;
  public ColorProgressBar healthBar;
  public ColorProgressBar manaBar;
  public Godot.Label itemBox;
  public Godot.Label objectiveBox;
  public Godot.Label interactionBox;
  public bool paused = false;
  public Actor player;
  
  public override void _Process(float delta){
    delay += delta;

    if(delay > 0.033f){
      delay -= 0.033f;
      UpdateHud();
    }
  }

  public override void InitData(){
    PauseMode = PauseModeEnum.Stop;
    Input.SetMouseMode(Input.MouseMode.Captured);

    if(Session.DebugMenu == "HUDMenu"){
      player = ActorFactory.DebugPlayerCharacter();
    }
    else{
      player = Session.GetPlayer();
    }
  }

  public void TogglePause(){
    paused=!paused;
    if(paused){
      ChangeSubmenu("PauseMenu");
    }
    else{
      ChangeSubmenu("");
    }
  }

  public bool Paused(){
    return paused;
  }

  public void UpdateHud(){
    if(player == null){
      GD.Print("Player 1 doesn't exist.");
      return;
    }
    Stats stats = player.GetStats();
    statBox.Text = StatusText(player);

    staminaBar.UpdateProgress(stats.Stamina, stats.StaminaMax);
    healthBar.UpdateProgress(stats.Health, stats.HealthMax);
    manaBar.UpdateProgress(stats.Mana, stats.ManaMax);

    string itemText = player.hotbar.GetInfo();

    itemBox.Text = itemText;
    
    string objectiveText = Session.GetObjectiveText();
    objectiveBox.Text = objectiveText;
  }

  private string StatusText(Actor player){
    if(player.stats == null){
      return "";
    }
    Stats stats = player.stats;

    string ret = "";

    ret += "Health: " + stats.Health + "/" + stats.HealthMax;
    ret += "\nStamina: " + stats.Stamina + "/" + stats.StaminaMax;
    ret += "\nMana: " + stats.Mana + "/" + stats.ManaMax;
    return ret;
  }

  public override void InitControls(){
    statBox = Menu.Label(this, "stats");

    if(player == null){
      GD.Print("Player 1 doesn't exist.");
      return;
    }
    
    Stats stats = player.GetStats();

    staminaBar = new ColorProgressBar(this, new Color(0,0.8f,0),stats.Stamina, stats.StaminaMax);
    healthBar = new ColorProgressBar(this, new Color(1,0,0),stats.Stamina, stats.HealthMax);
    manaBar = new ColorProgressBar(this, new Color(0,0,1),stats.Mana, stats.ManaMax);

    itemBox = Menu.Label(this, "item");
    objectiveBox = Menu.Label(this, "Objective Info");
    interactionBox = Menu.Label(this, "");

  }

  public override void ScaleControls(){
    ScaleControl(statBox, 2 * widthUnit, heightUnit, 0, 0.5f * heightUnit);
    ScaleControl(staminaBar, 3 * widthUnit, 0.3f * heightUnit, 0.3f * widthUnit, screenHeight - 1.5f * heightUnit);
    staminaBar.ScaleControls();
    ScaleControl(healthBar, 3 * widthUnit, 0.3f * heightUnit, 0.3f * widthUnit, screenHeight - 1.1f * heightUnit);
    healthBar.ScaleControls();
    ScaleControl(manaBar, 3 * widthUnit, 0.3f * heightUnit, 0.3f * widthUnit, screenHeight - 0.7f * heightUnit);
    manaBar.ScaleControls();
    ScaleControl(itemBox, 2 * widthUnit, heightUnit, 8 * widthUnit, 9 * heightUnit);
    ScaleControl(objectiveBox, 4 * widthUnit, heightUnit, 3 * widthUnit, 0);
    ScaleControl(interactionBox, 4 * widthUnit, heightUnit, 3 * widthUnit, 7 * heightUnit);
  }
}
