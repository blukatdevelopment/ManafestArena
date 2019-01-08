/*
    Handles info for exactly one encounter
*/
using Godot;
using System;
using System.Collections.Generic;

public class CareerNode {
    public int nodeId;
    public int child1, child2, child3; // -1 if null
    public enum NodeTypes{
        ArenaMatch,
        BossMatch,
        FinalBoss,
        Shop,
        RestSite,
        PressEvent
    };
    public NodeTypes nodetype;

    public void Execute(){
        switch(Nodetype){
            case NodeTypes.ArenaMatch:
                ExecuteArenaMatch();
                break;
            case NodeTypes.BossMatch:
                ExecuteBossMatch();
                break;
            case NodeTypes.FinalBoss:
                ExecuteFinalBoss();
                break;
            case NodeTypes.Shop:
                ExecuteShop();
                break;
            case NodeTypes.RestSite:
                ExecuterestSite();
                break;
            case NodeTypes.PressEvent:
                ExecutePressEvent();
                break;
        }
    }

    public void ExecuteArenaMatch(){}
    public void ExecuteBossMatch(){}
    public void ExecuteFinalBoss(){}
    public void ExecuteShop(){}
    public void ExecuterestSite(){}
    public void ExecutePressEvent(){}
}