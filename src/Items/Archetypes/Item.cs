/*
    Item provides a virtual implementation of IItem and some common 
    item functionality.

    This inheritance tree should preferrably kept shallow where possible
    to avoid branches that wind up converging. Item components should
    provide functionality wherever possible.

    If your item looks dramatically different from this class, then don't
    inherit and implement IItem in it.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Item: RigidBody, IItem, IHasInfo {
    public object wielder;
    public int id;
    public Speaker speaker;
    public string name, description;
    public string meshPath; // Set this before InitNodeStructure
    public ItemFactory.Items itemEnum = ItemFactory.Items.None; // Set this in factory
    public MeshInstance meshInstance;
    public CollisionShape collisionShape;
    public bool collisionDisabled;
    public Area area;

    public enum ItemInputs{
        A,     // eg. Primary        Left Mouse
        B,     // eg. Secondary      Right Mouse
        C,     // eg. Reload         R
        D,     // eg. Melee          F
        E,     // eg. Middle         Middle Mouse button
        F,     // eg. Next           Mouse Wheel forward
        G      // eg. previous       Mouse Wheel Backward
    };

    // Should take place after Item has been configured.
    public virtual void InitNodeStructure(){
        this.Connect("body_entered", this, nameof(OnCollide));

        speaker = new Speaker();
        AddChild(speaker);

        if(meshPath != ""){
          meshInstance = new MeshInstance();
          meshInstance.Mesh = ResourceLoader.Load(meshPath) as Mesh;
          AddChild(meshInstance);

          collisionShape = new CollisionShape();
          AddChild(collisionShape);
          collisionShape.MakeConvexFromBrothers();
        }
    }

    public virtual string GetInfo(){
        return name;
    }

    public virtual string GetMoreInfo(){
        return description;
    }

    public virtual void Use(MappedInputEvent inputEvent){}

    public virtual void OnCollide(object body){}

    public virtual int GetId(){
        return id;
    }

    public virtual object GetWielder(){
        return wielder;
    }

    public virtual Node GetNode(){
        return this;
    }

    public virtual void Update(float delta){
    }

    public virtual void SetCollision(bool val){
        Godot.Array owners = GetShapeOwners();
        collisionDisabled = !val;

        if(area == null){
          InitArea();
        }

        foreach(object owner in area.GetShapeOwners()){
          int ownerInt = (int)owner;
          CollisionShape cs = (CollisionShape)area.ShapeOwnerGetOwner(ownerInt);
          if(cs != null){
            cs.Disabled = !val;
          }
        }

        foreach(object owner in owners){
          int ownerInt = (int)owner;
          CollisionShape cs = (CollisionShape)ShapeOwnerGetOwner(ownerInt);
          if(cs != null){
            cs.Disabled = !val;
          }
        }
        
        ContactMonitor = val;
        
        if(val){
          ContactsReported = 10;
        }
        else{
          ContactsReported = 0;
        }
    }

    public virtual void SetPhysics(bool val){
        Mode = val ? RigidBody.ModeEnum.Rigid : RigidBody.ModeEnum.Static;
    }

    public  void InitArea(){
        if(area != null){
          return;
        }
        List<CollisionShape> shapes = GetCollisionShapes();
        this.area = new Area();
        CollisionShape areaShape = new CollisionShape();
        area.AddChild(areaShape);
        Godot.Array areaShapeOwners = area.GetShapeOwners();
        for(int i = 0; i < areaShapeOwners.Count; i++){
          int ownerInt = (int)areaShapeOwners[i];
          for(int j = 0; j < shapes.Count; j++){
            area.ShapeOwnerAddShape(ownerInt, shapes[i].Shape);
          }
        }
        area.Connect("body_entered", this, nameof(OnCollide));
        AddChild(area);
    }

    public List<CollisionShape> GetCollisionShapes(){
        List<CollisionShape> shapes = new List<CollisionShape>();
        Godot.Array owners = GetShapeOwners();
        foreach(object owner in owners){
          int ownerInt = (int)owner;
          CollisionShape cs = (CollisionShape)ShapeOwnerGetOwner(ownerInt);
          if(cs != null){
            shapes.Add(cs);
          }
        }
        return shapes;
    }

    public virtual void Equip(object wielder){
        this.wielder = wielder;
        SetCollision(false);

        Node node = wielder as Node;
        if(node != null){
            node.AddChild(this);
        }
    }

    public virtual void Unequip(){
        Node node = wielder as Node;
        if(node != null){
            node.RemoveChild(this);
        }

        this.wielder = null;
        SetCollision(true);
    }

    public virtual ItemFactory.Items GetItemEnum(){
        return itemEnum;
    }

    public virtual void LoadJson(string json){}
    
    public virtual string GetJson(){
        return "";
    }

    public virtual List<ItemFactory.Items> GetSupportedItems(){
        return new List<ItemFactory.Items>();
    }

    public virtual IItem Factory(ItemFactory.Items item){
        return null;
    }
    
    public void SetId(int id){
        this.id = id;
    }
}