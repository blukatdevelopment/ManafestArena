/*
  Like Larry the Cucumber.
  A pill-shaped body with a floating hand.
  Crude physics implemented here because actual physics engine runs on a different thread.
*/
using Godot;
using System;
using System.Collections.Generic;


public class PillBody : KinematicBody , IBody {
  Actor actor;
  Spatial eyes, hand;
  Speaker speaker;
  MeshInstance meshInstance;
  CollisionShape collisionShape;
  
  bool grounded;
  float gravityVelocity = 0f;

  const float GravityAcceleration = -9.81f;
  const float TerminalVelocity = -53;

  public PillBody(Actor actor){
    this.actor = actor;
    InitChildren();
  }

  private void InitChildren(){
    eyes = new Spatial();
    AddChild(eyes);
    
    speaker = new Speaker();
    AddChild(speaker);

    string meshPath = "";
    meshInstance = new MeshInstance();
    meshInstance.Mesh = ResourceLoader.Load(meshPath) as Mesh;
    AddChild(meshInstance);

    collisionShape = new CollisionShape();
    AddChild(collisionShape);
    collisionShape.MakeConvexFromBrothers();
  }

  public List<Node> GetHands(){ 
    return new List<Node>(){ hand }; 
  }
  
  public Node GetNode(){ 
    return this as Node; 
  }
  
  public void InitCam(int index){
    if(index != 0){
      return; // Splitscreen cameras not implemented
    }

    RemoveChild(eyes);
    
    Camera cam = new Camera();
    cam.Far = 1000f;
    eyes = (Spatial)cam;
    eyes.TranslateObjectLocal(new Vector3(0, 2f, 0));
    eyes.SetRotationDegrees(new Vector3());

    AddChild(eyes);
  }

  public void Move(Vector3 movement, float moveDelta = 1f){
      movement *= moveDelta;
      
      Transform current = GetTransform();
      Transform destination = current; 
      destination.Translated(movement);
      
      Vector3 delta = destination.origin - current.origin;
      KinematicCollision collision = MoveAndCollide(delta);
      
      if(collision != null && collision.Collider != null){
        ICollide collider = collision.Collider as ICollide;
        
        if(collider != null){
          collider.OnCollide(this as object);
        }
      }
      
      if(!grounded && collision != null && collision.Position.y < GetTranslation().y){
        if(gravityVelocity < 0){
          grounded = true;
          gravityVelocity = 0f;
        }
      }
  }

  public override void _Process(float delta){
    if(actor.stats != null && actor.stats.HasStat("health") && actor.stats.GetStat("health") > 0){
      Gravity(delta);
    }
  }

  private void Gravity(float delta){
    float gravityForce = GravityAcceleration * delta;
    gravityVelocity += gravityForce;

    if(gravityVelocity < TerminalVelocity){
      gravityVelocity = TerminalVelocity;
    }
    
    Vector3 grav = new Vector3(0, gravityVelocity, 0);
    Move(grav, delta);

    // Kill actor when it falls out of map
    if(actor.stats != null && actor.stats.HasStat("health") && GetTranslation().y < -100){
      Damage damage = new Damage();
      damage.health = actor.stats.GetStat("health");
      actor.stats.ReceiveDamage(damage);
    }
  }

  public void Jump(){
    if(!grounded){ return; }
    
    float jumpForce = 10;
    gravityVelocity = jumpForce;
    grounded = false; 
    
    if(actor.stats != null && actor.stats.HasStat("jumpcost")){
      int jumpCost = actor.stats.GetStat("jumpcost");
      Damage dmg = new Damage();
      dmg.stamina = jumpCost;
      actor.stats.ReceiveDamage(dmg);  
    }
  }

  public Speaker GetSpeaker(){
    return speaker;
  }

  public MeshInstance GetMesh(){
    return meshInstance;
  }

  public void Die(){
    Transform = Transform.Rotated(new Vector3(0, 0, 1), 1.5f);
  }
}