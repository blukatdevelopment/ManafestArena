/*
    Node of a simple dialogue tree used for press events.
*/
using Godot;
using System;
using System.Collections.Generic;

public class PressEventNode {

    int nodeId;
    string prompt;
    string optText1, optText2, optText3, optText4; // Option copy
    int optDest1, optDest2, optDest3, optDest4; // destination node
    string outcome1, outcome2, outcome3, outcome4; // Side effects of option
    const int Blank = -1; // Null destination
    const int End = -2; // This option should end the PressEvent
    const int RowLength = 15; // How many columns are needed to store this.

    public PressEventNode(){}

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
        ret.optText1 = row[2];
        ret.optText2 = row[3];
        ret.optText3 = row[4];
        ret.optText4 = row[5];
        ret.optDest1 = Util.ToInt(row[6]);
        ret.optDest2 = Util.ToInt(row[7]);
        ret.optDest3 = Util.ToInt(row[8]);
        ret.optDest4 = Util.ToInt(row[9]);
        ret.outcome1 = row[10];
        ret.outcome2 = row[11];
        ret.outcome3 = row[12];
        ret.outcome4 = row[13];

        return ret;
    }
}