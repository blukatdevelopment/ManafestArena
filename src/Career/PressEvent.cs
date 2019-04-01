/*
    A PressEvent is a random encounter in the career that offers the player 
    a dialogue tree to traverse, usually representing some in-game decision for them 
    to make that has consequences on their career.
*/
using Godot;
using System;
using System.Collections.Generic;

public class PressEvent {
    public List<PressEventNode> pressEventNodes;
    public PressEventNode currentNode;

    public PressEvent(){
        pressEventNodes = new List<PressEventNode>();
    }

    // Returns true when tree is done.
    public bool SelectOption(int option){
      if(currentNode == null){
        GD.Print("PressEvent.SelectOption: No currentNode loaded.");
        return false;
      }
      int destinationId = currentNode.optDest[option];
      PressEventNode destinationNode = PressEventNode.GetNode(destinationId, pressEventNodes);

      if(destinationId == -2){
        GD.Print("PressEvent.SelectOption: End of dialogue tree.");
        return true;
      }
      else if (destinationId == -1){
        GD.Print("PressEvent.SelectOption: Option " + option + "'s dest is null. How did you even select it?");
      }
      else if(destinationNode == null){
        GD.Print("PressEvent.SelectOption: Option " + option + "'s destination " + destinationId + " is invalid.");
      }
      else{
        currentNode = destinationNode;
      }

      return false;
    }

    public string ToString(){
      string ret = "";
      
      foreach(PressEventNode node in pressEventNodes){
        if(node != null){
          ret += node.ToString() + "\n";
        }
        else{
          GD.Print("Node is null");
        }
      }

      return ret;
    }

    public PressEvent(System.Collections.Generic.Dictionary<int, string[]> rows){
        pressEventNodes = new List<PressEventNode>();
        
        foreach(int key in rows.Keys){
          PressEventNode node = PressEventNode.FromRow(rows[key]);
          if(node != null){
            pressEventNodes.Add(node);
          }
          
        }

        currentNode = PressEventNode.GetNode(0, pressEventNodes);
    }

}