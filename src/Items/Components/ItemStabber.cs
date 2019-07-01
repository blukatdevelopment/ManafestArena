/*
    Triggers stab animation and administers damage to first
    collision that isn't the wielder.
*/
using Godot;
using System;
using System.Collections.Generic;

public class ItemStabber {
    IItem item;
    public Vector3 wieldedPosition, forwardPosition;
    public bool stabbing;
    public float stabSpeed, stabTimer;
    public Damage damage;
    public Speaker speaker;
    public Sound.Effects stabSound, impactSound;

    public ItemStabber(IItem item){
        this.item = item;
        this.stabSound = Sound.Effects.FistSwing;
        this.stabbing = false;
    }

    public void Config(
        float stabSpeed, // How long a stab takes to complete
        Damage damage, // Damage object passed to stabbed target
        Speaker speaker, // Use this to make noise
        Sound.Effects stabSound,
        Sound.Effects impactSound
    ){
        this.stabSpeed  = stabSpeed;
        this.damage     = damage;
        this.speaker    = speaker;
        this.stabSound = stabSound;
        this.impactSound = impactSound;
        this.forwardPosition = new Vector3();
        this.wieldedPosition = new Vector3();
    }

    public void Update(float delta){
        this.item = item;
        if(stabbing){
            stabTimer -= delta;
            if(stabTimer <= 0f){
                EndStab();
            }
        }
    }

    public void OnUpdateWielder(){

        EndStab();
        UpdateSender();
        UpdatePositions();
        
    }

    private void UpdateSender(){
        object wielder = item.GetWielder();
        Node wielderNode = wielder as Node;

        if(wielder == null || wielderNode == null || !wielderNode.IsInsideTree()){
            damage.sender = "";
            return;
        }
        
        if(wielderNode != null){
            damage.sender = wielderNode.GetPath();
        }
    }

    private void UpdatePositions(){
        Spatial itemSpatial = item.GetNode() as Spatial;
        
        if(itemSpatial == null){
            return;
        }
        
        wieldedPosition = itemSpatial.GetTranslation();
        forwardPosition = wieldedPosition + new Vector3(0, 0, -1);
    }

    public bool CanStartStab(){
        return !stabbing;
    }

    public void StartStab(){
        if(stabbing){
            return;
        }
        if(damage.sender == ""){
            UpdateSender();
        }
        stabbing = true;
        stabTimer = stabSpeed;

        if(speaker != null){
            speaker.PlayEffect(stabSound);
        }

        IBody body = item.GetWielder() as IBody;
        if(body != null){
            body.AnimationTrigger("stab");
        }

        item.SetCollision(true);
    }

    public void EndStab(){
        if(!stabbing){
            return;
        }
        GD.Print("Stab ended");
        stabbing = false;
        stabTimer = 0f;
        item.SetCollision(false);
    }

    public void OnCollide(object body){
        if(!stabbing){
            GD.Print("Not stabbing");
            return;
        }
        if(damage == null){
            GD.Print("Damage null");
            EndStab();
            return;
        }

        IReceiveDamage receiver = body as IReceiveDamage;
        if(body == item.GetWielder()){
            GD.Print("Not stabbing self");
            return;
        }
        if(receiver != null){
            receiver.ReceiveDamage(damage);

            if(speaker != null){
                speaker.PlayEffect(impactSound);
            }
            EndStab();
        }

    }
}