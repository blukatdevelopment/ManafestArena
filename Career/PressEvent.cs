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