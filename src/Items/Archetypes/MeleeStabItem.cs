using Godot;
using System;
using System.Collections.Generic;

public class MeleeStabItem : Item, IHasDamage, IWeapon {
    public ItemStabber stabber;
    public StatConsumer consumer;

    public MeleeStabItem(){}

    public MeleeStabItem(
        string name,
        string description,
        string meshPath,
        float stabSpeed,
        Damage damage,
        int healthCost,
        int manaCost,
        int staminaCost,
        Sound.Effects stabSound,
        Sound.Effects impactSound
    ){
        this.name = name;
        Name = name;
        this.description = description;
        this.meshPath = meshPath;
        InitNodeStructure();

        stabber = new ItemStabber(this as IItem);

        stabber.Config(
            stabSpeed,
            damage,
            speaker,
            stabSound,
            impactSound
        );

        consumer = new StatConsumer(healthCost, manaCost, staminaCost);
    }

    public override void Update(float delta){
        stabber.Update(delta);
    }

    public override void Equip(object wielder){
        this.wielder = wielder;
        IBody body = wielder as IBody;
        if(body != null){
            body.HoldItem(0, this as IItem);
        }

        SetCollision(false);
        SetPhysics(false);
        stabber.OnUpdateWielder();
    }

    public override void Unequip(){
        Node node = wielder as Node;

        IBody body = wielder as IBody;
        if(body != null){
            body.ReleaseItem(0, this as IItem);
        }

        this.wielder = null;
        stabber.OnUpdateWielder();
        SetCollision(true);
        SetPhysics(true);
    }

    public override void Use(MappedInputEvent inputEvent){
        Item.ItemInputs input = (Item.ItemInputs)inputEvent.mappedEventId;
        if(input == Item.ItemInputs.A && inputEvent.inputType == MappedInputEvent.Inputs.Press){
            Stab();
        }
    }

    public void Stab(){
        if(stabber.CanStartStab() && consumer.ConsumeStats()){
            stabber.StartStab();
        }
    }

    public Damage GetDamage(){
        return stabber.damage;
    }

    public float AttackDelay(){
        return stabber.stabSpeed;
    }

    public override void OnCollide(object body){
        stabber.OnCollide(body);
    }

    public override List<ItemFactory.Items> GetSupportedItems(){
        return new List<ItemFactory.Items>(){
            ItemFactory.Items.Knife,
            ItemFactory.Items.Claws
        };
    }

    public override IItem Factory(ItemFactory.Items item){
        Damage dmg = new Damage();

        switch(item){
            case ItemFactory.Items.Knife:
                dmg.health = 100;
                return new MeleeStabItem(
                        "Knife",
                        "Don't bring a gun to a knifefight.",
                        "res://Assets/Models/knife.obj",
                        1.5f,
                        dmg,
                        0,
                        15,
                        0,
                        Sound.Effects.FistSwing,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
            case ItemFactory.Items.Claws:
                dmg.health = 50;
                return new MeleeStabItem(
                        "Claws",
                        "Knives conveniently placed on your hands.",
                        "res://Assets/Models/claw.obj",
                        0.5f,
                        dmg,
                        0,
                        15,
                        0,
                        Sound.Effects.FistSwing,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
        }
        return null;
    }
}