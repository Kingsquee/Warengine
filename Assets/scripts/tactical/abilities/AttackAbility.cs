
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//gah so little time TWO HOURS LEFT
public class AttackAbility : Ability
{
    GameObject AbilityTile;
    List<Node> attackField = new List<Node>();
    public Transform target;

    public AttackAbility()
    {
        name = "Attack";
        tileColor = Color.red;
    }

    public override void setup(Selectable caster)
    {
        base.setup( caster );
        createTiles();
    }

    void createTiles()
    {
        Debug.Log( "Creating attack tiles" );

        attackField = Board.instance.getNodesAdjacent( caster.transform.position );

        foreach ( Node node in attackField )
        {
            if ( !node.obstruction )
                continue;

            //HACK: design a better check than maxvalue?
            int damage = int.MaxValue;
            damage = CombatDamageLookupTable.Lookup( caster.name, node.obstruction.name );
            if (damage == int.MaxValue)
                continue;

            var tmp = node.obstruction;
            if ( tmp != null )
                node.tile.subscribe( this );
        }
    }

    public override void onMouseDownSubscribedTile(Tile tile)
    {
        base.onMouseDownSubscribedTile( tile );
        Debug.Log( tile.node.obstruction.name );
        target = tile.node.obstruction;
        JobManager.instance.EnqueueJob( execute() );
    }

    public override IEnumerator execute()
    {
        Debug.LogWarning( "WARNING: BULLSHIT PLACEHOLDER COMBAT HAPPENING BETWEEN " + caster.name + " , " + target.name );

        int damage = CombatDamageLookupTable.Lookup(caster.name, target.name);

        var targetSquad = target.GetComponent<SquadInstance>();
        targetSquad.health -= damage;

        cleanup();
        caster.deselect();

        if (targetSquad.health > 0)
            Debug.Log( "Finished Attacking. " + target.name + " health is now " + targetSquad.health + ". Dealt " + damage + " damage." );
        else
            Debug.Log( "BAYUM. ULTRA KILLLL!" );
        yield return null; //new WaitForSeconds( 1f );
    }

    public override void cancel()
    {
        cleanup();
        base.cancel();
    }

    void cleanup()
    {
        Debug.Log( "Cleaning up attack tiles" );

        //SPEEDUP: Slow, store only tiles used?
        var allAbilityTiles = Transform.FindSceneObjectsOfType( typeof( Tile ) );
        foreach ( Tile t in allAbilityTiles )
        {
            t.unsubscribe( this );
        }
        attackField.Clear();
    }

}

