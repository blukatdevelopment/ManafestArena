public class ProjectileLauncher {
    IItem item;

    public Item.Types projectileType;
    public float launchImpulse;

    public ProjectileLauncher(IItem item){
        this.item = item;
    }

    public void Config(
        Item.Types projectileType,
        float launchImpulse
    ){
        this.projectileType = projectileType;
        this.damage = damage;
        this.launchImpulse = launchImpulse;
    }

    public void Fire(){
        speaker.PlayEffect(Sound.Effects.RifleShot);
        IItem projectile = ItemFactory.Factory(projectileType);
        
        Actor wielderActor = item.GetWielder() as Actor;
        
        if(wielderActor != null){
          proj.sender = wielderActor.NodePath();
        }
        
        Vector3 projectilePosition = ProjectilePosition();
        Vector3 globalPosition = item.GetNode().ToGlobal(projectilePosition);
        Spatial gameNode = (Spatial)Session.GameNode();
        
        Vector3 gamePosition = gameNode.ToLocal(globalPosition);
        projectile.Translation = gamePosition;
        gameNode.AddChild(projectile);

        Transform start = ittem.GetNode().GetGlobalTransform();
        Transform destination = start;
        destination.Translated(new Vector3(0, 0, 1));
        
        Vector3 impulse = start.origin - destination.origin;
        projectile.SetAxisVelocity(impulse * impulseStrength);
    }
}