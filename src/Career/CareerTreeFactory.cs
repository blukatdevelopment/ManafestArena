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
    List<int> previousLayer = new List<int>();
    int layerCount = Career.CareerLevels;

    for(int i = 0; i < layerCount; i++){
      previousLayer = GenerateLayer(previousLayer, i);
    }

    foreach(CareerNode node in careerNodes){
      node.encounter = Career.RandomEncounter();
    }
  }

  // Create a layer connected to the last one
  public List<int> GenerateLayer(List<int> previous, int layerId){

    List<CareerNode> layer = GenerateLayerNodes(layerId);
    List<int> ret = new List<int>();

    foreach(CareerNode node in layer){
      ret.Add(node.id);
    }

    // When there is no previous layer, return early
    int rootLayer = 0;
    if(layerId == rootLayer){
      return ret;
    }

    // Make sure every child has a parent
    foreach(CareerNode child in layer){
      int choice = Util.RandInt(0, previous.Count);
      CareerNode parent = GetNodeById(previous[choice]);
      parent.children.Add(child.id);
    }

    // Make sure every parent has a child
    foreach(int parentId in previous){
      CareerNode parentNode = GetNodeById(parentId);
      if(parentNode.children.Count == 0){
        int choice = Util.RandInt(0, ret.Count);
        parentNode.children.Add(ret[choice]);
      }
    }

    return ret;
  }

  public List<CareerNode> GenerateLayerNodes(int layerId){
    List<CareerNode> ret = new List<CareerNode>();
    for(int i = 0; i < Util.RandInt(1, Career.MaxCareerNodesPerLevel,true); i++){
      CareerNode node = NewNode();
      node.level = layerId;
      ret.Add(node);
    }
    return ret;
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