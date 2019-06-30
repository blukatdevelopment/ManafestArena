/*
  An actor's body consisting of a rigged humanoid model with animations.
*/
using Godot;
using System;
using System.Collections.Generic;


public class HumanoidBody : KinematicBody , IBody, IReceiveDamage {
  Actor actor;
  Spatial eyes, hand;
  Speaker speaker;
  string rootPath;
  MeshInstance meshInstance;
  CollisionShape collisionShape;

  HumanoidAnimationHandler animHandler;
  const float SkeletonUpdateDelay = 0.03f;
  IncrementTimer skeletonTimer;
  Skeleton skeleton;
  public enum BodyParts{
    Head,
    Chest,
    Shoulder_r,
    Shoulder_l,
    Forearm_r,
    Forearm_l,
    Hand_r,
    Hand_l,
    Spine,
    Hips,
    Thigh_r,
    Thigh_l,
    Shin_r,
    Shin_l,
    Foot_r,
    Foot_l
  };
  System.Collections.Generic.Dictionary<BodyParts, BoneAttachment> boneAttachments;
  System.Collections.Generic.Dictionary<BodyParts, CollisionShape> collisionShapes;
  System.Collections.Generic.Dictionary<BodyParts, int> boneIds;
  Vector3 spineRotation;

  public bool dead;
  

  bool grounded, crouched;
  float gravityVelocity = 0f;
  float spinePivot = 0f;

  const float GravityAcceleration = -9.81f;
  const float TerminalVelocity = -53;
  const float SightSize = 100f;

  public HumanoidBody(Actor actor, string rootPath = "res://Assets/Scenes/Actors/debug.tscn"){
    this.actor = actor;
    this.rootPath = rootPath;
    this.dead = false;
    skeletonTimer = new IncrementTimer(SkeletonUpdateDelay);
    InitChildren();
  }
  
  public Actor GetActor(){
    return actor;
  }

  public void AnimationTrigger(string triggerName){
    
  }

  private void InitChildren(){

    eyes = new Spatial();
    AddChild(eyes);

    hand = new Spatial();
    eyes.AddChild(hand);
    hand.Translation = new Vector3(0, 0f, -1f);
    
    speaker = new Speaker();
    AddChild(speaker);

    PackedScene ps = (PackedScene)GD.Load(rootPath);
    Node rootNode = ps.Instance();
    AddChild(rootNode);

    meshInstance = rootNode.FindNode("Cube") as MeshInstance;
    skeleton = rootNode.FindNode("Skeleton") as Skeleton;

    AnimationPlayer armsAnimationPlayer = rootNode.FindNode("ArmsAnimationPlayer") as AnimationPlayer;
    AnimationPlayer legsAnimationPlayer = rootNode.FindNode("LegsAnimationPlayer") as AnimationPlayer;
    animHandler = new HumanoidAnimationHandler(armsAnimationPlayer, legsAnimationPlayer);

    boneAttachments = new System.Collections.Generic.Dictionary<BodyParts, BoneAttachment>();
    collisionShapes = new System.Collections.Generic.Dictionary<BodyParts, CollisionShape>();
    boneIds = new System.Collections.Generic.Dictionary<BodyParts, int>();
    
    CreateBone(BodyParts.Head,        "head",         new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Chest,       "chest",        new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Shoulder_r,  "shoulder.R",   new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Shoulder_l,  "shoulder.L",   new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Forearm_r,   "forearm.R",    new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Forearm_l,   "forearm.L",    new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Hand_r,      "hand.R",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Hand_l,      "hand.L",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Spine,       "spine",        new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Hips,        "hips",         new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Thigh_r,     "thigh.R",      new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Thigh_l,     "thigh.L",      new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Shin_r,      "shin.R",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Shin_l,      "shin.L",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Foot_r,      "foot.R",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone(BodyParts.Foot_l,      "foot.L",       new Vector3(0.1f, 0.1f, 0.1f));
    grounded = true;
    spineRotation = boneAttachments[BodyParts.Spine].GetRotationDegrees();
  }

  private void CreateBone(BodyParts part, string name, Vector3 extents){
    boneIds.Add(part, skeleton.FindBone(name));
    
    CollisionShape hitbox = new CollisionShape();
    BoxShape boxShape = new BoxShape();
    boxShape.Extents = extents;
    hitbox.SetShape(boxShape);
    hitbox.Name = name;
    AddChild(hitbox);
    collisionShapes.Add(part, hitbox);

    BoneAttachment attachment = new BoneAttachment();
    attachment.BoneName = name;
    attachment.Name = name;
    skeleton.AddChild(attachment);

    boneAttachments.Add(part, attachment);
  }

  public void UpdateSkeleton(){
    foreach(BodyParts part in boneAttachments.Keys){
      collisionShapes[part].GlobalTransform = boneAttachments[part].GlobalTransform;
    }
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
    boneAttachments[BodyParts.Head].AddChild(eyes);
    eyes.Rotate(new Vector3(1, 0, 0), Util.ToRadians(90f));
    eyes.Rotate(new Vector3(0, 1, 0), Util.ToRadians(180f));
    eyes.Translate(new Vector3(0, 0.2f, 3f));
    
    eyes.AddChild(hand);
  }

  public void Move(Vector3 movement, float moveDelta = 1f, bool ignoreAnimator = true, bool sprint = false){
      if(!ignoreAnimator && movement != new Vector3()){
        animHandler.HandleMovement();
      }

      movement *= moveDelta;
      
      Transform current = GetTransform();
      Transform destination = current; 
      destination.Translated(movement);
      
      Vector3 delta = destination.origin - current.origin;
      KinematicCollision collision = MoveAndCollide(delta);
      
      if(collision == null){
        return;
      }

      if(collision.Collider != null){
        ICollide collider = collision.Collider as ICollide;
        
        if(collider != null){
          collider.OnCollide(this as object);
        }
      }

      if(grounded){
        return;
      }

      if(collision.Position.y < boneAttachments[BodyParts.Foot_r].GlobalTransform.origin.y || collision.Position.y < boneAttachments[BodyParts.Foot_l].GlobalTransform.origin.y){
        if(gravityVelocity < 0){
          grounded = true;
          gravityVelocity = 0f;
        }
      }
  }

  public void ToggleCrouch(){
    crouched = !crouched;
    animHandler.HandleCrouch(crouched);
  }

  public void Turn(Vector3 movement, float moveDelta = 1f){
    movement *= moveDelta;
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += movement.x;
    SetRotationDegrees(bodyRot);

    spineRotation.x -= movement.y;
    float maxX = 160f;
    float minX = 10f;

    if(spineRotation.x > maxX){
      spineRotation.x = maxX;
    }
    if(spineRotation.x < minX){
      spineRotation.x = minX;
    }

    boneAttachments[BodyParts.Spine].SetRotationDegrees(spineRotation);

    skeleton.SetBoneGlobalPose(boneIds[BodyParts.Spine], boneAttachments[BodyParts.Spine].Transform);

  }

  public void Update(float delta){
    if(!dead){
      Gravity(delta);
      if(skeletonTimer.CheckTimer(delta)){
        UpdateSkeleton();
      }
      animHandler.Update(delta);
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
    Move(grav, delta, true);

    // Kill actor when it falls out of map
    if(actor.stats != null && actor.stats.HasStat("health") && GetTranslation().y < -100){
      Damage damage = new Damage();
      damage.health = actor.stats.GetStat("health");
      actor.stats.ReceiveDamage(damage);
    }
  }

  public void Jump(){
    if(!grounded){ 
      return; 
    }
    float jumpForce = 11;
    
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
}