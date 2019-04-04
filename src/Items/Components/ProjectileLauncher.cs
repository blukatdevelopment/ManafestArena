public class ProjectileLauncher {
    IItem item;

    public Item.Types projectileType;
    public Damage damage;
    public float launchImpulse;

    public ProjectileLauncher(IItem item){
        this.item = item;
    }

    public void Config(
        Item.Types projectileType,
        Damage damage,
        float launchImpulse
    ){
        this.projectileType = projectileType;
        this.damage = damage;
        this.launchImpulse = launchImpulse;
    }

    public void Fire(){
        speaker.PlayEffect(Sound.Effects.RifleShot);
        Item projectile = Item.Factory(projectileType);
        projectile.Name = "Projectile";
        Projectile proj = projectile as Projectile;

        if(proj != null && damage != null){
            proj.damage = damage;
        }

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