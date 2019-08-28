using Godot;
using System;

public class PauseMenu : Container, IMenu, ISubmenu, IInputHandledMenu {
  
  public IHasSubmenu parentMenu;
  public Godot.Button quitButton;
  public Godot.Button mainMenuButton;
  public Godot.Button settingsButton;
  public Godot.Button resumeButton;
  public IInputHandler inputHandler;
  private float inputDelay;
  private const float InputDelayStart = 0.1f;

  public void Init(){
    parentMenu = GetParent() as IHasSubmenu;
    PauseMode = PauseModeEnum.Process;
    inputDelay = InputDelayStart;
    InitInput();
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }

  public IHasSubmenu GetParentMenu(){
    return parentMenu;
  }

  public override void _Process(float delta){
    if(inputDelay > 0){
      inputDelay -= delta;
      return;
    }
    inputHandler.Update(delta);
  }

  private void InitInput(){
    DeviceState device = Session.GetDevice(0);
    if(device == null){
      GD.Print("Pause menu needs Device(0) to exist");
      return;
    }
    inputHandler = new MenuInputHandler(this as IInputHandledMenu);
    MappedInputSource source = new MappedInputSource(Session.GetDevice(0), FPSInputHandler.GetMappings());
    inputHandler.RegisterInputSource(source as IInputSource);
  }
  
  void InitControls(){
    SetQuitButton((Godot.Button)Menu.Button(text : "Quit", onClick : Quit));
    SetMainMenuButton((Godot.Button)Menu.Button(text : "Main Menu", onClick : QuitToMainMenu));
    SetSettingsButton((Godot.Button)Menu.Button(text : "Settings", onClick : Settings));
    SetResumeButton((Godot.Button)Menu.Button(text : "Resume", onClick : Resume));
  }
  
  void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width / 10; // relative height and width units
      float hu = height / 10;
      
      Menu.ScaleControl(resumeButton, 4 * wu, 2 * hu, 3 * wu, 0);
      Menu.ScaleControl(mainMenuButton, 4 * wu, 2 * hu, 3 * wu, 2 * hu);
      Menu.ScaleControl(settingsButton, 4 * wu, 2 * hu, 3 * wu,  4 * hu);
      Menu.ScaleControl(quitButton, 4 * wu, 2 * hu, 3 * wu,  6 * hu);
  }

  public void Select(){
  }

  public void Pause(){
    Resume();
  }

  public void Back(){

  }

  public void Move(Vector2 direction){

  }
  
  public void SetQuitButton(Godot.Button button){
    if(quitButton != null){ quitButton.QueueFree(); }
    
    quitButton = button;
    AddChild(button);
  }
  
  public void SetMainMenuButton(Godot.Button button){
    if(mainMenuButton != null){ mainMenuButton.QueueFree(); }
    
    mainMenuButton = button;
    AddChild(button);
  }

  public void SetSettingsButton(Godot.Button button){
    if(settingsButton != null){ settingsButton.QueueFree(); }
    
    settingsButton = button;
    AddChild(button);
  }

  public void SetResumeButton(Godot.Button button){
    if(resumeButton != null){ resumeButton.QueueFree(); }
    
    resumeButton = button;
    AddChild(button);
  }
  
  public void Quit(){
      Session.session.Quit();
  }
  
  public void QuitToMainMenu(){
    Session.QuitToMainMenu();
  }

  public void Settings(){
    GetParentMenu().ChangeSubmenu("SettingsMenu");
  }

  public void Resume(){
    Session.Event(SessionEvent.PauseEvent());
  }
  
}