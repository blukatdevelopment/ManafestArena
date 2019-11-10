using Godot;
using System;
using System.Collections.Generic;

public class ShopMenu : Container, IMenu {
  public Career career;
  public Button finishedButton;
  public List<Button> itemButtons;
  public Dictionary<string, ItemData> itemsDict;


  public void Init(){
    career = Career.GetActiveCareer();
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }

  void InitControls(){
    finishedButton = Menu.Button("Finish shopping", () => { 
      career.CompleteEncounter();
    });
    AddChild(finishedButton);
  }

  void PurchaseItem(string name){
    if(!itemsDict.ContainsKey(name)){
      GD.Print("itemsDict Doesn't conttain " + name);
      return;
    }

    ItemData item = itemsDict[name];
    GD.Print("Purchasing  " + item.ToString());
  }


  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(finishedButton, 2 * wu, hu, width - (2 * wu), height - hu);

    for(int i = 0; i < 4; i++){
      Menu.ScaleControl(itemButtons[i], 2 * wu, 2 * hu, i * 2 * wu, 2 * hu);
    }
    for(int i = 4; i < 8; i++){
      int off = i - 4;
      Menu.ScaleControl(itemButtons[i], 2 * wu, 2 * hu, off * 2 * wu, 4 * hu); 
    }
    
  }

}