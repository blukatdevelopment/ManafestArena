using Godot;
using System;
using System.Collections.Generic;

public class WolfBody : KinematicBody , IBody, IReceiveDamage
{
    
  Actor actor;
  Spatial eyes, mouth;
  Speaker speaker;
  string rootPath;
  MeshInstance meshInstance;
  CollisionShape collisionShape;
  Skeleton skeleton;
  AnimationTree animationTree;
  AnimationNodeStateMachinePlayback stateMachine;
  public bool dead;
  const float ChangeAnimDelay = 0.05f;
  IncrementTimer ChangeAnimTimer;
  
  bool grounded, crouched;
  Vector3 gravityVelocity = new Vector3();
  const int maxY = 90;
  const int minY = -40;

  Vector3 GravityAcceleration = new Vector3(0,-9.81f,0) ;
  const float TerminalVelocity = -53;
  const float SightSize = 100f;

  public WolfBody(Actor actor, string rootPath = "res://Assets/Scenes/Actors/wolf_body.tscn"){
    this.actor = actor;
    this.rootPath = rootPath;
    this.dead = false;
    ChangeAnimTimer = new IncrementTimer(ChangeAnimDelay);
    InitChildren();
  }

  public void AnimationTrigger(string triggerName){
    GD.Print("Animation trigger " + triggerName);
    triggerName = triggerName.ToLower();
    if(triggerName=="bite"){
      stateMachine.Travel("bite");
    }
  }

  public Actor GetActor(){
    return actor;
  }

  private void InitChildren(){
    
    PackedScene ps = (PackedScene)GD.Load(rootPath);
    Node rootNode = ps.Instance();
    AddChild(rootNode);
    
    speaker = new Speaker();
    AddChild(speaker);

    CapsuleShape shape = new CapsuleShape();
    shape.SetRadius(.4f);
    collisionShape =  new CollisionShape();
    collisionShape.SetShape(shape);
    collisionShape.TranslateObjectLocal(new Vector3(0,0.5f,0));
    AddChild(collisionShape);

    BoxShape shape2 = new BoxShape();
    shape2.SetExtents(new Vector3(0.15f,0.2f,0.25f));
    collisionShape =  new CollisionShape();
    collisionShape.SetShape(shape2);
    collisionShape.TranslateObjectLocal(new Vector3(0,1.4f,-1));
    AddChild(collisionShape);

    eyes = rootNode.FindNode("Eyes") as Spatial;
    //eyes = rootNode.FindNode("Test") as Spatial; //for testing
    meshInstance = rootNode.FindNode("Body") as MeshInstance;
    skeleton = rootNode.FindNode("Skeleton") as Skeleton;
    mouth = rootNode.FindNode("Mouth") as Spatial;
    animationTree = rootNode.FindNode("AnimationTree") as AnimationTree;
    grounded = true;
    initAnimTree();
  }

  public void initAnimTree(){
    animationTree.SetActive(true);
    stateMachine = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
    setBlendPosition(0);
  }

  public void setBlendPosition(float blendPosition =0){
    animationTree.Set("parameters/walk/blend_position",blendPosition);
    animationTree.Set("parameters/run/blend_position",blendPosition);
    animationTree.Set("parameters/crouch/blend_position",blendPosition*0.7f);//the 0.7 is temporary
    if(blendPosition!=0){
      ChangeAnimTimer.StartTimer();
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

    GD.Print("Receided Damage " + Util.ToJson(damage));
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
    return new List<Node>(){}; //No hands for wolf (for now)
  }
  
  public Node GetNode(){ 
    return this as Node; 
  }
  
  public void InitCam(int index){
    if(index != 0){
      return; // Splitscreen cameras not implemented
    }
    
    Camera cam = new Camera();
    cam.Far = 1000f;
    eyes.AddChild(cam);
  }
  public void Move(Vector3 movement, float moveDelta = 1f, bool ignoreAnimator = true, bool sprint = false){

      if(!ignoreAnimator&&grounded){
        if(movement.z!=0){ 
           if(movement.z<0){
            setBlendPosition(1);
          }
          else{
            setBlendPosition(-1);
          }
          ChangeAnimTimer.StartTimer();
        }
      }

      movement.x*=0.25f;//wolves dont strafe
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

  }

  public void ToggleCrouch(){
    crouched = !crouched;
    if(crouched){
      stateMachine.Travel("crouch");
    }
    else{
      stateMachine.Travel("walk");
    }
  }

  public void Turn(Vector3 movement, float moveDelta = 1f){
    movement *= moveDelta;
    Vector3 bodyRot = this.GetRotationDegrees();
    if(grounded){
      bodyRot.y += movement.x;
    }
    else
    {
      bodyRot.y +=movement.x*0.25f;
    }
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

  }

  public void Update(float delta){
    if(!dead){
      Gravity(delta);
      if(ChangeAnimTimer.CheckTimer(delta))
        setBlendPosition(0);
      float margin = 0.05f;
      if(TestMove(Transform,margin*Vector3.Down)){
        if(gravityVelocity.y < 0){
          if(!grounded){
            grounded = true;
            gravityVelocity = new Vector3();
          }
          if(grounded &&stateMachine.GetCurrentNode()=="jump"){
            stateMachine.Travel("walk");
          }
        }
      }
    }
  }

  public void HoldItem(int hand, IItem item){
    Node node = item.GetNode();
    mouth.AddChild(node);
  }

  public void ReleaseItem(int hand, IItem item){
    if(mouth.IsAParentOf(item.GetNode())){
      mouth.RemoveChild(item.GetNode());
    }
  }

  private void Gravity(float delta){
    Vector3 gravityForce = GravityAcceleration * delta;
    gravityVelocity += gravityForce;

    if(gravityVelocity.Length() < TerminalVelocity){
      gravityVelocity = TerminalVelocity*gravityVelocity.Normalized();
    }
    
    Move(gravityVelocity, delta, true);

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
    crouched=false;
    Vector3 jumpForce = new Vector3(0,11,-5);
    
    if(actor.stats == null){
      gravityVelocity = jumpForce;
      grounded = false;
      stateMachine.Travel("jump");
      return;  
    }

    if(actor.stats != null && actor.stats.HasStat("jumpcost")){
      int jumpCost = actor.stats.GetStat("jumpcost");
      if(actor.stats.ConsumeStat("stamina", jumpCost)){
        gravityVelocity = jumpForce;
        grounded = false;
        stateMachine.Travel("jump");
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
    //play dead anim
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
