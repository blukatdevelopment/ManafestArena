/*
  Represents one encounter on the career map.
*/
using Godot;
using System;
using System.Collections.Generic;

public class CareerNode {
  public int id;
  public int level;
  public List<int> children;
  public IEncounter encounter;

  public CareerNode(){}

  public CareerNode(int id){
    this.id = id;
    children = new List<int>();
  }

  public static CareerNode GetById(List<CareerNode> nodes, int id){
    foreach(CareerNode node in nodes){
      if(node.id == id){
        return node;
      }
    }
    return null;
  }

  public static List<CareerNode> GetByLevel(List<CareerNode> nodes, int level){
    List<CareerNode> levelNodes = new List<CareerNode>();

    foreach(CareerNode node in nodes){
      if(node.level == level){
        levelNodes.Add(node);
      }
    }
    return levelNodes;
  }
}