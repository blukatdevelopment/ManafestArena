using Godot;
using System;
using System.Collections.Generic;

public class CareerMenu : Container, IMenu {
  public Career career;
  public Button mainMenuButton;
  public Control careerParent;
  public List<CareerNode> careerNodes;
  public System.Collections.Generic.Dictionary<int, Button> careerButtons;
  public TextEdit background;
  public int targetOffset;
  public int minOffset;
  public int maxOffset;

  public void Init(){
    career = Career.GetActiveCareer();
    careerNodes = career.careerNodes;
    Sound.PlayRandomSong(Sound.GetPlaylist(Sound.Playlists.Menu));
    if(career.encounterInProgress){
      GD.Print("Node in progress");
      career.BeginEncounter(career.lastNode);
    }
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }

  public override void _Process(float delta){
    HandleScrolling(delta);
  }

  public override void _Input(InputEvent evt){
    if (evt is InputEventMouseButton){
      InputEventMouseButton emb = (InputEventMouseButton)evt;
      if (emb.IsPressed()){
        if (emb.ButtonIndex == (int)ButtonList.WheelUp){
          targetOffset += 1;
        }
        if (emb.ButtonIndex == (int)ButtonList.WheelDown){
          targetOffset -= 1;
        }
      }
    }
  }

  public void HandleScrolling(float delta){
    Rect2 screen = this.GetViewportRect();
    float height = screen.Size.y;
    float hu = height/10;// relative height and width units
    float scrollVelocity = 15f;
    float maxDist = 1;
    float change = scrollVelocity * delta;
    
    float offset = careerParent.GetPosition().y/hu;
    targetOffset = Mathf.Clamp(targetOffset,minOffset,maxOffset);
    targetOffset = (int)Mathf.Clamp(targetOffset,offset-maxDist,offset+maxDist);
    float dist = Mathf.Abs(targetOffset-offset);
    if(dist<=change){
      offset = targetOffset;
    }
    else if(targetOffset>offset){
      offset+=change;
    }
    else{
      offset-=change;
    }
    careerParent.SetPosition(new Vector2(0,offset*hu));
  }

  public void InitControls(){
    background = Menu.BackgroundBox();
    AddChild(background);

    careerParent = new Control();
    AddChild(careerParent);
    minOffset = CurrentYPos();
    maxOffset = 0;
    targetOffset = minOffset;

    careerButtons = new System.Collections.Generic.Dictionary<int, Button>();

    foreach(CareerNode node in careerNodes){
      AddNodeButton(node);
    }

    mainMenuButton = Menu.Button("Main Menu", () => { 
      ReturnToMainMenu(); 
    });
    AddChild(mainMenuButton);

  }

  public void AddNodeButton(CareerNode node){
    Button button = NodeButton(node.id, node.encounter.GetDisplayName());
    careerButtons.Add(node.id, button);
    careerParent.AddChild(button);
  }

  public Button NodeButton(int id, string type){
    return Menu.Button(type, () => {
      ExecuteNode(id);
    });
  }

  void ExecuteNode(int id){
    GD.Print("Executing node " + id + ".");

    career.lastNode = id;
    ScaleControls();
    career.BeginEncounter(id);
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(background, width, height, 0, 0);
    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    careerParent.SetPosition(new Vector2(0,targetOffset*hu));

    for(int i = 0; i < Career.CareerLevels; i++){
      ScaleLevel(i);
    }
  }

  public int CurrentYPos(){
    float effectiveLevel = Career.CareerLevels;
    float ret = (int)effectiveLevel * 2;
    ret -= 6;
    return -(int)ret;
  }

  public void ScaleLevel(int level){
    List<CareerNode> nodes = CareerNode.GetByLevel(careerNodes, level);
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;

    float nodeSpacing = nodes.Count + 1;
    float wu = width/nodeSpacing;
    float hu = height/10;
    float padding = wu / ((float)nodes.Count);

    for(int i = 0; i < nodes.Count; i++){
      int nodeId = nodes[i].id;
      float xPos = (wu + padding) * i;
      xPos += (padding/2f);
      // We want the tree to display upside-down
      float effectiveLevel = Career.CareerLevels - level;
      float yPos = effectiveLevel * (hu * 2);

      float xSize = wu;
      float ySize = hu;

      bool active = CareerNodeActive(nodeId); 
      if(active){
        float scaleOffset = hu/2f;
        xPos -= (scaleOffset / 2.0f);
        yPos -= (scaleOffset / 2.0f);
        xSize += scaleOffset;
        ySize += scaleOffset;
      }

      Menu.ScaleControl(careerButtons[nodeId], xSize, ySize, xPos, yPos);
      careerButtons[nodeId].Disabled = !active;
    }

  }

  public bool CareerNodeActive(int id){
    CareerNode lastNode = CareerNode.GetById(careerNodes, career.lastNode);
    CareerNode node = CareerNode.GetById(careerNodes, id);
    bool active = false;
    if(node.level == 0 && lastNode == null){
      active = true;
    }
    else if(lastNode != null && lastNode.children.IndexOf(id) != -1){
      active = true;
    }
    return active;
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu("MainMenu");
  }
}