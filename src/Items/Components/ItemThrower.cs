using Godot;
using System;
using System.Collections.Generic;

public class ItemThrower {
    IItem item;

    public const float DefaultImpulseStrength = 25f;
    public bool thrown;
    public float thrownImpulseStrength;
    public object pastWielder;
    public bool damageActive;
    public Damage damage;

    public ItemThrower(IItem item){
        this.item = item;
    }

    public void Config(
        float thrownImpulseStrength,
        Damage damage
    ){
        this.thrownImpulseStrength = thrownImpulseStrength;
        this.damage = damage;

    }

    public void Throw(){
        if(thrown){
            return;
        }
        thrown = true;

        object wielder = item.GetWielder();
        Actor actor = wielder as Actor;
        pastWielder = wielder;

        if (actor != null) {
            item.SetCollision(true);
            wielder = null;
        }

        Spatial spat = item.GetNode() as Spatial;
        Transform start = spat.GetGlobalTransform();
        Transform destination = start;
        destination.Translated(new Vector3(0, 0, 1));
        
        Vector3 impulse = start.origin - destination.origin;

        RigidBody rigidBody = item.GetNode() as RigidBody;
        if(rigidBody != null){
            rigidBody.SetAxisVelocity(impulse * thrownImpulseStrength);
        }
    }

    public void OnUpdateWielder(){
        object wielder = item.GetWielder();
        if(wielder == null){
            damage.sender = "";
            return;
        }
        Node wielderNode = wielder as Node;
        if(wielderNode != null){
            damage.sender = wielderNode.GetPath();
        }
    }

    private Node GetParent(){
        return item.GetNode().GetParent();
    }
}