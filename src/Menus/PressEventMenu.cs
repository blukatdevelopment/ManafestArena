using Godot;
using System;
using System.Collections.Generic;

public class PressEventMenu : Container, IMenu {
  public Career career;
  public TextEdit promptText;
  public Button option1Button, option2Button, option3Button, option4Button;
  public PressEvent pressEvent;
  public TextEdit background;

  public void Init(){
    career = Career.GetActiveCareer();
    pressEvent = null;//Session.session.career.pressEvent;
    InitControls();
    ScaleControls();
    GetTree().GetRoot().Connect("size_changed", this, "ScaleControls");
  }

  void InitControls(){
    background = Menu.BackgroundBox();
    AddChild(background);

    PressEventNode node = pressEvent.currentNode;
    promptText = Menu.TextBox(node.prompt);
    AddChild(promptText);

    if(node.optDest[0] != -1){
      option1Button = Menu.Button(node.optText[0], () => {
           SelectOption(0); 
         });
      AddChild(option1Button);
    }
    if(node.optDest[1] != -1){
      option2Button = Menu.Button(node.optText[1], () => {
           SelectOption(1); 
         });
      AddChild(option2Button);
    }
    if(node.optDest[2] != -1){
      option3Button = Menu.Button(node.optText[2], () => {
           SelectOption(2); 
         });
      AddChild(option3Button);
    }
    if(node.optDest[3] != -1){
      option4Button = Menu.Button(node.optText[3], () => {
           SelectOption(3); 
         });
      AddChild(option4Button);
    }
  }

  public void SelectOption(int option){
    //GD.Print("Selected option " + option);
    bool finished = pressEvent.SelectOption(option);
    
    if(!finished){
      TearDownControls();
      InitControls();
      ScaleControls();
      return;
    }

    career.CompleteEncounter();
  }


  public void TearDownControls(){
    if(promptText != null){
      promptText.QueueFree();
      promptText = null;
    }
    if(option1Button != null){
      option1Button.QueueFree();
      option1Button = null;
    }
    if(option2Button != null){
      option2Button.QueueFree();
      option2Button = null;
    }
    if(option3Button != null){
      option3Button.QueueFree();
      option3Button = null;
    }
    if(option4Button != null){
      option4Button.QueueFree();
      option4Button = null;
    }
  }

  void ScaleControls(){
    Rect2 screen = this.GetViewportRect();
    float width = screen.Size.x;
    float height = screen.Size.y;
    float wu = width/10; // relative height and width units
    float hu = height/10;

    Menu.ScaleControl(background, width, height, 0, 0);
    Menu.ScaleControl(promptText, 6 * wu, 4 * hu, 2 * wu, 0);
    if(option1Button != null){
      Menu.ScaleControl(option1Button, 4 * wu, hu, 3 * wu, 4 * hu);
    }
    if(option2Button != null){
      Menu.ScaleControl(option2Button, 4 * wu, hu, 3 * wu, 5 * hu);
    }
    if(option3Button != null){
      Menu.ScaleControl(option3Button, 4 * wu, hu, 3 * wu, 6 * hu);
    }
    if(option4Button != null){
      Menu.ScaleControl(option4Button, 4 * wu, hu, 3 * wu, 7 * hu);
    }
  }

  public void ReturnToMainMenu(){
    Session.ChangeMenu("MainMenu");
  }

}