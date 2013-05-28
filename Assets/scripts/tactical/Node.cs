using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a location on the board, consisting of Terrain, Pathfinding, and Visibility data.
/// </summary>
public class Node
{

    //Instance Data:

    //Terrain Data
    public Tile tile;
    public TerrainInstance terrain;
    public Transform obstruction;
    public Vector3 position;

    
    //Pathfinding Data
    public List<Edge> edges = new List<Edge>();
    public Node pathParent;
    public int costModifier = 1; //TODO: This will be calculated by unitType's costModifier for this node's terrain data. 
    public int totalPathCost = int.MaxValue;
    public int weightOfCheapestEdge;
    public bool known;

    //Visibility Data


    public Node(TerrainType terrainType , Vector3 position)
    {
        this.position = position;
        Tile.Create( this );
        TerrainInstance.Create( this , terrainType );
    }

    public void ResetPathfindingValues()
    {
        weightOfCheapestEdge = int.MaxValue;
        totalPathCost = int.MaxValue;
        pathParent = null;
        known = false;
    }

    public void CalculatePathCost(Edge e)
    {
        if ( e == null || e.childNode == null )
        {
            totalPathCost = 0;
            return;
        }
        totalPathCost = e.parentNode.totalPathCost + e.weight;
    }

    public void DebugDraw()
    {
        if ( pathParent == null )
            return;

        Debug.DrawLine( position , pathParent.position , Color.green );
        pathParent.DebugDraw();
    }

    public void AssignEdges()
    {
        edges.Clear();
        foreach (Node n in Board.instance.getNodesAdjacent( position ))
            AddEdge( n );
    }

    private void AddEdge(Node n)
    {
        var e = new Edge();
        e.parentNode = this;
        e.childNode = n;
        e.weight = 1;
        edges.Add( e );
        //Debug.Log( "Created edge between " + e.parentNode.position + " and " + e.childNode.position );

    }
}

public class Edge
{
    public Node childNode;
    public Node parentNode;
    public int weight = int.MaxValue;
    public int costModifier = 0;
}
