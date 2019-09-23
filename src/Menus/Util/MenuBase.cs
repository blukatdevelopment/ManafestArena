/*
  An attempt to move duplicated logic and helper methods into a base class
  to simplify menus.
*/

using Godot;
using System;
using System.Collections.Generic;

public class MenuBase : Container, IMenu {
  public Node submenu;
  public float screenWidth, screenHeight, widthUnit, heightUnit;
  public float widthUnitDenominator, heightUnitDenominator;
  public const float DefaultWidthUnitDenominator = 10f;
  public const float DefaultHeightUnitDenominator = 10f;

  public void Init(){
    widthUnitDenominator = DefaultWidthUnitDenominator;
    heightUnitDenominator = DefaultHeightUnitDenominator;

    InitData();
    InitControls();
    PreScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "PreScaleControls");
  }

  public virtual void InitData(){}

  public void UpdateHeightUnits(){
    Rect2 screen = this.GetViewportRect();
    screenWidth =  screen.Size.x;
    screenHeight = screen.Size.y;
    widthUnit = screenWidth / widthUnitDenominator;
    heightUnit = screenHeight / widthUnitDenominator;
  }

  public virtual void InitControls(){}

  public void PreScaleControls(){
    UpdateHeightUnits();
    ScaleControls();
  }

  public virtual void ScaleControls(){}

  public void ChangeSubmenu(string menuName){
    if(submenu != null){
      submenu.QueueFree();
    }

    if(menuName==null){
      submenu = null;
      Input.SetMouseMode(Input.MouseMode.Captured);
      return;
    }
    Input.SetMouseMode(Input.MouseMode.Visible);
    Node menuNode = Menu.MenuFactory(menuName);
    IMenu menu = menuNode as IMenu;
    if(menu == null){
      GD.Print("Menu " + menuName + " was null.");
    }
    else{
      submenu = menuNode;
      AddChild(menuNode);
      menu.Init();
    }
  }

  public void ScaleControl(Control control, float width, float height, float x, float y){
    if(control == null){ return; }
    
    control.SetSize(new Vector2(width, height)); 
    control.SetPosition(new Vector2(x, y));
  }

}