/*
    Node of a simple dialogue tree used for press events.
*/
using Godot;
using System;
using System.Collections.Generic;

public class PressEventNode {

    public int nodeId;
    public string prompt;

    // Options data
    public string[] optText; // Option copy
    public int[] optDest; // option destinations
    public string[] sideEffects; // Side effects of option
    
    const int Blank = -1; // Null destination
    const int End = -2; // This option should end the PressEvent
    const int RowLength = 14; // How many columns are needed to store a node

    public PressEventNode(){
      optText = new string[4];
      optDest = new int[4];
      sideEffects = new string[4];
    }

    new public string ToString(){
      string ret = "";

      ret += "Node[" + nodeId + "]: " + prompt + "\n";
      for(int i = 0; i < 4; i++){
        ret += "\t[" + i + "]: \"" + optText[i] + "\", ";
        ret += optDest[i] + "," + sideEffects[i] + "\n";
      }
      return ret;
    }

    public static PressEventNode GetNode(int nodeId, List<PressEventNode> nodes){
        foreach(PressEventNode node in nodes){
            if(nodeId == node.nodeId){
                return node;
            }
        }
        return null;
    }

    public static PressEventNode FromRow(string[] row){
        if(row.Length < RowLength){
            GD.Print("PressEventNode.FromRow: Needed " + RowLength + " columns and got " + row.Length);
            return null;
        }

        PressEventNode ret = new PressEventNode();
        ret.nodeId = Util.ToInt(row[0]);
        ret.prompt = row[1];
        ret.optText[0] = row[2];
        ret.optText[1] = row[3];
        ret.optText[2] = row[4];
        ret.optText[3] = row[5];
        ret.optDest[0] = Util.ToInt(row[6]);
        ret.optDest[1] = Util.ToInt(row[7]);
        ret.optDest[2] = Util.ToInt(row[8]);
        ret.optDest[3] = Util.ToInt(row[9]);
        ret.sideEffects[0] = row[10];
        ret.sideEffects[1] = row[11];
        ret.sideEffects[2] = row[12];
        ret.sideEffects[3] = row[13];

        return ret;
    }
}