using Godot;
using System;
using System.Collections.Generic;

public class BatBody : KinematicBody , IBody, IReceiveDamage {
    
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
  bool grounded = false, diving = false, rising = false;
  Vector3 velocity = new Vector3();
  const int maxY = 90;
  const int minY = -40;

  const float StartingHeight = 4;
  float height = StartingHeight;

  const float GravityForceY = -9.81f;
  Vector3 GravityAcceleration = new Vector3(0,GravityForceY,0) ;
  const float TerminalVelocity = 53;
  const float SightSize = 100f;

  public BatBody(Actor actor, string rootPath = "res://Assets/Scenes/Actors/bat_body.tscn"){
    this.actor = actor;
    this.rootPath = rootPath;
    this.dead = false;
    InitChildren();
  }

  public void AnimationTrigger(string triggerName){
    // FIX ME: Implement animations.
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
    shape.SetRadius(.06f);
    shape.SetHeight(.09f);
    collisionShape =  new CollisionShape();
    collisionShape.SetShape(shape);
    collisionShape.TranslateObjectLocal(new Vector3(0,0,0.03f));
    AddChild(collisionShape);

    eyes = rootNode.FindNode("Eyes") as Spatial;

    meshInstance = rootNode.FindNode("Body") as MeshInstance;
    skeleton = rootNode.FindNode("Skeleton") as Skeleton;
    mouth = rootNode.FindNode("Mouth") as Spatial;
    animationTree = rootNode.FindNode("AnimationTree") as AnimationTree;
    initAnimTree();
  }

  public void initAnimTree(){
    animationTree.SetActive(true);
    stateMachine = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
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
      
      string dead = this.GetPath();
      string killer = damage.sender;
      SessionEvent evt = SessionEvent.ActorDiedEvent(dead, killer);
      Session.session.HandleEvent(evt);

      Die();
    }
  }

  public int GetHealth(){
    IReceiveDamage receiver = actor.stats as IReceiveDamage;
    return receiver == null ? 0 : receiver.GetHealth();
  }

  public List<Node> GetHands(){ 
    return new List<Node>(){};
  }
  
  public Node GetNode(){ 
    return this as Node; 
  }
  
  public void InitCam(int index){}

  public void Move(Vector3 movement, float moveDelta = 1f, bool ignoreAnimator = true, bool sprint = false){
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

  public void Turn(Vector3 movement, float moveDelta = 1f){
    movement *= moveDelta;
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += movement.x;

    bodyRot.x += movement.y;

    if(bodyRot.x < minY){
      bodyRot.x = minY;
    }

    if(bodyRot.x > maxY){
      bodyRot.x = maxY;
    }

    this.SetRotationDegrees(bodyRot);
  }

  public void Update(float delta){
    float margin = 0.1f;

    if(dead){
      if(grounded){
        return;
      }
    }
    else{
      Fly(delta);
      return;
    }

    ApplyGravity(delta);
    if(TestMove(Transform,margin*Vector3.Down)){
        grounded = true;
        velocity = new Vector3();
    }
  }

  public void Fly(float delta){
    float margin = 0.2f;
    float riseSpeed = 3;

    if(GetTranslation().y - height > margin){
      Move(Vector3.Down * riseSpeed, delta, true);
    }
    else if(GetTranslation().y - height <- margin){
      Move(Vector3.Up*riseSpeed, delta, true);
    }
  }

  private void DieWhenOutOfBounds(){
    float minimumYBounds = -100;
    
    if(actor.stats != null && GetTranslation().y < minimumYBounds){
      Damage damage = new Damage();
      damage.health = actor.stats.Health;
      actor.stats.ReceiveDamage(damage);
    }

  }

  private void ApplyGravity(float delta){
    Vector3 gravityForce = GravityAcceleration * delta;
    velocity += gravityForce;

    if(velocity.Length() > TerminalVelocity){
      velocity = TerminalVelocity*velocity.Normalized();
    }
    
    Move(velocity, delta, true);

    DieWhenOutOfBounds();
  }

  public void ToggleCrouch(){}
  public void Jump(){}

  public void HoldItem(int hand, IItem item){
    Node node = item.GetNode();
    mouth.AddChild(node);
  }

  public void ReleaseItem(int hand, IItem item){
    if(mouth.IsAParentOf(item.GetNode())){
      mouth.RemoveChild(item.GetNode());
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
    //FIX ME: play dead anim
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

    return torsoRot;
  }

}
