/*
  This pseudo-singleton is the focal point for the active session's state.
  The Session should be the root of the scene in the game. If it's null, things simply
  won't work.

*/
using Godot;
using System;
using System.Collections.Generic;
using System.Text;


public class Session : Node {
  public static Session session;
  
  public NetworkSession netSes;
  public Random random;
  public AudioStreamPlayer jukeBox;
  public Vector2 mousePosition;
  public Vector2 mouseMovement;
  public Sound.Songs currentSong;

  public Node activeMenu;
  public List<Node> activeGamemodes;

  // Settings
  public float masterVolume, sfxVolume, musicVolume;
  public string userName;
  public float mouseSensitivityX, mouseSensitivityY;

  // Input
  List<DeviceState> deviceStates;
  

  public static int NextItemId(){
    return 0;
  }

  public static int NextActorId(){
    return 0;
  }

  public override void _Ready() {
    activeGamemodes = new List<Node>();
    EnforceSingleton();
    ChangeMenu("MainMenu");
    InitJukeBox();
    InitSettings();
    PerformTests();

    AddDevice(0);
  }

  public override void _Process(float delta){
    foreach(Node modeNode in activeGamemodes){
      IGamemode gamemode = modeNode as IGamemode;
      if(gamemode != null){
        gamemode.Update(delta);
      }
    }
  }

  public override void _Input(Godot.InputEvent evt){
    InputEventMouseMotion mot = evt as InputEventMouseMotion;
    if(mot != null){
      mousePosition = mot.GlobalPosition;
      mouseMovement = mot.Relative;
    }
  }

  public void PerformTests(){
    Test.Init();
  }

  public static void AddGamemode(Node node){
    Session.session.activeGamemodes.Add(node);
    Session.session.AddChild(node);
  }

  public void InitSettings(){
    SettingsDb db = new SettingsDb();
    masterVolume = Util.ToFloat(db.SelectSetting("master_volume"));
    sfxVolume = Util.ToFloat(db.SelectSetting("sfx_volume"));
    musicVolume = Util.ToFloat(db.SelectSetting("music_volume"));
    userName = db.SelectSetting("username");
    mouseSensitivityX = Util.ToFloat(db.SelectSetting("mouse_sensitivity_x"));
    mouseSensitivityY = Util.ToFloat(db.SelectSetting("mouse_sensitivity_y"));

    deviceStates = new List<DeviceState>();
    AddDevice(0); // Input player1, as they can be assumed to exist

    Sound.RefreshVolume();
  }

  public static void AddDevice(int joypad){
    if(GetDevice(0) != null){
      GD.Print("Multiple devices are not currently supported");
      return;
    }
    DeviceState ds = new DeviceState(joypad);
    Session.session.deviceStates.Add(ds);
    Session.session.AddChild(ds);
  }

  public static DeviceState GetDevice(int joypad){
    foreach(DeviceState device in Session.session.deviceStates){
      if(device.joypad == joypad){
        return device;
      }
    }
    return null;
  }

  public static void SaveSettings(){
    SettingsDb db = new SettingsDb();
    
    db.StoreSetting("master_volume", "" + Session.session.masterVolume);
    db.StoreSetting("sfx_volume", "" + Session.session.sfxVolume);
    db.StoreSetting("music_volume", "" + Session.session.musicVolume);
    db.StoreSetting("mouse_sensitivity_x", "" + Session.session.mouseSensitivityX);
    db.StoreSetting("mouse_sensitivity_y", "" + Session.session.mouseSensitivityY);
    db.StoreSetting("username", Session.session.userName);
    Sound.RefreshVolume();
  }
  
  public void Quit(){
    GetTree().Quit(); 
  }
  
  public static Actor GetPlayer(){
    foreach(Node gamemodeNode in Session.session.activeGamemodes){
      Actor actor = GetPlayer(gamemodeNode as IGamemode);
      if(actor != null){
        return actor;
      }
    }
    return null;
  }

  public static Actor GetPlayer(IGamemode gamemode){
    if(gamemode == null){
      return null;
    }
    return gamemode.GetPlayer();
  }

  public static void InitJukeBox(){
    if(Session.session.jukeBox != null){
      return;
    }

    Session.session.jukeBox = new AudioStreamPlayer();
    Session.session.AddChild(Session.session.jukeBox);
  }
  
  public static System.Random GetRandom(){
    if(Session.session.netSes != null && Session.session.netSes.random != null){
      return Session.session.netSes.random;
    }

    if(Session.session.random != null){
      return Session.session.random;
    }

    Session.session.random = new System.Random();
    return Session.session.random;
  }

  public static bool NetActive(){
    if(session.netSes != null){
      return true;
    }
    
    return false;
  }

  /* Used for syncing items. */
  public static bool IsServer(){
    if(session.netSes != null){
      return session.netSes.isServer;
    }
    
    return false;
  }

  /* Convenience method for creating nodes. */
  public static Node Instance(string path){
    byte[] bytes = Encoding.Default.GetBytes(path);
    path = Encoding.UTF8.GetString(bytes);
    PackedScene packedScene = (PackedScene)GD.Load(path);
    
    if(packedScene == null){
      GD.Print("Path [" + path + "] is invalid." );
      return null;
    }
    
    return packedScene.Instance();
  }
  
  /* Remove game nodes/variables in order to return it to a menu. */
  public static void ClearGame(bool keepNet = false){
    Session ses = Session.session;
    foreach(Node node in ses.activeGamemodes){
      node.QueueFree();
    }
    ses.activeGamemodes = new List<Node>();
    Input.SetMouseMode(Input.MouseMode.Visible);
  }
  
  public static Node GameNode(){
    Session ses = Session.session;
    foreach(Node node in ses.activeGamemodes){
      return node;
    }
    
    return ses;
  }

  public static Spatial GameSpatial(){
    Session ses = Session.session;
    foreach(Node node in ses.activeGamemodes){
      Spatial spat = node as Spatial;
      if(spat != null){
        return spat;
      }
    }
    
    return null;
  }
  
  public static void QuitToMainMenu(){
    Session.ChangeMenu("MainMenu");
    Session.ClearGame();
  }

  public static void ChangeMenu(string menuName){
    Session ses = Session.session;
    if(ses.activeMenu != null){
      ses.activeMenu.QueueFree();
    }

    Input.SetMouseMode(Input.MouseMode.Visible);
    Node menuNode = Menu.MenuFactory(menuName);
    IMenu menu = menuNode as IMenu;
    if(menu == null){
      GD.Print("Menu " + menuName + " was null.");
    }
    else{
      ses.activeMenu = menuNode;
      ses.AddChild(menuNode);
      menu.Init();
    }
  }
  
  private void EnforceSingleton(){
    if(Session.session == null){ Session.session = this; }
    else{ this.QueueFree(); }
  }
  
  public static string GetObjectiveText(){
    Session ses = Session.session;
    
    foreach(Node gamemodeNode in ses.activeGamemodes){
      IGamemode gamemode = gamemodeNode as IGamemode;
      if(gamemode != null){
        return gamemode.GetObjectiveText();
      }
    }
    
    return "No gamemode active";
  }
  
  public void HandleEvent(SessionEvent sessionEvent){
    foreach(Node gamemodeNode in Session.session.activeGamemodes){
      IGamemode gamemode = gamemodeNode as IGamemode;
      if(gamemode != null){
        gamemode.HandleEvent(sessionEvent);
      }
    }
  }
  
  public static void Event(SessionEvent sessionEvent){
    Session.session.HandleEvent(sessionEvent);
  }

  public static Vector3 WorldPosition(Item item){
    return item.Translation;
  }
}