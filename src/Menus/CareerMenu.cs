using Godot;
using System;
using System.Collections.Generic;

public class CareerMenu : Container, IMenu {
  public Career career;
  public Button mainMenuButton;
  public List<CareerNode> careerNodes;
  public System.Collections.Generic.Dictionary<int, Button> careerButtons;
  public TextEdit background;
  public int nodeYOffset;

  public void Init(){
    nodeYOffset = CurrentYPos();

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
    HandleScrolling();
  }

  public void HandleScrolling(){
    Vector2 mousePos = Util.GetMousePosition();
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    if(mousePos.x < 2*wu || mousePos.x > 6*wu){
      return; // Not in the center of the screen
    }

    float y = mousePos.y;
    int increment = 0;
    int pixelsPast;

    int srs = 3; // Scroll region size
    int brs = 10 - srs; // Bottom scroll region start

    if(y < srs*hu){
      pixelsPast = (int)((srs*hu) - y);
      increment = (int)(pixelsPast / (srs*hu) * 10); 
    }
    else if(y > brs*hu){
      pixelsPast = (int)(y - (brs*hu));
      increment =  (int)(pixelsPast / (srs*hu) * 10f);
      increment *= -1;
    }

    nodeYOffset += increment;
    ScaleControls();
  }

  public void InitControls(){
    background = Menu.BackgroundBox();
    AddChild(background);

    mainMenuButton = Menu.Button("Main Menu", () => { 
      ReturnToMainMenu(); 
    });
    AddChild(mainMenuButton);

    careerButtons = new System.Collections.Generic.Dictionary<int, Button>();

    foreach(CareerNode node in careerNodes){
      AddNodeButton(node);
    }
  }

  public void AddNodeButton(CareerNode node){
    Button button = NodeButton(node.id, node.encounter.GetDisplayName());
    careerButtons.Add(node.id, button);
    AddChild(button);
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
    for(int i = 0; i < Career.CareerLevels; i++){
      ScaleLevel(i);
    }
  }

  public int CurrentYPos(){
    float effectiveLevel = Career.CareerLevels;
    float hu = this.GetViewportRect().Size.y /10;
    float ret = (int)effectiveLevel * hu * 2;
    ret -= (hu * 8);
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

    for(int i = 0; i < nodes.Count; i++){
      int nodeId = nodes[i].id;
      float padding = wu / ((float)nodes.Count);
      float xPos = (wu + padding/2f) * i;
      xPos += (padding/2f);
      // We want the tree to display upside-down
      float effectiveLevel = Career.CareerLevels - level;
      float yPos = effectiveLevel * (hu * 2);
      yPos += nodeYOffset;

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