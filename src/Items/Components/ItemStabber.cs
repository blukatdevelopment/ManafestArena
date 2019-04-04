/*
    Item snaps forward to damage an enemy,
    then snaps back when either a target
    is hit or the stab time runs out.
*/
public class ItemStabber {
    IItem item;
    public Vector3 wieldedPosition, forwardPosition;
    public bool stabbing = false;
    public float stabSpeed, stabTimer;
    public Damage damage;
    public Speaker speaker;
    public Sound.Effects stabSound;

    public ItemStabber(IItem item){
        this.item = item;
        this.stabSound = Sound.Effects.FistSwing;
    }

    public void Config(
        float stabSpeed, // How long a stab takes to complete
        Damage damage, // Damage object passed to stabbed target
        Vector3 forwardPosition, // How far forward will they go?
        Vector3 wieldedPosition, // Where should the item return to?
        Speaker speaker // Use this to make noise
    ){
        this.stabSpeed  = stabSpeed;
        this.damage     = damage;
        this.speaker    = speaker;
    }

    public void Update(float delta){
        this.item = item;
        if(stabbing){
            stabTimer -= delta;
            if(stabTimer <= 0f){
                stabbing = false;
                EndStab();
            }
        }
    }

    public void StartStab(){
        if(stabbing){
            return;
        }

        stabbing = true;
        stabTimer = stabSpeed;

        if(speaker != null){
            speaker.PlayEffect(stabSound);
        }

        item.GetNode().Translation = forwardPosition;
        item.SetCollision(true);
    }

    public void EndStab(){
        stabbing = false;
        stabTimer = 0f;

        item.GetNode().Translation = wieldedPosition;
        item.SetCollision(false);
    }

    public void OnCollide(object body){
        if(!stabbing){
            return;
        }
        if(damage == null){
            EndStab();
            return;
        }

        IReceiveDamage receiver = body as IReceiveDamage;
        if(receiver != null){
            receiver.ReceiveDamage(damage);
        }

    }
}