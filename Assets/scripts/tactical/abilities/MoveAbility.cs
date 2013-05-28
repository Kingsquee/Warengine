using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveAbility : Ability {

    WalkableField pathfinder = new WalkableField();
    List<Node> moveField = new List<Node>();
    List<Node> path = new List<Node>();
    int movementPoints = 15;
    float moveSpeed = 1f;

    int totalCost
    {
        get
        {
            int i = 0;
            foreach ( Node n in path )
            {
                if ( n == Board.instance.getNode( caster.transform.position ) )
                    continue;
                i += n.costModifier;
            }
            return i;
        }
    }

    public MoveAbility()
    {
        name = "Move";
        tileColor = Color.white;
    }

    public override void setup( Selectable caster )
    {
        base.setup( caster );

        if ( path.Count != 0 )
            cleanup();
        createTiles();
    }
    /*
    bool changingAltitude = false;
    public override void update()
    {
        base.update();
        if ( Input.GetKeyDown( KeyCode.LeftShift ) )
        {
            if ( changingAltitude )
                return;
            caster.StartCoroutine(changeAltitude());
        }
    }

    IEnumerator changeAltitude()
    {
        //cancel();
        abilityContainer.cancelAll();
        changingAltitude = true;
        var caa = new ChangeAltitudeAbility();
        caa.initData( caster , null );
        caa.setup();
        yield return JobManager.instance.StartCoroutine( caa.execute() );
        Debug.Log( "Caster is me? " + ( GameManager.selected == caster ) );
        //createTiles();
        abilityContainer.setupAll();
        changingAltitude = false;
    }
    */
    void createTiles()
    {
        var startPosition = Board.instance.getNode( caster.transform.position );
        var list = Board.instance.getNodesAsList();

        moveField = pathfinder.GetField( startPosition , list  , movementPoints );

        foreach ( Node node in moveField )
        {
            if (node.position == Mathfx.Round(caster.transform.position))
                continue;
            node.tile.subscribe( this );
        }
    }

    public void tryAddNodeToPath( Node node )
    {
        if ( node.tile == null )
            return;

        if ( !node.tile.isAbilitySubscribed( this ) )
            Debug.Log( "HEY I'M NOT PART OF YOUR GROUP" );
        
        /*
        if ( path.Count == 0 )
            if ( Board.instance.GetNodesAdjacent( caster.transform.position ).Contains( node ) == false )
                return;
        */

        if ( !path.Contains(Board.instance.getNode( caster.transform.position )))
            path.Insert( 0 , Board.instance.getNode( caster.transform.position ) );

        if ( node == path[ 0 ] )
            return;
        

        if ( path.Contains( node ) )
        {
            //Debug.Log( "Path already contained node" );
            for ( int i = path.Count-1 ; i > path.IndexOf( node ) ; --i )
            {
                //Debug.Log( "Removing node from Path - it is at position " + i + ". Path length is " + (path.Count-1) );
                if (path[i] != null)
                {
                    removeNodeFromPathAt( i );
                }
            }
        }
        else if ( ( totalCost + node.costModifier ) > movementPoints )
        {
            //Debug.Log( "Path longer than allowed, generating auto path from CASTER!" );
            setPath(getAutoPathTo( node ));
        }
        else //normal adding of new node
        {
            //TODO: Usability: Use A*, or write a recursive function to find path between path's last node and this node?
            //Debug.Log( "Path doesn'tile contain node." );

            bool adjacent = false;
            foreach ( Edge e in path[path.Count - 1].edges )
                if ( e.childNode == node )
                    adjacent = true;

            if ( adjacent )
            {
                //Debug.Log( "Node was adjacent. Adding." );
                addNodeToPath( node );
            }
            else
            {
                //This is a performance/eyecandy optimization.
                Edge edge = null;
            
                foreach ( Edge e in path[ path.Count - 1 ].edges )
                    foreach ( Edge f in e.childNode.edges )
                        if ( f.childNode == node )
                        {
                            edge = e;
                        }

                if ( isEdgeTraversable(node, edge) && !path.Contains( edge.childNode ))
                {
                    //Debug.Log( "Node was nearby. Adding." );
                    addNodeToPath( edge.childNode );
                    addNodeToPath( node );
                }
                else
                {
                    //Debug.Log( "Node was far away. Generating auto path from CASTER" );
                    setPath( getAutoPathTo( node ) );
                }
            }
        }

        foreach ( Node n in Board.instance.getNodesAsList() )
        {
            if (n.tile != null)
                n.tile.dehighlight();
        }
        foreach ( Node n in path )
        {
            if (n.tile != null)
                n.tile.highlight();
        }
    }

    public static bool isEdgeTraversable(Node node, Edge edge)
    {
        if (edge == null || edge.childNode == null /*|| !G.Contains(edge.childNode)*/ )
            return false;

        if (!edge.childNode.known && edge.weight < edge.childNode.weightOfCheapestEdge)
        {
            if (edge.childNode.obstruction /*&& obstruction isn't on our side*/)
                return false;

            if (edge.childNode.terrain.type.gameplayData.altitude != node.terrain.type.gameplayData.altitude)
                return false;

            return true;
        }

        return false;

    }
    
    List<Node> getAutoPath(Node startNode , Node endNode)
    {
        //use A* here
        return null;
    }

    void setPath(List<Node> l)
    {
        for ( int i = 0 ; i < path.Count ; i++ )
        {
            if ( path[ i ].tile )
                path[ i ].tile.removeHighlightObject();
        }
        path.Clear();
        foreach ( Node n in l )
            addNodeToPath( n );
    }

    void addNodeToPath(Node n)
    {
        path.Add(n);
        setArrowsFor( n );
        //set texture to path_arrow
    }

    void removeNodeFromPath(Node n)
    {
        if ( n != null )
            if ( n.tile != null )
                n.tile.removeHighlightObject();
        setArrowsFor( path[path.IndexOf( n ) - 1 ]);
        path.Remove( n );
    }

    void removeNodeFromPathAt(int i)
    {
        Node n = path[ i ];
        if ( n != null )
            if ( n.tile != null )
                n.tile.removeHighlightObject();
        setArrowsFor( path[ i - 1 ] );
        path.RemoveAt( i );

    }


    //Draws the arrows for a path, updating them relative to the node and its followers in the path.
    void setArrowsFor(Node n)
    {
        // two -> one -> node

        Node one = null;
        Node two = null;

        Quaternion q = new Quaternion();

        //if we're the start node
        if ( path.IndexOf( n ) == 0 )
            return;

        //one is the node directly following node
        one = path[ path.IndexOf( n ) - 1 ];

        //two is the node directly following one
        if ( path.IndexOf( n ) - 2 > 0 )
            two = path[ path.IndexOf( n ) - 2 ];


        Vector3 delta01 = Vector3.zero;
        Vector3 delta12 = Vector3.zero;

        //find the positional difference between node and one
        delta01 = n.position - one.position;

        //find the positional difference between one and two
        if ( two != null )
            delta12 = one.  position - two.position;
        else
            delta12 = one.position - caster.transform.position;

        //Check the the differences between node and one, finding the angle, then finding the differences between one and two, finding their angle. 
        // From there, we can tell if we need to have keep the arrow straight, or bend it.

        //SPEEDUP: Use a pool for this.
        if ( Mathfx.CompareXZ( delta01 , Vector3.forward ) )
        {
            q.eulerAngles = Vector3.zero;

            if ( one != path[ 0 ] )
            {
                if ( Mathfx.CompareXZ( delta12 , Vector3.forward ) || Mathfx.CompareXZ(delta12 , Vector3.back ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_straight" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.right ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_left" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.left ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_right" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
            }
        }
        else if ( Mathfx.CompareXZ( delta01, Vector3.back ) )
        {
            q.eulerAngles = new Vector3( 0f , 180f , 0f );

            if ( one != path[ 0 ] )
            {
                if ( Mathfx.CompareXZ( delta12 , Vector3.forward ) || Mathfx.CompareXZ( delta12 , Vector3.back ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_straight" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.right ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_right" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.left ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_left" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
            }
        }
        else if ( Mathfx.CompareXZ(delta01 , Vector3.left ) )
        {
            q.eulerAngles = new Vector3( 0f , -90f , 0f );

            if ( one != path[ 0 ] )
            {
                if ( Mathfx.CompareXZ( delta12 , Vector3.forward ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_left" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.back ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_right" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.right ) || Mathfx.CompareXZ( delta12 , Vector3.left ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_straight" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
            }
        }
        else if ( Mathfx.CompareXZ( delta01 , Vector3.right ) )
        {
            q.eulerAngles = new Vector3( 0f , 90f , 0f );

            if ( one != path[ 0 ] )
            {
                if ( Mathfx.CompareXZ( delta12 , Vector3.forward ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_right" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.back ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_left" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
                else if ( Mathfx.CompareXZ( delta12 , Vector3.right ) || Mathfx.CompareXZ( delta12 , Vector3.left ) )
                {
                    one.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_straight" ) as GameObject , one.position , q ) as GameObject ).transform );
                }
            }
        }
        n.tile.setHighlightObject( ( GameObject.Instantiate( Resources.Load( "prefabs/path/path_arrow" ) as GameObject , n.position , q ) as GameObject ).transform );
    }


    List<Node> getAutoPathTo( Node endNode )
    {
        Node n = endNode;
        var tmpPath = new List<Node>();

        if ( n == null )
            return tmpPath;
        
        while ( n.pathParent != null )
        {
            tmpPath.Add( n );
            if ( n.pathParent != null )
                n = n.pathParent;
        }
        tmpPath.Add( n );
        tmpPath.Reverse();

        //string v = "";
        //foreach ( Node d in tmpPath )
        //    v += " " + d.position;

        //Debug.Log( "auto path is " + v );
        return tmpPath;
    }
   
    public override IEnumerator execute()
    {
        if ( path != null && path.Count != 0 )
        {
            GameManager.DisableUnitSelection();
            //Debug.Log( "Executing!" );
            var vecPath = new Vector3[ path.Count ];

            for ( int i = 0 ; i < vecPath.Length ; i++ )
                vecPath[ i ] = path[ i ].position;

            GoSpline g = new GoSpline( vecPath , true );

            Board.instance.clearVirtualPosition( caster.transform.position );
            Board.instance.occupyVirtualPosition( vecPath[ vecPath.Length - 1 ] , caster.transform );

            //TODO: play move animation while tween is playing
            Go.to( caster.transform , moveSpeed , new TweenConfig().positionPath( g , false , LookAtType.None ).setEaseType( EaseType.QuartInOut ) );
            //TODO: play stop animation after tween finishes

            cleanup();
            caster.deselect();
            yield return new WaitForSeconds( moveSpeed );
            GameManager.EnableUnitSelection();
        }
        else
        {
            cleanup();
            caster.deselect();
            yield return null;
        }
    }

    public override void cancel()
    {
        cleanup();
        base.cancel();
    }

    void cleanup()
    {
        //SPEEDUP: Slow, store only tiles used?
        var allAbilityTiles = Transform.FindSceneObjectsOfType( typeof( Tile ) );
        foreach (Tile t in allAbilityTiles)
        {
            t.unsubscribe( this );
        }
        moveField.Clear();
        path.Clear();
        //Screen.showCursor = true;
    }

    public override void onMouseDownSubscribedTile(Tile tile)
    {
        base.onMouseDownSubscribedTile( tile );
        JobManager.instance.EnqueueJob( execute() );
    }

    public override void onMouseEnterSubscribedTile(Tile tile)
    {
        //Screen.showCursor = false;
        tryAddNodeToPath( tile.node );
    }

    public override void onMouseExitSubscribedTile(Tile tile)
    {
        //Screen.showCursor = true;
    }
    
    public override void onMouseEnterUnsubscribedTile(Tile tile)
    {
        miniCleanup();
    }
    
    void miniCleanup()
    {
        //begin mini cleanup, doesn'terrainInstance require rebuildilng the movefield.

        //SPEEDUP: Slow, store only tiles used?
        var allAbilityTiles = Transform.FindSceneObjectsOfType( typeof( Tile ) );
        foreach ( Tile t in allAbilityTiles )
        {
            t.removeHighlightObject();
            t.dehighlight();
        }
        path.Clear();
        //Screen.showCursor = true;
    }
    
}
