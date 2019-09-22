using Godot;
using System;

public class HUDMenu : Container, IMenu, IHasSubmenu{

  public float delay = 0.0f;
  public Godot.Label statBox;
  public ColorProgressBar staminaBar;
  public ColorProgressBar healthBar;
  public ColorProgressBar manaBar;
  public Godot.Label itemBox;
  public Godot.Label objectiveBox;
  public Godot.Label interactionBox;
  public Node submenu;
  public bool paused = false;
  
  public override void _Process(float delta){
    delay += delta;

    if(delay > 0.033f){
      delay -= 0.033f;
      UpdateHud();
    }
  }

  public void Init(){
    PauseMode = PauseModeEnum.Stop;
    Input.SetMouseMode(Input.MouseMode.Captured);
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }

  public void ChangeSubmenu(string menuName = null){
    Menu.ChangeSubmenu(submenu, menuName);
  }

  public void TogglePause(){
    paused=!paused;
    if(paused){
      ChangeSubmenu("PauseMenu");
    }
    else{
      ChangeSubmenu();
    }
  }

  public bool Paused(){
    return paused;
  }

  public void UpdateHud(){
    Actor player = Session.GetPlayer();
    
    if(player == null){
      GD.Print("Player 1 doesn't exist.");
      return;
    }
    IStats stats = player.GetStats();
    statBox.Text = StatusText(player);

    staminaBar.UpdateProgress(stats.GetStat("stamina"),stats.GetStat("staminamax"));
    healthBar.UpdateProgress(stats.GetStat("health"),stats.GetStat("healthmax"));
    manaBar.UpdateProgress(stats.GetStat("mana"),stats.GetStat("manamax"));

    string itemText = player.hotbar.GetInfo();

    itemBox.Text = itemText;
    
    string objectiveText = Session.GetObjectiveText();
    objectiveBox.Text = objectiveText;
  }

  private string StatusText(Actor player){
    if(player.stats == null){
      return "";
    }
    IStats stats = player.stats;

    string ret = "";

    ret += "Health: " + stats.GetStat("health") + "/" + stats.GetStat("healthmax");
    ret += "\nStamina: " + stats.GetStat("stamina") + "/" + stats.GetStat("staminamax");
    ret += "\nMana: " + stats.GetStat("mana") + "/" + stats.GetStat("manamax");
    return ret;

  }

  void InitControls(){
    statBox = Menu.Label("stats");
    AddChild(statBox);

    IStats stats = Session.GetPlayer().GetStats();

    staminaBar = new ColorProgressBar(new Color(0,0.8f,0),stats.GetStat("stamina"),stats.GetStat("staminamax"));
    AddChild(staminaBar);

    healthBar = new ColorProgressBar(new Color(1,0,0),stats.GetStat("health"),stats.GetStat("healthmax"));
    AddChild(healthBar);

    manaBar = new ColorProgressBar(new Color(0,0,1),stats.GetStat("mana"),stats.GetStat("manamax"));
    AddChild(manaBar);

    itemBox = Menu.Label("item");
    AddChild(itemBox);
    
    objectiveBox = Menu.Label("Objective Info");
    AddChild(objectiveBox);

    interactionBox = Menu.Label("");
    AddChild(interactionBox);
  }

  public void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(statBox, 2 * wu, hu, 0, 0.5f* hu);
    Menu.ScaleControl(staminaBar,3 * wu,0.3f*hu,0.3f*wu,height - 1.5f*hu);
    staminaBar.ScaleControls();
    Menu.ScaleControl(healthBar,3 * wu,0.3f*hu,0.3f*wu,height - 1.1f*hu);
    healthBar.ScaleControls();
    Menu.ScaleControl(manaBar,3 * wu,0.3f*hu,0.3f*wu,height - 0.7f*hu);
    manaBar.ScaleControls();
    Menu.ScaleControl(itemBox, 2 * wu, hu, 8 * wu, 9 * hu);
    Menu.ScaleControl(objectiveBox, 4 * wu, hu, 3 * wu, 0);
    Menu.ScaleControl(interactionBox, 4 * wu, hu, 3 * wu, 7 * hu);
  }
}
