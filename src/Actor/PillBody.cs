/*
  Like Larry the Cucumber.
  A tube body with a floating hand.
*/
using Godot;
using System;
using System.Collections.Generic;


public class PillBody : RigidBody, IBody {
  public List<Node> GetHands(){ return null; }
  public Node GetNode(){ return null; }
  public void InitCam(int index){}
  public void Move(Vector3 movement, float moveDelta){}
  public void Jump(){}
}