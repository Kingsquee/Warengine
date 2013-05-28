using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The game board
/// </summary>
public class Board : MonoBehaviour {

    private Node[ , ] nodes;
    public const int width = 30;
    public const int length = 20;

    private Vector3[] cardinalDirs = new Vector3[4] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

	// Absolute game start.
	void Awake () 
    {
        LevelEditor.instance.PopulateTypeContainers();

        nodes = new Node[width , length];
        generateNodes();
	}
    
    /// <summary>
    /// Gets the node at xz position. 
    /// This function does not perform sanity checks on its input, so it runs fast; If that makes you worried, sanitize your input with areCoordsValid(), or or use getNode().
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="z">z position</param>
    /// <returns>node at position xyz</returns>
    public Node getNodeUnsafely(int x, int z)
    {
        return nodes[x, z];
    }

    /// <summary>
    /// Gets the node at Vector3 position. 
    /// This function does not perform sanity checks on its input, so it runs fast; If that makes you worried, sanitize your input with areCoordsValid(), or or use getNode().
    /// </summary>
    /// <param name="v">node position</param>
    /// <returns>node at position of Vector3</returns>
    public Node getNodeUnsafely(Vector3 v)
    {
        var vr = Mathfx.Round( v );
        return getNodeUnsafely( (int)vr.x, (int)vr.z );
    }

    /// <summary>
    /// Gets the node at Vector3 position. This function automatically calls areCoordsValid() on the input. If you want faster access or know what you're doing, use getNodeUnsafely instead.
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="z">z position</param>
    /// <returns>Node at position xyz</returns>
    public Node getNode( int x, int z )
    {
        if (areCoordsValid(x, z))
            return nodes[x, z];
        return null;
    }

    /// <summary>
    /// Gets the node at Vector3 position. This function automatically calls areCoordsValid() on the input. If you want faster access or know what you're doing, use getNodeUnsafely instead.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Node at position of Vector3</returns>
    public Node getNode( Vector3 v )
    {
        var vr = Mathfx.Round( v );
        return getNode( (int)vr.x , (int)vr.z );
    }

    /// <summary>
    /// Checks if the coordinates are valid positions on the board
    /// </summary>
    /// <param name="x">x position</param>
    /// <param name="z">z position</param>
    public bool areCoordsValid(int x, int z)
    {
        //Are the params within the board width?
        if ((x < width) && (x >= 0) && (z < length) && (z >= 0))
            return true;
        return false;
    }

    public void setNode( Vector3 v , Node n )
    {
        Node o = getNode( v );
        o = n;
    }
    
    /// <summary>
    /// Gets the nodes adjacent to the position. (Forward, Behind, Left, and Right)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public List<Node> getNodesAdjacent(Vector3 v)
    {
        List<Node> list = new List<Node>();

        foreach (Vector3 dir in cardinalDirs)
        {
            list.Add( getNode( v + dir ) );
            if (list[list.Count - 1] == null)
                list.RemoveAt( list.Count - 1 );
        }

        return list;
    }
    

    //SPEEDUP: Cache List of nodes?
    public List<Node> getNodesAsList()
    {
        List<Node> nodeList = new List<Node>();
        for ( int x = 0 ; x < width ; x++ )
        {
            for ( int z = 0 ; z < length ; z++ )
            {
                nodeList.Add( nodes[x , z] );
            }
        }
        
        return nodeList;
    }

    public void clearVirtualPosition( Vector3 position )
    {
        var p = Mathfx.Round( position );
        getNode( p ).obstruction = null;
    }

    public void occupyVirtualPosition(Vector3 position, Transform occupant)
    {
        var p = Mathfx.Round( position );
        //Debug.Log( p );
        getNode( p ).obstruction = occupant;
    }


    void generateNodes()
    {
        //Debug.Log( width );
        //Debug.Log( length );

        TerrainType terra = TypeContainer<TerrainType>.get( "Plain" );
        //Debug.Log( "TERRAIN TYPE OF TERRA IS " + terra.staticDataame );

        Node n;

        for ( int x = 0 ; x < width ; x++ )
        {
            for ( int z = 0 ; z < length ; z++ )
            {
                n = new Node( terra, new Vector3(x , 0f, z) );
                nodes[x , z] = n;
            }
        }
        

        for ( int x = 0 ; x < width ; x++ )
        {
            for ( int z = 0 ; z < length ; z++ )
            {
                n = getNodeUnsafely( x ,  z );

                n.AssignEdges();
                n.terrain.autoWall();

                if (z == 0 || z == length-1)
                    SquadInstance.Create( getNodeUnsafely( x, z ), TypeContainer<SquadType>.get( "placeholder squad" ) );
            }
        }    
    }

   /* void OnDrawGizmos()
    {
        if ( Application.isPlaying == false )
            return;

        Gizmos.color = Color.red;

        foreach ( KeyValuePair<Vector2, Node> node in nodes )
        {
           // Gizmos.DrawCube( new Vector3( node.Key.x, -0.75f , node.Key.y ), Vector3.one );
            foreach ( Edge e in node.Value.edges )
                Gizmos.DrawLine( e.parentNode.position , e.childNode.position);
        }
    }*/

    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.K ) )
        {


            for ( int x = 0 ; x < width ; x++ )
            {
                for ( int z = 0 ; z < length ; z++ )
                {
                    //Instantiate( Resources.Load( "prefabs/moveTile" ) as GameObject , node.position , Quaternion.identity );
                    foreach ( Edge e in nodes[x , z].edges )
                    {
                        Debug.DrawLine( e.parentNode.position , e.childNode.position , Color.red , 5f );
                    }

                }
            }            
        }

    }







    //Singleton stuff

    static Board _instance = null;
    public static Board instance
    {
        get
        {
            if ( !_instance )
            {
                //check if a Board is already available in the scene graph
                _instance = FindObjectOfType( typeof( Board ) ) as Board;

                //nope, create a new one
                if ( !_instance )
                {
                    var obj = new GameObject( "Board" );
                    _instance = obj.AddComponent<Board>();
                }
            }

            return _instance;
        }
    }

    void OnApplicationQuit()
    {
        //release reference on exit
        _instance = null;
    }    
}


