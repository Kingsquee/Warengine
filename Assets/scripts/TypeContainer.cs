using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Type = System.Type;


/*
 * 4/25/2013 4:42AM
 * GOOD MOOOORNING KINGSLEY!
 * This fine 4:42 AM I had the brilliant idea to make a generic static container class instead of all these copypasta'd WhaverTypeContainer classes. 
 * This might lead to some problems with initialization in the short-term, but that's only because our initialization is currently retarded. Populate() functions et all..
 * 
 * But yes, hopefully this seems as good an idea in the morning as it did this...morning. :)
 */

public class testGenericContainer
{
    void yetUntested()
    {
        //TypeContainer<SquadType>.add("yolo", new SquadType(etc
    }
}

public static class TypeContainer<T>
{
    private static Dictionary<Type, Dictionary<string, T>> container = new Dictionary<Type, Dictionary<string, T>>();

    /// <summary>
    /// Adds a property type to the cache list.
    /// </summary>
    /// <param name="type"></param>
    public static void add(string name, T type)
    {
        Type t = typeof( T );
        if (!container.ContainsKey( t ))
            container.Add( t, new Dictionary<string, T>() );

        if (container[t].ContainsKey( name ))
            return;
        container[t].Add( name, type );
    }

    /// <summary>
    /// Gets the data wrapper for the property type
    /// </summary>
    /// <param name="name">the name of the type of property to get.</param>
    /// <returns>the data wrapper for the type of property.</returns>
    public static T get(string name)
    {
        Type t = typeof( T );
        if (container.Count == 0)
            //PropertyTypeContainer.instance.Populate(); 
            throw new System.Exception( "Tried to get type " + name + ", but the table has not been populated yet." );

        if (!container.ContainsKey( t ))
            throw new System.Exception("Oh mah gawd its already there");

        T returningValue = default(T);
        container[t].TryGetValue( name, out returningValue );
        return returningValue;
    }

    /// <summary>
    /// Returns all values in container
    /// </summary>
    /// <returns></returns>
    public static T[] getAll()
    {
        Type t = typeof( T );
        T[] pts = new T[container[t].Values.Count];
        container[t].Values.CopyTo( pts, 0 );
        return pts;
    }
}
