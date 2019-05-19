using Godot;
using System;
using System.Collections.Generic;

public class ProjectileLauncher {
  IItem item;
  Damage damage;
  Speaker speaker;

  public ItemFactory.Items projectileType;
  public float launchImpulse;

  public ProjectileLauncher(IItem item){
    this.item = item;
  }

  public void Config(
    ItemFactory.Items projectileType,
    Damage damage,
    float launchImpulse,
    Speaker speaker
  ){
    this.projectileType = projectileType;
    this.damage = damage;
    this.launchImpulse = launchImpulse;
    this.speaker = speaker;
  }

  public void Fire(){
    speaker.PlayEffect(Sound.Effects.RifleShot);
    IItem projectile = ItemFactory.Factory(projectileType);
    object wielder = item.GetWielder();

    Node wielderNode = wielder as Node;


    ProjectileItem proj = ItemFactory.Factory(projectileType) as ProjectileItem;

    if(wielderNode != null){
      proj.sender = wielderNode.GetPath();
    }
    
    Spatial spat = item.GetNode() as Spatial;
    Spatial projSpat = projectile.GetNode() as Spatial;

    Vector3 projectilePosition = new Vector3(0, 0, -1);
    Vector3 globalPosition = spat.ToGlobal(projectilePosition);

    Spatial gameNode = Session.GameSpatial();
    

    if(gameNode == null){
        GD.Print("No gamenode to work with here.");
        return;
    }
    else{
        gameNode.AddChild(projectile.GetNode());
        Vector3 gamePosition = gameNode.ToLocal(globalPosition);
        projSpat.Translation = gamePosition;    
    }

    Transform start = spat.GetGlobalTransform();
    Transform destination = start;
    destination.Translated(new Vector3(0, 0, 1));

    Vector3 impulse = start.origin - destination.origin;
    RigidBody projRb = projectile.GetNode() as RigidBody;
    projRb.SetAxisVelocity(impulse * launchImpulse);
  }
}