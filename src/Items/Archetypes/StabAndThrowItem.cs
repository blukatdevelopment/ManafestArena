public class StabAndThrowItem : MeleeStabItem{
    ItemThrower thrower;

    public MeleeStabItem(
        string name,
        string description,
        string meshPath,
        float stabSpeed,
        Damage damage,
        int healthCost,
        int manaCost,
        int staminaCost,
        float throwStrength
    ){
        this.name = name;
        this.description = description;
        this.meshPath = meshPath;
        InitNodeStructure();

        stabber = new ItemStabber(this as IItem);

        stabber.Config(
            stabSpeed,
            damage,
            forewardPosition,
            wieldedPosition,
            speaker
        );

        consumer = new StatConsumer(healthCost, manaCost, staminaCost);
        thrower = new ItemThrower(this as IItem);
        thrower.Config();
    }

    public override void Equip(object wielder){
        this.wielder = wielder;
        SetCollision(false);
        stabber.OnUpdateWielder();
    }
}