using Godot;
using System;

public class SettingsMenu : Container, IMenu, ISubmenu {
    public IHasSubmenu parentMenu;
    public Godot.Button backButton;
    public Godot.Button revertButton;
    public Godot.Button saveButton;
    public Godot.HSlider masterVolumeSlider;
    public Godot.TextEdit masterVolumeLabel;
    public Godot.HSlider sfxVolumeSlider;
    public Godot.TextEdit sfxVolumeLabel;
    public Godot.HSlider musicVolumeSlider;
    public Godot.TextEdit musicVolumeLabel;
    public Godot.HSlider mouseXSlider;
    public Godot.TextEdit mouseXLabel;
    public Godot.HSlider mouseYSlider;
    public Godot.TextEdit mouseYLabel;
    public Godot.TextEdit userNameBox;
    public Godot.Button controlsButton;
    public TextEdit background;

    public void Init(){
      parentMenu = GetParent() as IHasSubmenu;
      PauseMode = PauseModeEnum.Process;
      InitControls();
      ScaleControls();
      GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
    }

  public IHasSubmenu GetParentMenu(){
    return parentMenu;
  }
  
    public void InitControls(){
      background = Menu.BackgroundBox();
      AddChild(background);

      backButton = Menu.Button("Back", Back);
      AddChild(backButton);

      revertButton = Menu.Button("Revert", RevertSettings);
      AddChild(revertButton);

      saveButton = Menu.Button("Save", SaveSettings);
      AddChild(saveButton);      

      controlsButton = Menu.Button("Controls", ToControlsMenu);
      AddChild(controlsButton);

      masterVolumeSlider = Menu.HSlider(0f, 1.0f, Session.session.masterVolume, 0.05f);
      AddChild(masterVolumeSlider);
      masterVolumeSlider.Connect("value_changed", this, nameof(UpdateMasterVolume));
      
      masterVolumeLabel = (Godot.TextEdit)Menu.TextBox("Master Volume: " + Session.session.masterVolume, false);
      AddChild(masterVolumeLabel);

      sfxVolumeSlider = Menu.HSlider(0f, 1.0f, Session.session.sfxVolume, 0.05f);
      AddChild(sfxVolumeSlider);
      sfxVolumeSlider.Connect("value_changed", this, nameof(UpdateSfxVolume));
      
      sfxVolumeLabel = (Godot.TextEdit)Menu.TextBox("Sound Effects Volume: " + Session.session.sfxVolume, false);
      AddChild(sfxVolumeLabel);

      musicVolumeSlider = Menu.HSlider(0f, 1.0f, Session.session.musicVolume, 0.05f);
      AddChild(musicVolumeSlider);
      musicVolumeSlider.Connect("value_changed", this, nameof(UpdateMusicVolume));
      
      musicVolumeLabel = (Godot.TextEdit)Menu.TextBox("Music Volume: " + Session.session.musicVolume, false);
      AddChild(musicVolumeLabel);

      mouseXSlider = Menu.HSlider(0f, 5.0f, Session.session.mouseSensitivityX, 0.05f);
      AddChild(mouseXSlider);
      mouseXSlider.Connect("value_changed", this, nameof(UpdateMouseX));
      
      mouseXLabel = (Godot.TextEdit)Menu.TextBox("Mouse Sensitivity X: " + Session.session.mouseSensitivityX, false);
      AddChild(mouseXLabel);

      mouseYSlider = Menu.HSlider(0f, 5.0f, Session.session.mouseSensitivityY, 0.05f);
      AddChild(mouseYSlider);
      mouseYSlider.Connect("value_changed", this, nameof(UpdateMouseY));
      
      mouseYLabel = Menu.TextBox("Mouse Sensitivity Y: " + Session.session.mouseSensitivityY, false);
      AddChild(mouseYLabel);

      userNameBox = Menu.TextBox(Session.session.userName, false);
      AddChild(userNameBox);
    }

    void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width/10; // relative height and width units
      float hu = height/10;
      
      Menu.ScaleControl(background, width, height, 0, 0);
      Menu.ScaleControl(backButton, 2 * wu, hu, 0, height - hu);
      Menu.ScaleControl(revertButton, 2 * wu, hu, 4 * wu, height - hu);
      Menu.ScaleControl(saveButton, 2 * wu, hu, 8 * wu, height - hu);
      Menu.ScaleControl(controlsButton, 2 * wu, hu, 8 * wu, 0);
      Menu.ScaleControl(masterVolumeLabel, 2 * wu, 0.5f * hu, 0, 2 * hu);
      Menu.ScaleControl(masterVolumeSlider, 2 * wu, 0.5f * hu, 0, 2.5f * hu);
      Menu.ScaleControl(sfxVolumeLabel, 2 * wu, 0.5f * hu, 0, 3 * hu);
      Menu.ScaleControl(sfxVolumeSlider, 2 * wu, 0.5f * hu, 0, 3.5f * hu);
      Menu.ScaleControl(musicVolumeLabel, 2 * wu, 0.5f * hu, 0, 4 * hu);
      Menu.ScaleControl(musicVolumeSlider, 2 * wu, 0.5f * hu, 0, 4.5f * hu);
      Menu.ScaleControl(mouseXLabel, 2 * wu, 0.5f * hu, 0, 5 * hu);
      Menu.ScaleControl(mouseXSlider, 2 * wu, 0.5f * hu, 0, 5.5f * hu);
      Menu.ScaleControl(mouseYLabel, 2 * wu, 0.5f * hu, 0, 6 * hu);
      Menu.ScaleControl(mouseYSlider, 2 * wu, 0.5f * hu, 0, 6.5f * hu);
      Menu.ScaleControl(userNameBox, 2 * wu, 0.5f * hu, 0, 7 * hu);
    }

    public void ToControlsMenu(){
      if(parentMenu!=null){
        parentMenu.ChangeSubmenu("ControlsMenu");
        return;
      }
      Session.ChangeMenu("ControlsMenu");
    }

    public void Back(){
      if(parentMenu!=null){
        parentMenu.ChangeSubmenu("PauseMenu");
        return;
      }
      Session.ChangeMenu("MainMenu");
    }

    public void UpdateMasterVolume(float val){
      string str = "Master Volume: " + val;
      masterVolumeLabel.SetText(str);
    }

    public void UpdateSfxVolume(float val){
      string str = "Sound Effects Volume: " + val;
      sfxVolumeLabel.SetText(str);
    }

    public void UpdateMusicVolume(float val){
      string str = "Music Volume: " + val;
      musicVolumeLabel.SetText(str);
    }

    public void UpdateMouseX(float val){
      string str = "Mouse Sensitivity X: " + val;
      mouseXLabel.SetText(str);
    }

    public void UpdateMouseY(float val){
      string str = "Mouse Sensitivity Y: " + val;
      mouseYLabel.SetText(str);
    }


    public void SaveSettings(){
      Session.session.masterVolume = masterVolumeSlider.Value;
      Session.session.sfxVolume = sfxVolumeSlider.Value;
      Session.session.musicVolume = musicVolumeSlider.Value;
      Session.session.mouseSensitivityX = mouseXSlider.Value;
      Session.session.mouseSensitivityY = mouseYSlider.Value;
      Session.session.userName = userNameBox.GetText();

      Sound.RefreshVolume();
      Session.SaveSettings();
    }

    public void RevertSettings(){
      masterVolumeSlider.Value = Session.session.masterVolume;
      sfxVolumeSlider.Value = Session.session.sfxVolume;
      musicVolumeSlider.Value = Session.session.musicVolume;
      mouseXSlider.Value = Session.session.mouseSensitivityX;
      mouseYSlider.Value = Session.session.mouseSensitivityY;

      userNameBox.SetText(Session.session.userName);
    }
}
