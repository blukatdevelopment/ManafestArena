using Godot;
using System;
using System.Collections.Generic;

public class MeleeStabItem : Item, IHasDamage {
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

    public override void Equip(object wielder){
        this.wielder = wielder;
        SetCollision(false);
        stabber.OnUpdateWielder();
    }

    public override void Unequip(){
        this.wielder = null;
        stabber.OnUpdateWielder();
        SetCollision(true);

    }

    public override void Use(ItemInputs input){
        if(input == Item.ItemInputs.ARelease){
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
                dmg.health = 35;
                return new MeleeStabItem(
                        "Knife",
                        "Don't bring a gun to a knifefight.",
                        "res://Assets/Models/dagger.obj",
                        0.5f,
                        dmg,
                        0,
                        15,
                        0
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
                        0
                    ) as IItem;
            break;
        }
        return null;
    }
}