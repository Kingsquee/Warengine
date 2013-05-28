using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SquadMovementType { Infantry, Mech, Tires, Tread, Air, Ships, Pipeline, Transport, Oozium } //Smoozium!

public class SquadStaticData
{
    public string name;
    //TODO: Texture2D icon; Should Squads have an icon, or should we just rip that from the SpriteType somehow?
    public int productionCost;
    public SquadMovementType movementType;
    public Altitude altitude;
    public int movementPointsDefault;
    public int fuelDefault;
    public int visionDefault;
    public int minRangeDefault;
    public int maxRangeDefault;
    public int ammoDefault;
    public List<Ability> abilities;

    public SquadStaticData(
        string name, 
        int productionCost, 
        SquadMovementType movementType, 
        Altitude altitude, 
        int movementPointsDefault, 
        int fuelDefault, 
        int visionDefault, 
        int minRangeDefault, 
        int maxRangeDefault, 
        int ammoDefault,
        List<Ability> abilities) //TODO: expand this
    {
        this.name = name;
        this.productionCost = productionCost;
        this.movementType = movementType;
        this.altitude = altitude;
        this.movementPointsDefault = movementPointsDefault;
        this.fuelDefault = fuelDefault;
        this.visionDefault = visionDefault;
        this.minRangeDefault = minRangeDefault;
        this.maxRangeDefault = maxRangeDefault;
        this.ammoDefault = ammoDefault;
        this.abilities = abilities;
    }
}

public class SquadType
{
    public SquadStaticData staticData;

    public GameObject prefab;

    public SquadType(SquadStaticData data)
    {
        staticData = data;

        //HACK: squadtype prefab is always the same, since we don't need any variations if squads are just billboards.
        prefab = Resources.Load( "prefabs/squads/placeholder squad" ) as GameObject;

        TypeContainer<SquadType>.add( staticData.name, this );
    }
}

[RequireComponent( typeof( Selectable ) )]
public class SquadInstance : MonoBehaviour
{
    private Selectable selectable;

    public SquadType _type;
    public SquadType type
    {
        get { return _type; }
        set
        {
            _type = value;
            _fuel = _type.staticData.fuelDefault;
            _vision = _type.staticData.visionDefault;
            _minRange = _type.staticData.minRangeDefault;
            _maxRange = _type.staticData.maxRangeDefault;
            _ammo = _type.staticData.ammoDefault;
        }
    }

    public AbilityContainer abilityContainer;

    private int _health = 100;      public int health 
    { 
        get { return _health; }
        set { _health = value; if (_health <= 0) kill(); } 
    }
    private int _fuel;              public int fuel
    {
        get { return _fuel; }
        set { _fuel = value; }
    }
    private int _vision;            public int vision
    {
        get { return _vision; }
        set { _vision = value; }
    }
    private int _minRange;          public int minRange
    {
        get { return _minRange; }
        set { _minRange = value; }
    }
    private int _maxRange;          public int maxRange
    {
        get { return _maxRange; }
        set { _maxRange = value; }
    }
    private int _ammo;              public int ammo
    {
        get { return _ammo; }
        set { _ammo = value; }
    }


    public delegate IEnumerator KillAnimationDelegate(GameObject go);
    KillAnimationDelegate killAnimation = DefaultKillAnimation;

    /// <summary>
    /// Creates a new SquadInstance
    /// </summary>
    /// <param name="node">The board node the </param>
    /// <param name="squadType"></param>
    /// <returns></returns>
    public static SquadInstance Create(Node node, SquadType squadType)
    {
        if (node.obstruction != null)
        {
            throw new System.Exception( "SquadInstance.Create() was called on a non-null terrain node's obstruction. Use SquadInstance.Replace() instead." );
        }

        GameObject go = GameObject.Instantiate(
            squadType.prefab,
            node.position,
            Quaternion.identity
            ) as GameObject;


        SquadInstance squadInstance = go.AddGetComponent<SquadInstance>();
        squadInstance.type = squadType;

        squadInstance.selectable = go.AddGetComponent<Selectable>();

        squadInstance.abilityContainer = new AbilityContainer();
        squadInstance.abilityContainer.caster = squadInstance.selectable;
        foreach (Ability a in squadType.staticData.abilities)
            squadInstance.abilityContainer.add( a );
        go.name = squadType.staticData.name;

        return squadInstance;
    }

    public static void Replace(SquadInstance squadInstance, SquadType newType)
    {
        Node n = Board.instance.getNode( squadInstance.transform.position );
        if (n == null)
        {
            Debug.LogError( "The squadInstance you are trying to replace is not within the bounds of the Board. Something dun messed up." );
            return;
        }

        squadInstance.kill();
        n.obstruction = null;
        Create( n, newType );

    }

    void Start()
    {
        Debug.Log( transform.position );
        Board.instance.occupyVirtualPosition( transform.position, transform );        
    }

    void onSelect()
    {
        abilityContainer.setupAll();
    }

    void onDeselect()
    {
        abilityContainer.cancelAll();
    }

    private static IEnumerator DefaultKillAnimation(GameObject go)
    {
        var time = 0.5f;

        Debug.Log( "Running tween" );

        GameManager.DisableUnitSelection();

        Go.to( go.transform, time, new TweenConfig().position( new Vector3( 0, Camera.mainCamera.transform.position.y - 1f, 0 ), true ).setEaseType( EaseType.BounceOut )/*.rotation( new Vector3( 45, 45, -45) )*/ );
        yield return new WaitForSeconds( time );

        GameManager.EnableUnitSelection();

        Debug.Log( "waited " + time + " seconds" );
    }

    public void kill()
    {
        var n = Board.instance.getNode( transform.position );
        var ob = n.obstruction;
        if (ob == transform)
            n.obstruction = null;


        if (GameManager.selected != null && GameManager.selected.transform == transform)
            GameManager.selected.deselect();

        Debug.Log( "Playing death animation" );
        JobManager.instance.EnqueueJob( killAnimation( gameObject ) );
        JobManager.instance.EnqueueJob( GameObjectExtensions.IteratorDestroy( gameObject ) );
    }


}