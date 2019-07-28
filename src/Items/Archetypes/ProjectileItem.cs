// Throw this out of a projectile launcher
using Godot;
using System;
using System.Collections.Generic;

public class ProjectileItem : Item {
    public CollisionDamager collision;
    public string sender;


    public ProjectileItem(){}

    public ProjectileItem(
        string name,
        string description,
        string meshPath,
        Damage damage,
        Sound.Effects impactSound
    ){
        this.name = name;
        this.description = description;
        this.meshPath = meshPath;

        InitNodeStructure();        

        collision = new CollisionDamager(this as IItem);
        collision.Config(
            damage,
            speaker,
            impactSound
        );
        InitArea();
    }

    public override void OnCollide(object body){
        ProjectileItem proj = body as ProjectileItem;
        if(proj != null){
            GD.Print("Colliding with a projectile");
            if(proj == this){
                GD.Print("Colliding with self");
                return;
            }
        }
        
        collision.OnCollide(body);
        QueueFree();
    }

    public override List<ItemFactory.Items> GetSupportedItems(){
        return new List<ItemFactory.Items>(){
            ItemFactory.Items.MusketBall,
            ItemFactory.Items.CrossbowBolt
        };
    }

    public override IItem Factory(ItemFactory.Items item){
        Damage dmg = new Damage();
        IItem ret = null;
        switch(item){
            case ItemFactory.Items.MusketBall:
                dmg.health = 100;
                ret = new ProjectileItem(
                        "Musket Ball",
                        "Spherical and deadly when moving at high speeds.",
                        "res://Assets/Models/Bullet.obj",
                        dmg,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
            case ItemFactory.Items.CrossbowBolt:
                dmg.health = 100;
                ret = new ProjectileItem(
                        "Crossbow Bolt",
                        "Pointy, aerodnamic, and deadly.",
                        "res://Assets/Models/Bullet.obj",
                        dmg,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
        }
        return ret;
    }
}