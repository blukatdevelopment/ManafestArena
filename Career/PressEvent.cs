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

    public PressEvent(List<string[]> rows){
        pressEventNodes = new List<PressEventNode>();
        
        foreach(string[] row in rows){
            pressEventNodes.Add(PressEventNode.FromRow(row));
        }

        currentNode = PressEventNode.GetNode(0, pressEventNodes);
    }
}