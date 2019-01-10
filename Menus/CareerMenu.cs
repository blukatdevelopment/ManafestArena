using Godot;
using System;
using System.Collections.Generic;

public class CareerMenu : Container, IMenu {
  public Button mainMenuButton;
  public Session.Gamemodes activeMode = Session.Gamemodes.None;
  public List<CareerNode> careerNodes;
  public System.Collections.Generic.Dictionary<int, Button> careerButtons;


  public void Init(float minX, float minY, float maxX, float maxY){
    InitControls();
    ScaleControls();
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){
    ScaleControls();
  }

  public bool IsSubMenu(){
    return false;
  }

  public void Clear(){
    this.QueueFree();
  }

  void InitControls(){
    mainMenuButton = Menu.Button("Main Menu", () => { 
      ReturnToMainMenu(); 
    });
    AddChild(mainMenuButton);

    careerButtons = new System.Collections.Generic.Dictionary<int, Button>();

    if(Session.session.career == null || Session.session.career.careerNodes == null){
      GD.Print("Session's Career not initialized");
      return;
    }

    careerNodes = Session.session.career.careerNodes;

    foreach(CareerNode node in careerNodes){
      AddNodeButton(node);
    }
  }

  void AddNodeButton(CareerNode node){
    Button button = NodeButton(node.nodeId, Enum.GetName(typeof(CareerNode.NodeTypes), node.nodeType));
    careerButtons.Add(node.nodeId, button);
    AddChild(button);
  }

  Button NodeButton(int id, string type){
    return Menu.Button(type, () => {
      ExecuteNode(id);
    });
  }

  void ExecuteNode(int id){
    GD.Print("Executing node " + id + ".");
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(mainMenuButton, 2 * wu, hu, 0, height - hu);
    ScaleNodeButtons();
  }

  void ScaleNodeButtons(){
    System.Collections.Generic.Dictionary<int, CareerNode[]> levels = CareerNode.GetLevels(careerNodes);
    foreach(int key in levels.Keys){
      ScaleLevel(key, levels[key]);
    }
  }

  void  ScaleLevel(int level, CareerNode[] levelNodes){
    for(int i = 0; i < levelNodes.Length; i++){
      ScaleNodeButton(levelNodes[i].nodeId, level, i, levelNodes.Length);
    }
  }

  void ScaleNodeButton(int node, int level, int x, int xMax){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    if(!careerButtons.ContainsKey(node)){
      GD.Print("Node " + node + " has no button.");
      return;
    }

    float xSpace = 6 * wu;
    float xSpaceUnits = xSpace / (xMax + 1);
    float xPos = wu + (x + 1) * xSpaceUnits;

    float yPos = hu * (1+level);

    Button nodeButton = careerButtons[node];
    Menu.ScaleControl(nodeButton, wu, hu, xPos, yPos);
    GD.Print("Scaled node " + node + " at level " + level + ", [" + xPos + "," + yPos + "]");
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu(Menu.Menus.Main);
  }


}