using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementType
{
    public string name;
    public Dictionary<TerrainType, int> movementCosts = new Dictionary<TerrainType, int>();
}

/// <summary>
/// Stores gameplay data for TerrainType and PropertyType.
/// </summary>
public class TerrainStaticData
{
    /// <summary>
    /// The name of this type of terrain.
    /// </summary>
    public string name;

    //TODO: Implement icons for Terrains and Properties
    /// <summary>
    /// Icon for this terrain. Used in the editor.
    /// </summary>
    public Texture2D icon;

    /// <summary>
    /// The defense rating of this type of terrain.
    /// </summary>
    public int defense;

    /// <summary>
    /// Whether this type of terrain is capturable or not.
    /// </summary>
    public Capturable isCapturable;

    /// <summary>
    /// What altitude this type of terrain is at. Defines what kinds of units can move on it.
    /// </summary>
    public Altitude altitude;

    public TerrainStaticData(string name/*, TODO: icon */, int defense, Altitude altitude, Capturable isCapturable)
    {
        this.name = name;
        //this.icon = icon;
        this.defense = defense;
        this.altitude = altitude;
        this.isCapturable = isCapturable;
    }

    //Cleanup function for the Texture2D, since Mono's GC won't catch it when TerrainStaticData dies.
    //TODO: Does this even need to be called? The only time I can imagine it happening is when the app closes, and then the memory will be freed anyway.
    public static void Destroy(TerrainStaticData gameplayTerrainData)
    {
        Texture2D.Destroy( gameplayTerrainData.icon );
        gameplayTerrainData = null;
    }
}

/// <summary>
/// Stores visual representation of terrain type.
/// </summary>
public class TerrainPrefabs
{
    public string name;
    public GameObject block;
    public GameObject[,] walls = new GameObject[3, 3];

    public GameObject getWall(TerrainCornerType left, TerrainCornerType right)
    {
        GameObject go;
        go = walls[(int)left, (int)right];
        return go;
    }

    public TerrainPrefabs(string name)
    {
        this.name = name;

        System.Text.StringBuilder path = new System.Text.StringBuilder( "prefabs/terrain/" + name );

        string tilesetPath = "prefabs/terrain/" + name;
        block = Resources.Load( tilesetPath + "/terrain_block" ) as GameObject;

        //load walls
        for (int l = 0; l < 3; l++)
        {
            for (int r = 0; r < 3; r++)
            {
                if (path.Length != 0)
                    path.Remove( 0, path.Length );

                path.Append( tilesetPath + "/terrain_wall_" );

                switch (l)
                {
                    case ((int)TerrainCornerType.Flat): path.Append( "L-Flat" ); break;
                    case ((int)TerrainCornerType.Inner): path.Append( "L-Inner" ); break;
                    case ((int)TerrainCornerType.Outer): path.Append( "L-Outer" ); break;
                }

                path.Append( "_" );

                switch (r)
                {
                    case ((int)TerrainCornerType.Flat): path.Append( "R-Flat" ); break;
                    case ((int)TerrainCornerType.Inner): path.Append( "R-Inner" ); break;
                    case ((int)TerrainCornerType.Outer): path.Append( "R-Outer" ); break;
                }

                walls[l, r] = Resources.Load( path.ToString(), typeof( GameObject ) ) as GameObject;
            }
        }
    }

    /// <summary>
    /// Destructor for TerrainPrefabs
    /// </summary>
    /// <param name="terrainPrefabs"></param>
    public static void Destroy(TerrainPrefabs terrainPrefabs)
    {
        GameObject.Destroy( terrainPrefabs.block );
        foreach (GameObject wall in terrainPrefabs.walls)
            GameObject.Destroy( wall );

        terrainPrefabs = null;
    }
}

/// <summary>
/// Contains all information regarding a type of terrain.
/// </summary>
/// 
public class TerrainType
{
    /// <summary>
    /// The data representation of this terrain type
    /// </summary>
    public readonly TerrainStaticData gameplayData;

    /// <summary>
    /// The visual representations of this terrain type
    /// </summary>
    public TerrainPrefabs prefabs;

    public TerrainType(TerrainStaticData data)
    {
        gameplayData = data;

        this.prefabs = new TerrainPrefabs( gameplayData.name );
        TypeContainer<TerrainType>.add( this.gameplayData.name, this );
    }

    public TerrainType(TerrainStaticData data, TerrainPrefabs prefabs)
    {
        gameplayData = data;

        if (prefabs == null)
            this.prefabs = new TerrainPrefabs( gameplayData.name );

        TypeContainer<TerrainType>.add( gameplayData.name, this );
    }
}

/// <summary>
/// Contains all information regarding a terrain instance.
/// </summary>
/// 
public class TerrainInstance : MonoBehaviour {

    public static GameObject autoWaller = new GameObject( "AutoWaller" );

    public Node node;

    public PropertyInstance property;

    public TerrainType type;

    /// <summary>
    /// Getter for staticData. 
    /// </summary>
    public TerrainStaticData staticData
    {
        get
        {
            if (property != null)
                return property.type.gameplayData;
            return type.gameplayData;
        }
    }

    public List<GameObject> walls = new List<GameObject>();

    /// <summary>
    /// Creates a new TerrainInstance
    /// </summary>
    /// <param name="node">The node the TerrainInstance represents, and from where it derives its position</param>
    /// <param name="terrainType">the type of the terrainInstance</param>
    /// <returns>a terrainInstance instance</returns>
    public static TerrainInstance Create(Node node, TerrainType terrainType)
    {

        if (node.terrain != null)
        {
            throw new System.Exception( "TerrainInstance.Create() was called on a node with a non-null terrainInstance. Use TerrainInstance.Replace() instead." );
        }

        //Debug.Log( terrainType.prefabs.block );
        GameObject go = GameObject.Instantiate( 
            terrainType.prefabs.block , 
            node.position , 
            Quaternion.identity ) as GameObject;

        TerrainInstance terrainInstance = go.AddComponent<TerrainInstance>();
        terrainInstance.node = node;
        node.terrain = terrainInstance;
        terrainInstance.type = terrainType;
        terrainInstance.transform.parent = node.tile.transform;

        //nobody cares
        //go.name = terrainInstance.type.prefabs.block.name;

        return terrainInstance;
    }

    /// <summary>
    /// Replaces a terrainInstance with a new type of terrain.
    /// </summary>
    /// <param name="block">the terrainInstance to replace.</param>
    /// <param name="newType">the type of terrain of the new terrainInstance.</param>
    public static void Replace(TerrainInstance block , TerrainType newType)
    {
        Node n = block.node;
        Destroy( n.terrain.gameObject );
        n.terrain = null;
        Create( n , newType );

        //SPEEDUP: Could be optimized if know previous block modified.
        List<Node> nearNodes = Board.instance.getNodesAdjacent( n.position );
        List<Node> fartherNodes;
        foreach ( Node nearNode in nearNodes )
        {
            fartherNodes = Board.instance.getNodesAdjacent( nearNode.position );
            foreach ( Node fartherNode in fartherNodes )
            {
                fartherNode.terrain.autoWall();
            }
            nearNode.terrain.autoWall();
        }
    }

    public void createWall(TerrainCornerType left, TerrainCornerType right, float rot )
    {
        try
        {
            Vector3 rotDir = new Vector3( 0f , rot , 0f );

            GameObject go = type.prefabs.getWall( left , right );
            if ( go == null )
                return;

            go = Instantiate( type.prefabs.getWall( left, right ) , transform.position , Quaternion.Euler( rotDir) ) as GameObject;
            walls.Add( go );
            go.transform.parent = transform;
        }
        catch ( System.Exception e )
        {
            Debug.LogError( e.ToString() + ": COULD NOT INSTANTIATE WALL " + left.ToString() + " " + right.ToString() );
        }
        //go.transform.eulerAngles = new Vector3(0f, direction.y, 0f);
    }

    
    /// <summary>
    /// Helper function for autoWall()
    /// </summary>
    /// <param name="altitude">its altitude why the fuck can't you figure this out yourself</param>
    /// <returns></returns>
    private bool isDifferent(Altitude altitude)
    {
        if ( altitude == Altitude.unassigned ) return true;
        return altitude != type.gameplayData.altitude ? true : false;
    }

    
    /// <summary>
    /// Adds pretty looking cliffs between terrain of different altitudes
    /// </summary>
    public void autoWall()
    {
        if ( node == null )
            return;

        Transform auto = autoWaller.transform;
        auto.position = node.position;
        auto.rotation = transform.rotation;

        foreach ( GameObject wall in walls )
        {
            Destroy( wall );
        }

        for ( int i = 0 ; i < 4 ; i++ )
        {

            // O O O
            // O X O
            // O O O
            //auto.rotation = Quaternion.AngleAxis( 90 , Vector3.up );


            Node n;
            Altitude front = Altitude.unassigned;
            Altitude frontRight = Altitude.unassigned; ;
            Altitude frontLeft = Altitude.unassigned; ;
            Altitude back = Altitude.unassigned; ;
            Altitude backRight = Altitude.unassigned; ;
            Altitude backLeft = Altitude.unassigned; ;
            Altitude left = Altitude.unassigned; ;
            Altitude right = Altitude.unassigned; ;

            //Assign front
            n = Board.instance.getNode( auto.position + auto.forward );
            if (n != null)
            front = n.terrain.type.gameplayData.altitude;

            //assign frontright
            n = Board.instance.getNode( auto.position + auto.forward + auto.right );
            if ( n != null )
            frontRight = n.terrain.type.gameplayData.altitude;

            //assign frontleft
            n = Board.instance.getNode( auto.position + ( auto.forward - auto.right ) );
            if ( n != null )
            frontLeft = n.terrain.type.gameplayData.altitude;

            //assign back
            n = Board.instance.getNode( auto.position - ( auto.forward ) );
            if ( n != null )
            back = n.terrain.type.gameplayData.altitude;

            //assign backright
            n = Board.instance.getNode( auto.position - ( auto.forward + auto.right ) );
            if ( n != null )
            backRight = n.terrain.type.gameplayData.altitude;

            //assign backleft
            n = Board.instance.getNode( auto.position - ( auto.forward - auto.right ) );
            if ( n != null )
            backLeft = n.terrain.type.gameplayData.altitude;

            //assign left
            n = Board.instance.getNode( auto.position - ( auto.right ) );
            if ( n != null ) 
                left = n.terrain.type.gameplayData.altitude;

            //assign right
            n = Board.instance.getNode( auto.position + ( auto.right ) );
            if ( n != null )
            right = n.terrain.type.gameplayData.altitude;

            TerrainCornerType lCorner = TerrainCornerType.Flat;
            TerrainCornerType rCorner = TerrainCornerType.Flat;

            // 2 1 2
            //   X 
            //
            //if there's nothing in front (1)
            if ( isDifferent(front) )
            {
                //if there's nothing on the frontleft and nothing on the frontright
                if ( isDifferent(frontLeft) && isDifferent( frontRight) )
                {
                    //if there's nothing on left and right
                    if ( isDifferent( left ) && isDifferent( right ) )
                    {
                        //wall with both rounded corners
                        lCorner = TerrainCornerType.Outer;
                        rCorner = TerrainCornerType.Outer;
                    }
                    //if there's something on the right
                    else if ( isDifferent(left) && !isDifferent(right))
                    {
                        //wall with rounded on left, flat on right
                        lCorner = TerrainCornerType.Outer;
                        rCorner = TerrainCornerType.Flat;
                    }
                    //if there's something on the left
                    else if ( !isDifferent(left) && isDifferent(right))
                    {
                        //wall with flat on the left, rounded on right
                        lCorner = TerrainCornerType.Flat;
                        rCorner = TerrainCornerType.Outer;
                    }
                }
                //if there's nothing on the frontleft and something on the frontright
                else if ( isDifferent(frontLeft) && !isDifferent(frontRight))
                {
                    if ( isDifferent(left) )
                    {
                        //wall with rounded edge on left, inner corner on right
                        lCorner = TerrainCornerType.Outer;
                        rCorner = TerrainCornerType.Inner;
                    }
                    else
                    {
                        //wall with flat on left, inner corner on right
                        lCorner = TerrainCornerType.Flat;
                        rCorner = TerrainCornerType.Inner;
                    }
                }
                //if there's something on the frontleft and nothing on the frontright
                else if ( !isDifferent(frontLeft) && isDifferent(frontRight))
                {
                    if ( isDifferent(right) )
                    {
                        //wall with inner corner on left, rounded on right
                        lCorner = TerrainCornerType.Inner;
                        rCorner = TerrainCornerType.Outer;
                    }
                    else
                    {
                        //wall with inner corner on left, flat on right
                        lCorner = TerrainCornerType.Inner;
                        rCorner = TerrainCornerType.Flat;
                    }
                }
                else if ( !isDifferent( frontLeft ) && !isDifferent( frontRight ) )
                {
                    lCorner = TerrainCornerType.Inner;
                    rCorner = TerrainCornerType.Inner;
                }
                createWall( lCorner , rCorner , auto.eulerAngles.y );

            }
            //else don't place a wall here
            auto.rotation = Quaternion.AngleAxis( auto.eulerAngles.y + 90 , Vector3.up );

        }

    }

    public void kill()
    {
        foreach ( GameObject w in walls )
            Destroy( w );
        Destroy( gameObject );
    }
}
