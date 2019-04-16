// Throw this out of a projectile launcher
public class ProjectileItem : Item {
    public CollisionDamager collision;

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
        collsion.Config(
            damage,
            speaker,
            impactSound
        );
    }

    public override OnCollide(object body){
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

        switch(item){
            case ItemFactory.Items.MusketBall:
                dmg.health = 100;
                return new ProjectileItem(
                        "Musket Ball",
                        "Spherical and deadly when moving at high speeds.",
                        "res://Assets/Models/Bullet.obj",
                        dmg,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
            case ItemFactory.Items.CrossbowBolt:
                dmg.health = 100;
                return new ProjectileItem(
                        "Crossbow Bolt",
                        "Pointy, aerodnamic, and deadly.",
                        "res://Assets/Models/Bullet.obj",
                        dmg,
                        Sound.Effects.FistImpact
                    ) as IItem;
            break;
        }
        return null;
    }
}