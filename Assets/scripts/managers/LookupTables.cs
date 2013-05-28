using UnityEngine;
using System.Collections.Generic;

public static class CombatDamageLookupTable
{
    private static Dictionary<string, Dictionary<string, int>> table = new Dictionary<string, Dictionary<string, int>>();

    /// <summary>
    /// Looks up the damage for the two entries.
    /// </summary>
    /// <param name="attacker">The attacking squad</param>
    /// <param name="defender">the defending squad</param>
    /// <returns>The damage attacker should deal to defender</returns>
    public static int Lookup(string attacker, string defender)
    {

        if (attacker == null || defender == null)
        {
            if (attacker == null) Debug.LogError( "Combat damage lookup table attacker parameter cannot be null" );
            if (defender == null) Debug.LogError( "Combat damage lookup table defender parameter cannot be null" );
        }

        //HACK: HARD CODED COMBAT LOKOUP SUCCOOS OH GAWDS
        Add( attacker, defender, 100 );


        int r = 0;

#if UNITY_EDITOR
        try
        {
            r = table[attacker][defender];
        }
        catch (System.Exception)
        {
            Debug.LogError( "Error accessing combat damage lookup table for " + attacker + " and " + defender + "." );
        }
#else
        r = table[attacker][defender];
#endif

        return r;
    }

    public static void Add(string entityA, string entityB, int value)
    {
        try
        {
            if (table.ContainsKey( entityA ))
            {
                if (table[entityA].ContainsKey( entityB ))
                    return;
                table[entityA].Add( entityB, value );
            }
            else
            {
                table.Add( entityA, new Dictionary<string, int>() { { entityB, value } } );
            }
        }
        catch (System.Exception)
        {
            Debug.LogError( "Error assigning value to combat damage lookup table for " + entityA + " and " + entityB + "." );

            throw;
        }

    }
}



/// <summary>
/// The idea of this lookup table is to see what kind of animation the attacker should play when two entities fight. 
/// e.g. Does a Mech squad attack a Tank with its Machinegun attack or its Rocket Launcher attack?
/// </summary>
public static class CombatAttackAnimationLookupTable
{
    private static Dictionary<string, Dictionary<string, int>> table = new Dictionary<string, Dictionary<string, int>>();

    /// <summary>
    /// Looks up the animation to play when two entries engage in combat.
    /// </summary>
    /// <param name="attacker">The attacking squad</param>
    /// <param name="defender">the defending squad</param>
    /// <returns>The animation the attacker should deal to defender</returns>
    public static int Lookup(string attacker, string defender)
    {
        if (attacker == null || defender == null)
        {
            if (attacker == null) Debug.LogError( "Combat animation lookup table attacker parameter cannot be null" );
            if (defender == null) Debug.LogError( "Combat animation lookup table defender parameter cannot be null" );
        }

        Debug.LogWarning( attacker + " , " + defender );

        int r = 0;
        try
        {
            r = table[attacker][defender];
        }
        catch (System.Exception)
        {
            Debug.LogError( "Error accessing combat animation lookup table for " + attacker + " and " + defender + "." );
            throw;
        }

        return r;
    }

    public static void Set(string attacker, string defender, int value)
    {
        try
        {
            table[attacker][defender] = value;
        }
        catch (System.Exception)
        {
            Debug.LogError( "Error assigning value to combat animation lookup table for " + attacker + " and " + defender + "." );

            throw;
        }
    }
}

//TODO: Should there be a CombatDefenseAnimationLookupTable?

/// <summary>
/// Lookup table for the movement cost of a squadType on a terrainType.
/// </summary>
public static class MovementCostLookuptable
{
    private static Dictionary<string, Dictionary<string, int>> table = new Dictionary<string, Dictionary<string, int>>();

    /// <summary>
    /// Looks up the movement cost for a squadType on a terrainType.
    /// </summary>
    /// <param name="squadType">The squadType</param>
    /// <param name="terrainType">the terrainType</param>
    /// <returns>The movement cost of the squadType moving over the terrainType</returns>
    public static int Lookup(string squadType, string terrainType)
    {
        if (squadType == null || terrainType == null)
        {
            if (squadType == null) Debug.LogError( "Movement cost lookup table attacker parameter cannot be null" );
            if (terrainType == null) Debug.LogError( "Movement cost lookup table defender parameter cannot be null" );
        }

        Debug.LogWarning( squadType + " , " + terrainType );

        int r = 0;
        try
        {
            r = table[squadType][terrainType];
        }
        catch (System.Exception)
        {
            Debug.LogError( "Error accessing movement cost lookup table for " + squadType + " and " + terrainType + "." );
            throw;
        }

        return r;
    }

    public static void Set(string squadType, string terrainType, int value)
    {
        try
        {
            table[squadType][terrainType] = value;
        }
        catch (System.Exception)
        {
            Debug.LogError( "Error assigning value to movement cost lookup table for " + squadType + " and " + terrainType + "." );

            throw;
        }
    }
}
