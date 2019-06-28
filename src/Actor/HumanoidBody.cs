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

  const float SkeletonUpdateDelay = 0.03f;
  IncrementTimer skeletonTimer;
  Skeleton skeleton;
  int[] bones;
  CollisionShape[] hitBoxes;
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

  public bool dead;
  
  bool grounded;
  float gravityVelocity = 0f;
  const int minY = 90;
  const int maxY = -40;
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
    eyes.TranslateObjectLocal(new Vector3(0, 2f, 0));
    eyes.SetRotationDegrees(new Vector3());

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

    AnimationPlayer animationPlayer = rootNode.FindNode("AnimationPlayer") as AnimationPlayer;
    animationPlayer.Play("Both_Rest");

    bones = new int[16];
    hitBoxes = new CollisionShape[16];

    CreateBone((int)BodyParts.Head,        "head",         new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Chest,       "chest",        new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Shoulder_r,  "shoulder.R",   new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Shoulder_l,  "shoulder.L",   new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Forearm_r,   "forearm.R",    new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Forearm_l,   "forearm.L",    new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Hand_r,      "hand.R",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Hand_l,      "hand.L",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Spine,       "spine",        new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Hips,        "hips",         new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Thigh_r,     "thigh.R",      new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Thigh_l,     "thigh.L",      new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Shin_r,      "shin.R",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Shin_l,      "shin.L",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Foot_r,      "foot.R",       new Vector3(0.1f, 0.1f, 0.1f));
    CreateBone((int)BodyParts.Foot_l,      "foot.L",       new Vector3(0.1f, 0.1f, 0.1f));
    grounded = true;
  }

  private void CreateBone(int index, string name, Vector3 extents){
    bones[index] = skeleton.FindBone(name);
    hitBoxes[index] = new CollisionShape();
    BoxShape boxShape = new BoxShape();
    boxShape.Extents = extents;
    hitBoxes[index].SetShape(boxShape);
    hitBoxes[index].Name = name;
    AddChild(hitBoxes[index]);
  }

  public void UpdateSkeleton(){
    for(int i = 0; i < bones.Length; i++){
      Transform hbTrans = hitBoxes[i].GlobalTransform;
      Transform skelTrans = skeleton.GetBoneGlobalPose(bones[i]);
      hbTrans.origin = skeleton.ToGlobal(skelTrans.origin);
      hitBoxes[i].GlobalTransform = hbTrans;
    }

    Transform headTrans = skeleton.GetBoneGlobalPose(bones[(int)BodyParts.Head]);
    headTrans.origin += new Vector3(0, 0, -5);
    Transform eyesTrans = eyes.GlobalTransform;
    eyesTrans.origin = skeleton.ToGlobal(headTrans.origin);
    //eyesTrans.basis = headTrans.basis;
    eyes.GlobalTransform = eyesTrans;

    // Vector3 eyesRot = eyes.GetRotationDegrees();
    // eyesRot.x += 90f;
    // eyesRot.y = 180;

    // eyes.SetRotationDegrees(eyesRot);
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

      if(grounded || collision == null){
        return;
      }

      if(collision.Position.y < hitBoxes[(int)BodyParts.Foot_r].GlobalTransform.origin.y || collision.Position.y < hitBoxes[(int)BodyParts.Foot_l].GlobalTransform.origin.y){
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

    Spatial spineSpat = new Spatial();
    spineSpat.GlobalTransform = skeleton.GetBoneGlobalPose(bones[(int)BodyParts.Spine]);
    //hitBoxes[(int)BodyParts.Spine];
    Vector3 headRot = spineSpat.GetRotationDegrees();

    headRot.x += movement.y;
    GD.Print("headRot after" + headRot);
    

    spineSpat.SetRotationDegrees(headRot);
    skeleton.SetBoneGlobalPose(bones[(int)BodyParts.Spine], spineSpat.Transform);
    
    SetRotationDegrees(bodyRot);
  }

  public void Update(float delta){
    if(!dead){
      Gravity(delta);
      if(skeletonTimer.CheckTimer(delta)){
        UpdateSkeleton();
      }
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