/*
  An interface for bodies used by an actor.
*/
using Godot;
using System;
using System.Collections.Generic;

public interface IBody {
  Actor GetActor();
  List<Node> GetHands();
  Node GetNode();
  void InitCam(int index);
  void Move(Vector3 movement, float moveDelta);
  void Jump();
  Speaker GetSpeaker();
  MeshInstance GetMesh();
  void Die();
}