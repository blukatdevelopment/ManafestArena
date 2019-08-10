/*
  This builds a career tree using instance variables to simplify what otherwise
  might be a set of static methods in Career.cs

*/
using Godot;
using System;
using System.Collections.Generic;

public class CareerTreeFactory {
  public List<CareerNode> careerNodes;
  int nextNodeId;

  public CareerTreeFactory(){
    careerNodes = new List<CareerNode>();
    nextNodeId = 0;
  }

  public CareerNode NewNode(){
    CareerNode node = new CareerNode(nextNodeId);
    nextNodeId++;
    careerNodes.Add(node);
    return node;
  }

  public CareerNode GetNodeById(int id){
    return CareerNode.GetById(careerNodes, id);
  }

  public void Generate(){
    CareerNode node = NewNode();
    node.encounter = Career.RandomEncounter();
    node.level = 0;
    for(int i = 1; i < 4; i++){
      CareerNode child = NewNode();
      child.level = i;
      child.encounter = Career.RandomEncounter();

      node.children.Add(child.id);
      node = child;
    }

    return; 
  }

  public static Career Factory(string championName){
    Career ret = new Career();
    ret.player = ActorFactory.FromName(championName);
    
    CareerTreeFactory factory = new CareerTreeFactory();
    factory.Generate();
    ret.careerNodes = factory.careerNodes;
    ret.lastNode = -1;

    return ret;
  }

}