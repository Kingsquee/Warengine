using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Luna's shuddering moonquakes, I hate having to rewrite this.
//CCHAAAAAAARGE
public class PrimsField_Attack 
{
    int movementPoints;

    Node r;
    Node u;

    List<Node> G, Q, L;

    public PrimsField_Attack(Vector3 startPosition)
    {
        //Debug.Log( startPosition );

        //Debug.Log( Board.instance.nodes.ContainsKey( new Vector2( startPosition.x , startPosition.z ) ) );
            
        r = Board.instance.getNode(startPosition);

    }

    public List<Node> GetField( List<Node> nodes , int movementPoints )
    {
        this.movementPoints = movementPoints;

        G = new List<Node>( nodes );
        Q = new List<Node>( G );
        L = new List<Node>();

        //Debug.Log( "G's count is " + G.Count );
        //Debug.Log( "Q's count is " + Q.Count );

        foreach ( Node n in G )
            n.ResetPathfindingValues(); 

        r.weightOfCheapestEdge = 0;
        r.CalculatePathCost( null );

        while ( Q.Count != 0 )
        {
            u = GetLowestCostNode();
            Q.Remove( u );
            u.known = true;
            if ( u.totalPathCost > movementPoints )
                continue;

            L.Add( u );
            CheckEdges( u );
        }

        //foreach ( Node node in L )
            //Debug.Log( node.position );
        return L;
    }

    void CheckEdges( Node n )
    {
        //Debug.Log( " Checking edge for " + u.position );
        foreach ( Edge edge in n.edges )
        {
            if ( edge == null || edge.childNode == null)
                return;

            if ( !edge.childNode.known && edge.weight < edge.childNode.weightOfCheapestEdge )
            {
                //If it's not obstructed, ignore.
                if ( !edge.childNode.obstruction)
                    continue;

                //if it's obstructed but isn'terrainInstance a selectableEntity, ignore.
                Selectable e = edge.childNode.obstruction.GetComponent<Selectable>();
                if ( e == null )
                    continue;

                //FIXME: attacks probably shouldn'tile require calculating terrain movement weights. butwhatever it works.
                edge.childNode.pathParent = edge.parentNode;
                edge.childNode.weightOfCheapestEdge = edge.weight;
                edge.childNode.CalculatePathCost( edge );
                 
            }
        }
    }

    Node GetLowestCostNode()
    {
        Node n = Q[0];
        int minimumWeight = int.MaxValue;
        if ( Q.Count > 0 )
        {
            foreach ( Node sideToCheck in Q )
            {
                if ( sideToCheck.totalPathCost < minimumWeight )
                {
                    minimumWeight = sideToCheck.totalPathCost;
                    n = sideToCheck;
                }
            }
        }
        //Debug.Log("Getting " + node.position + ", with a cost of " + node.weightOfCheapestEdge);
        
        return n;
    }
}
