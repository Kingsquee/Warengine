//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Luna's shuddering moonquakes, I hate having to rewrite this.
//CCHAAAAAAARGE
public class WalkableField 
{
    int movementPoints;

    Node r;
    Node u;

    List<Node> G, Q, L;

    public List<Node> GetField( Node startNode, List<Node> nodes, int movementPoints )
    {
        r = startNode;

        this.movementPoints = movementPoints;

        G = nodes;
        //G = new List<Node>( nodes );
        Q = new List<Node>( G );
        L = new List<Node>();

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

        return L;
    }

    void CheckEdges( Node n )
    {
        foreach (Edge edge in n.edges)
        {
            if (MoveAbility.isEdgeTraversable(n, edge))
            {
                edge.childNode.pathParent = edge.parentNode;
                edge.childNode.weightOfCheapestEdge = edge.weight;
                edge.childNode.CalculatePathCost(edge);
            }
        }
    }

    //SPEEDUP: Performance can be improved by using a fibonacci heap
    Node GetLowestCostNode()
    {
        Node n = Q[0];
        int minimumWeight = int.MaxValue;
        if ( Q.Count > 0 )
        {
            foreach ( Node nodeToCheck in Q )
            {
                if ( nodeToCheck.totalPathCost < minimumWeight )
                {
                    minimumWeight = nodeToCheck.totalPathCost;
                    n = nodeToCheck;
                }
            }
        }
        return n;
    }
}
