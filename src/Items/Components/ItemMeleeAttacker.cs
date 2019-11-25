/*
    Triggers attack animation and administers damage to first
    collision that isn't the wielder.
*/
using Godot;
using System;
using System.Collections.Generic;

public class ItemMeleeAttacker {
    IItem item;
    String animationName;
    public bool attacking;
    public float attackSpeed, attackTimer;
    public Damage damage;
    public Speaker speaker;
    public Sound.Effects attackSound, impactSound;

    public ItemMeleeAttacker(IItem item, String animationName){
        this.item = item;
        this.animationName = animationName;
        this.attacking = false;
    }

    public void Config(
        float attackSpeed,
        Damage damage,
        Speaker speaker,
        Sound.Effects attackSound,
        Sound.Effects impactSound
    ){
        this.attackSpeed  = attackSpeed;
        this.damage       = damage;
        this.speaker      = speaker;
        this.attackSound  = attackSound;
        this.impactSound  = impactSound;
    }

    public void Update(float delta){
        if(attacking){
            attackTimer -= delta;
            if(attackTimer <= 0f){
                EndAttack();
            }
        }
    }

    public void OnUpdateWielder(){
        EndAttack();
        UpdateSender();        
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

    public bool CanStartAttack(){
        return !attacking;
    }

    public void StartAttack(){
        if(attacking){
            return;
        }
        if(damage.sender == ""){
            UpdateSender();
        }
        attacking = true;
        attackTimer = attackSpeed;

        if(speaker != null){
            speaker.PlayEffect(attackSound);
        }

        IBody body = item.GetWielder() as IBody;
        if(body != null){
            body.AnimationTrigger(animationName);
        }

        item.SetCollision(true);
    }

    public void EndAttack(){
        if(!attacking){
            return;
        }
        attacking = false;
        attackTimer = 0f;
        item.SetCollision(false);
    }

    public void OnCollide(object body){
        if(!attacking){
            GD.Print("Not attaacking");
            return;
        }
        if(damage == null){
            GD.Print("Damage null");
            EndAttack();
            return;
        }
        
        Node node = body as Node;
        if(node != null){
            GD.Print("Collided with " + node.Name);
        }
        IReceiveDamage receiver = body as IReceiveDamage;
        if(body == item.GetWielder() || body == item){
            GD.Print("Not attacking self");
            return;
        }
        if(receiver != null){
            receiver.ReceiveDamage(damage);

            if(speaker != null){
                speaker.PlayEffect(impactSound);
            }
            EndAttack();
        }

    }
}