using Godot;
using System;
using System.Collections.Generic;

public class ControlsMenu : Container, IMenu {
    public Godot.Button backButton;
    public Godot.Button saveButton;
    public Godot.Button revertButton;
    public Godot.ItemList itemList;
    public TextEdit background;
    public List<InputMapping> inputMappings;

    public bool keyboardInput = true; // Whether to display keyboard or joystick controls
    public bool waitingForInput = false;
    public int activeMapping = -1;
    public DebugInputHandler inputHandler;
    public float inputDelayRemaining;
    public const float InputDelay = 0.25f;

    
    public void Init(){
      InitControls();
      ScaleControls();
      GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
    }

    public void Clear(){
      this.QueueFree();
    }

    public void InitControls(){
      background = Menu.BackgroundBox();
      AddChild(background);

      saveButton = Menu.Button("Save", Save);
      AddChild(saveButton);

      revertButton = Menu.Button("Revert", Revert);
      AddChild(revertButton);

      backButton = Menu.Button("Back", Back);
      AddChild(backButton);

      itemList = new ItemList();
      AddChild(itemList);

      itemList.AllowRmbSelect = false;
      itemList.Connect("item_selected", this, nameof(SetInput));
      PopulateMappings(FPSInputHandler.GetMappings());
    }

    public void PopulateMappings(List<InputMapping> inputMappings){
      inputHandler = new DebugInputHandler();
      List<InputMapping> detectionMappings = keyboardInput ? DebugInputHandler.GetKeyboardMappings() : DebugInputHandler.GetJoyMappings();
      MappedInputSource source = new MappedInputSource(Session.GetDevice(0), detectionMappings);
      inputHandler.RegisterInputSource(source);
      this.inputMappings = inputMappings;
      for(int i = 0; i < inputMappings.Count; i++){
        itemList.AddItem(GetMappingText(inputMappings[i]));
      }

    }

    public override void _Process(float delta){
      if(waitingForInput){
        DetectInput(delta);
      }
    }

    public void DetectInput(float delta){
      inputDelayRemaining -= delta;
      if(inputDelayRemaining > 0f){
        GD.Print("Delaying input");
        return;
      }

      GD.Print("DetectInput");
      MappedInputEvent input = inputHandler.GetNextInput();
      if(input != null){
        GD.Print("Found " + input.ToString());
        if(keyboardInput && input.mappedEventId >= 0){
          Godot.KeyList key = (Godot.KeyList)input.mappedEventId;
          GD.Print("Keyboard key: " + key);
          inputMappings[activeMapping].inputId = input.mappedEventId;
          inputMappings[activeMapping].inputType = InputMapping.Inputs.KeyboardKey;
        }
        else if(keyboardInput){
          int mouseButton = input.mappedEventId * -1;
          inputMappings[activeMapping].inputId = mouseButton;
          inputMappings[activeMapping].inputType = InputMapping.Inputs.KeyboardKey;
        }
        itemList.SetItemText(activeMapping, GetMappingText(inputMappings[activeMapping]));
        activeMapping = -1;
        waitingForInput = false;
      }
      
    }

    public string GetMappingText(InputMapping mapping){
      string ret = "" + (FPSInputHandler.Inputs)mapping.mappedEventId + ": ";
      switch(mapping.inputType){
        case InputMapping.Inputs.JoyButton:
          ret += "Joypad button " + mapping.inputId;
          break;
        case InputMapping.Inputs.JoyAxis: 
          ret += "Joypad axis " + mapping.inputId + "(";
          ret += mapping.sensitivity < 0 ? "-)" : "+)";
          break;
        case InputMapping.Inputs.MouseButton:
          ret += "Mouse button " + mapping.inputId;
          break;
        case InputMapping.Inputs.MouseAxis: 
          ret += "Mouse axis";
          break;
        case InputMapping.Inputs.KeyboardKey:
          ret += (Godot.KeyList)mapping.inputId + " key"; 
          break;
      }
      return ret;
    }

    public void SetInput(int index){
      if(waitingForInput){
        GD.Print("Waiting for existing input");
        return;
      }
      InputMapping mapping = inputMappings[index];
      FPSInputHandler.Inputs input = (FPSInputHandler.Inputs)mapping.mappedEventId;
      GD.Print("Setting " + input);

      inputDelayRemaining = InputDelay;
      waitingForInput = true;
      activeMapping = index;
    }

    public void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width/10; // relative height and width units
      float hu = height/10;
      
      Menu.ScaleControl(background, width, height, 0, 0);
      Menu.ScaleControl(backButton, 2 * wu, hu, 0, height - hu);
      Menu.ScaleControl(saveButton, 2 * wu, hu, 8 * wu, height - hu);
      Menu.ScaleControl(revertButton, 2 * wu, hu, 4 * wu, height - hu);
      Menu.ScaleControl(itemList, 4 * wu, 6 * hu, width - 5 * wu, 2 * hu);
    }

    public void Save(){
      GD.Print("Save");
      SettingsDb db = new SettingsDb();
      foreach(InputMapping mapping in inputMappings){
        FPSInputHandler.Inputs input = (FPSInputHandler.Inputs)mapping.mappedEventId;
        db.StoreSetting("FPS" + input, mapping.Flatten());
      }
    }

    public void Revert(){
      GD.Print("Revert");
      itemList.QueueFree();
      itemList = new ItemList();
      AddChild(itemList);
      itemList.AllowRmbSelect = false;
      itemList.Connect("item_selected", this, nameof(SetInput));
      ScaleControls();
      PopulateMappings(FPSInputHandler.DefaultMappings());
    }

    public void Back(){
      Session.ChangeMenu("SettingsMenu");
    }

}
