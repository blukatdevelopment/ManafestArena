using Godot;
using System;
using System.Collections.Generic;

public class MeleeBiteItem : Item, IHasDamage, IWeapon {
    public ItemMeleeAttacker attacker;
    public StatConsumer consumer;

    public MeleeBiteItem(){}

    public MeleeBiteItem(
        string name,
        string description,
        string meshPath,
        float biteSpeed,
        Damage damage,
        int healthCost,
        int manaCost,
        int staminaCost,
        Sound.Effects biteSound,
        Sound.Effects impactSound
    ){
        this.name = name;
        Name = name;
        this.description = description;
        this.meshPath = meshPath;
        InitNodeStructure();

        attacker = new ItemMeleeAttacker(this as IItem, "bite");

        attacker.Config(
            biteSpeed,
            damage,
            speaker,
            biteSound,
            impactSound
        );

        consumer = new StatConsumer(healthCost, manaCost, staminaCost);
    }

    public override void Update(float delta){
        attacker.Update(delta);
    }

    public override void Equip(object wielder){
        this.wielder = wielder;
        IBody body = wielder as IBody;
        if(body != null){
            body.HoldItem(0, this as IItem);
        }

        SetCollision(false);
        SetPhysics(false);
        attacker.OnUpdateWielder();
    }

    public override void Unequip(){
        Node node = wielder as Node;

        IBody body = wielder as IBody;
        if(body != null){
            body.ReleaseItem(0, this as IItem);
        }

        this.wielder = null;
        attacker.OnUpdateWielder();
        SetCollision(true);
        SetPhysics(true);
    }

    public override void Use(MappedInputEvent inputEvent){
        Item.ItemInputs input = (Item.ItemInputs)inputEvent.mappedEventId;
        if(input == Item.ItemInputs.A && inputEvent.inputType == MappedInputEvent.Inputs.Press){
            Bite();
        }
    }

    public void Bite(){
        if(attacker.CanStartAttack() && consumer.ConsumeStats()){
            attacker.StartAttack();
        }
    }

    public Damage GetDamage(){
        return attacker.damage;
    }

    public float AttackDelay(){
        return attacker.attackSpeed;
    }

    public override void OnCollide(object body){
        attacker.OnCollide(body);
    }

    public override List<ItemFactory.Items> GetSupportedItems(){
        return new List<ItemFactory.Items>(){
            ItemFactory.Items.Teeth
        };
    }

    public override IItem Factory(ItemFactory.Items item){
        Damage dmg = new Damage();
        IItem ret = null;
        switch(item){
            case ItemFactory.Items.Teeth:
                dmg.health = 50;
                ret = new MeleeBiteItem(
                        "Teeth",
                        "Helps you chew through bones",
                        "res://Assets/Models/teeth.obj",
                        1f,
                        dmg,
                        0,
                        15,
                        0,
                        Sound.Effects.FistSwing,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
        }
        return ret;
    }
}