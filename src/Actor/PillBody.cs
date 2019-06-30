/*
  Like Larry the Cucumber.
  A pill-shaped body with a floating hand.
  Crude physics implemented here because actual physics engine runs on a different thread.
*/
using Godot;
using System;
using System.Collections.Generic;


public class PillBody : KinematicBody , IBody, IReceiveDamage {
  Actor actor;
  Spatial eyes, hand;
  Speaker speaker;
  string meshPath;
  MeshInstance meshInstance;
  CollisionShape collisionShape;

  public bool dead;
  
  bool grounded;
  float gravityVelocity = 0f;
  const int maxY = 90;
  const int minY = -40;

  const float GravityAcceleration = -9.81f;
  const float TerminalVelocity = -53;
  const float SightSize = 100f;

  public PillBody(Actor actor, string meshPath = "res://Assets/Models/Actor.obj"){
    this.actor = actor;
    this.meshPath = meshPath;
    this.dead = false;
    InitChildren();
  }

  public void AnimationTrigger(string triggerName){
    GD.Print("Animation trigger " + triggerName);
  }
  
  public Actor GetActor(){
    return actor;
  }

  private void InitChildren(){

    eyes = new Spatial();
    AddChild(eyes);
    eyes.TranslateObjectLocal(new Vector3(0, 2f, 0));
    eyes.SetRotationDegrees(new Vector3());

    hand = new Spatial();
    eyes.AddChild(hand);
    hand.Translation = new Vector3(0, 0f, -1f);
    
    speaker = new Speaker();
    AddChild(speaker);

    meshInstance = new MeshInstance();
    meshInstance.Mesh = ResourceLoader.Load(meshPath) as Mesh;
    AddChild(meshInstance);

    collisionShape = new CollisionShape();
    AddChild(collisionShape);
    collisionShape.MakeConvexFromBrothers();
    grounded = true;
  }

  public void ReceiveDamage(Damage damage){
    if(dead){
      return;
    }
    
    IReceiveDamage receiver = actor.stats as IReceiveDamage;
    if(receiver == null){
      return;
    }

    receiver.ReceiveDamage(damage);

    if(GetHealth() < 1){
      GD.Print("Died because health was" + GetHealth());
      Die();
    }
  }

  public int GetHealth(){
    IReceiveDamage receiver = actor.stats as IReceiveDamage;
    return receiver == null ? 0 : receiver.GetHealth();
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
    eyes.RemoveChild(hand);

    RemoveChild(eyes);
    
    Camera cam = new Camera();
    cam.Far = 1000f;
    eyes = (Spatial)cam;
    AddChild(eyes);
    eyes.TranslateObjectLocal(new Vector3(0, 2f, 0));
    eyes.SetRotationDegrees(new Vector3());
    
    eyes.AddChild(hand);
  }

  public void Move(Vector3 movement, float moveDelta = 1f, bool ignoreAnimation = true, bool sprint = false){
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

  public void Turn(Vector3 movement, float moveDelta = 1f){
    movement *= moveDelta;
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += movement.x;
    this.SetRotationDegrees(bodyRot);
    
    Vector3 headRot = eyes.GetRotationDegrees();
    headRot.x += movement.y;

    if(headRot.x < minY){
      headRot.x = minY;
    }

    if(headRot.x > maxY){
      headRot.x = maxY;
    }

    eyes.SetRotationDegrees(headRot);
    SetRotationDegrees(bodyRot);
  }

  public void Update(float delta){
    if(!dead){
      Gravity(delta);
    }
  }

  public void HoldItem(int hand, IItem item){
    Node node = item.GetNode();
    this.hand.AddChild(node);
    Spatial spat = node as Spatial;
    if(spat != null){
      spat.Translation = new Vector3(0, 0, -1);
    }
  }

  public void ReleaseItem(int hand, IItem item){
    this.hand.RemoveChild(item.GetNode());
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
    
    if(actor.stats == null){
      gravityVelocity = jumpForce;
      grounded = false;
      return;  
    }

    if(actor.stats != null && actor.stats.HasStat("jumpcost")){
      int jumpCost = actor.stats.GetStat("jumpcost");
      if(actor.stats.ConsumeStat("stamina", jumpCost)){
        gravityVelocity = jumpForce;
        grounded = false;
      }
    }
  }

  public Speaker GetSpeaker(){
    return speaker;
  }

  public MeshInstance GetMesh(){
    return meshInstance;
  }

  public bool IsDead(){
    return dead;
  }

  public void Die(){
    Transform trans = Transform;
    Vector3 pos = trans.origin;
    trans = trans.Rotated(new Vector3(0, 0, 1), 1.5f);
    trans.origin = pos;
    Transform = trans;
    dead = true;
  }

  public List<Actor> ActorsInSight(){
    Vector3 start = eyes.GlobalTransform.origin;
    Vector3 end = Get3DCursor();

    Spatial spat = this as Spatial;
    Vector3 offset = new Vector3(SightSize, SightSize, SightSize);
    Vector3 min = spat.Transform.origin - offset;
    Vector3 max = spat.Transform.origin + offset;
    List<object> objects = Util.SiblingBoxCast(spat, min, max);
    List<Actor> ret = new List<Actor>();
    GD.Print("Found " + objects.Count + " objects via gridcast");
    foreach(object obj in objects){
      Node node = obj as Node;
      Actor sightedActor = Actor.GetActorFromNode(node);
      if(sightedActor != null && sightedActor != actor){
        ret.Add(sightedActor);
      }
    }
    return ret;
  }

  // The end of a ray pointed forward in global space
  public Vector3 Get3DCursor(float distance = 100f){
    Vector3 start = eyes.GlobalTransform.origin;
    Transform headTrans = eyes.Transform;
    Vector3 end = Util.TForward(headTrans);
    end *= distance;
    end += start;
    return end;
  }

  public Vector3 LookingDegrees(){
    Vector3 torsoRot = GetRotationDegrees();
    Vector3 eyesRot = eyes.GetRotationDegrees();
    torsoRot.x = eyesRot.x;
    return torsoRot;
  }

  public void ToggleCrouch(){
    // Nothing to see here, folks.
  }
}