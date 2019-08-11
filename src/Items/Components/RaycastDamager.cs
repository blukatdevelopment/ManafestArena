/*
    Performs a raycast and issues damage accordingly.
*/
using Godot;
using System;
using System.Collections.Generic;

public class RaycastDamager {
  IItem item;
  float distance;
  Speaker speaker;
  Damage damage;

  public RaycastDamager(IItem item){
    this.item = item;
  }

  public void Config(float distance, Damage damage, Speaker speaker){
    this.damage = damage;
    this.speaker = speaker;
    this.distance = distance;
  }

  public void Fire(){
    speaker.PlayEffect(Sound.Effects.FistSwing);

    Vector3 org = OriginPoint();
    Vector3 dest = DestinationPoint();
    GD.Print("org " + org);
    GD.Print("dest " + dest);
    object obj = Util.RayCast(org, dest, GetWorld());
    GD.Print("obj" + obj);
    IReceiveDamage recipient = obj as IReceiveDamage;
    GD.Print("Recipient " + recipient);
    if(recipient != null){
      recipient.ReceiveDamage(damage);
    }
  }

  public Vector3 OriginPoint(){
    Vector3 ret = GlobalTransform().origin;
    return ret;
  }

  public Vector3 DestinationPoint(){
    Vector3 ret = new Vector3();
    Transform trans = GlobalTransform();
    ret = Util.TForward(trans);
    ret *= distance;
    ret += trans.origin;
    return ret;
  }

  public Transform GlobalTransform(){
    Spatial spatial = item.GetNode() as Spatial;
    // You better use the constructor correctly
    return spatial.GlobalTransform;
  }
  public World GetWorld(){
    Spatial spatial = item.GetNode() as Spatial;
    if(spatial != null){
      return spatial.GetWorld();
    }
    return null; 
  }

}