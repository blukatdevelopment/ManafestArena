/*
    Represents one encounter on the career map.
*/
using Godot;
using System;
using System.Collections.Generic;

public class CareerNode {
    public int nodeId;
    public int child1, child2, child3; // -1 if null
    public string extraInfo; // Extra data
    public IEncounter encounter;

    public CareerNode(){
        nodeId = -1;
        child1 = -1;
        child2 = -1;
        child3 = -1;
    }

    public bool IsLeaf(){
        if(child1 < 0 && child2 < 0 && child3 < 0){
            return true;
        }
        return false;
    }

    public bool HasChild(int child){
        if(child == -1){
            return false;
        }
        if(child1 == child || child2 == child || child3 == child){
            return true;
        }
        return false;
    }

    public string ToString(){
        string  ret = "CareerNode[" + nodeId + "," + 0 + "][";
        ret += child1 + "," + child2 + "," + child3 + "]";
        return ret;
    }

    public static int GetLevel(CareerNode node, List<CareerNode> nodes){
        if(node == null || nodes == null){
            GD.Print("Invalid args" + node + "," + nodes);
            return -1;
        }

        System.Collections.Generic.Dictionary<int, CareerNode[]> levels = GetLevels(nodes);

        foreach(int key in levels.Keys){
            CareerNode[] level = levels[key];
            for(int i = 0; i < level.Length; i++){
                if(level[i].nodeId == node.nodeId){
                    return key;
                }
            }
        }
        return -1;
    }

    public static System.Collections.Generic.Dictionary<int, CareerNode[]> GetLevels(List<CareerNode> nodes){
        System.Collections.Generic.Dictionary<int, CareerNode[]> ret;
        ret = new System.Collections.Generic.Dictionary<int, CareerNode[]>();

        
        CareerNode root = Root(nodes);
        CareerNode[] rootRow = new CareerNode[1];
        rootRow[0] = root;
        ret.Add(0, rootRow);
        
        if(root == null){
            GD.Print("CareerNode.GetLevels Root node null");
            return null;
        }
        else if(root.IsLeaf()){
            return ret;
        }


        int level = 1;
        List<CareerNode> nextLevel = GetChildren(root, nodes);
        
        while(nextLevel.Count > 0){
            ret.Add(level, nextLevel.ToArray());
            level++;
            nextLevel = GetChildren(nextLevel, nodes);
        }

        return ret;
    }

    public static CareerNode GetNode(int nodeId, List<CareerNode> nodes){
        foreach(CareerNode node in nodes){
            if(node.nodeId == nodeId){
                return node;
            }
        }
        return null;
    }

    public static List<CareerNode> GetChildren(List<CareerNode> previousLevel, List<CareerNode> nodes){
        List<CareerNode> ret = new List<CareerNode>();

        foreach(CareerNode node in previousLevel){
            List<CareerNode> children = GetChildren(node, nodes);
            foreach(CareerNode child in children){
                if(!NodeExists(child.nodeId, ret)){
                    ret.Add(child);
                }
            }
        }

        return ret;
    }

    public static bool NodeExists(int nodeId, List<CareerNode> nodes){
        foreach(CareerNode node in nodes){
            if(node.nodeId == nodeId){
                return true;
            }
        }
        return false;
    }

    public static List<CareerNode> GetChildren(CareerNode node, List<CareerNode> nodes){
        List<CareerNode> ret = new List<CareerNode>();
        foreach(CareerNode candidate in nodes){
            int id = candidate.nodeId;
            if(id == node.child1 || id == node.child2 || id == node.child3){
                ret.Add(candidate);
            }
        }
        return ret;
    }

    public static List<CareerNode> Leaves(List<CareerNode> nodes){
      List<CareerNode> ret = new List<CareerNode>();

      foreach(CareerNode node in nodes){
        if(node.IsLeaf()){
          ret.Add(node);
        }
      }
      
      return ret;
    }

    public static bool IsRoot(CareerNode node, List<CareerNode> nodes){
        if(ParentCount(node, nodes) == 0){
            return true;
        }
        return false;
    }

    public static CareerNode Root(List<CareerNode> nodes){
        foreach(CareerNode node in nodes){
            if(ParentCount(node, nodes) == 0){
                return node;
            }
        }

        return null;
    }

    /*
        As a player traverses their career, they will be presented with
        multiple choices per level according to previous choices.

    */
    public static bool NodeIsActive(CareerNode node, List<CareerNode> nodes, int currentLevel, int lastNode){
        if(node == null || nodes == null){
            GD.Print("NodeIsActive: Invalid args " + node + "," + nodes);
            return false;
        }

        int nodeLevel = GetLevel(node, nodes);

        if(node.IsLeaf() && currentLevel == -1){
            return true;
        }
        if(CareerNode.IsRoot(node, nodes) && currentLevel == 0){
            GD.Print("Node " + node.nodeId + " is root and currentLevel is 0");
            return true;
        }

        if(node.HasChild(lastNode)){
            return true;
        }

        return false;
    }

    public static int ParentCount(CareerNode node, List<CareerNode> nodes){
        int count = 0;
        foreach(CareerNode candidate in nodes){
            if(candidate.child1 == node.nodeId || candidate.child2 == node.nodeId || candidate.child3 == node.nodeId){
                count++;
            }
        }

        return count;
    }

    public static List<CareerNode> GetParents(CareerNode node, List<CareerNode> nodes){
        List<CareerNode> ret = new List<CareerNode>();

        foreach(CareerNode candidate in nodes){
            if(candidate.child1 == node.nodeId || candidate.child2 == node.nodeId || candidate.child3 == node.nodeId){
                ret.Add(candidate);
            }    
        }

        return ret;
    }

    public static List<CareerNode> GetActiveParents(CareerNode node, List<CareerNode> nodes, int currentLevel, int lastNode){
        List<CareerNode> parents = GetParents(node, nodes);
        List<CareerNode> ret = new List<CareerNode>();

        foreach(CareerNode parent in parents){
            if(NodeIsActive(parent, nodes, currentLevel, lastNode)){
                ret.Add(parent);
            }
        }

        return ret;
    }

}