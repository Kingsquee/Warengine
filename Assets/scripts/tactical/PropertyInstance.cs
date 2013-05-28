using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all information regarding a terrain property's type.
/// </summary>
public class PropertyType
{
    /// <summary>
    /// The data representation of this property type
    /// </summary>
    public readonly TerrainStaticData gameplayData;

    /// <summary>
    /// The visual representation of this property type
    /// </summary>
    public GameObject prefab;

    public PropertyType(string assetName, TerrainStaticData data)
    {
        gameplayData = data;

        prefab = loadPrefabByName( assetName );

        TypeContainer<PropertyType>.add( gameplayData.name, this );
    }

    private GameObject loadPrefabByName(string assetName)
    {
        //load stuff
        var path = "prefabs/terrainproperties/" + assetName;
        //Debug.LogError( path );
        GameObject prefab = Resources.Load( path ) as GameObject;
        //Debug.LogError( prefab );
        return prefab;
    }
}

/// <summary>
/// Contains all information regarding a terrain property's instance.
/// </summary>
public class PropertyInstance : MonoBehaviour {

    public TerrainInstance terrainInstance;
    public PropertyType type;
   
    /// <summary>
    /// Creates a new Propertyinstance.
    /// </summary>
    /// <param name="terrainInstance">the terrain instance this property will be attached to</param>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    public static PropertyInstance Create(TerrainInstance terrainInstance, PropertyType propertyType)
    {
        if (terrainInstance.property != null)
        {
            throw new System.Exception( "PropertyInstance.Create() was called on a terrainInstance with a non-null property. Use PropertyInstance.Replace() instead." );
        }

        //Debug.Log( terrainType.prefabs.block );
        GameObject go = GameObject.Instantiate(
            propertyType.prefab,
            terrainInstance.node.position,
            Quaternion.identity ) as GameObject;

        PropertyInstance propertyInstance = go.AddComponent<PropertyInstance>();
        propertyInstance.terrainInstance = terrainInstance;
        terrainInstance.property = propertyInstance;
        propertyInstance.type = propertyType;
        propertyInstance.transform.parent = terrainInstance.transform;

        //nobody cares
        //go.name = propertyType.prefab.name;

        return propertyInstance;
    }

    public static void Replace(PropertyInstance propertyInstance, PropertyType newType)
    {
        TerrainInstance t = propertyInstance.terrainInstance;
        Destroy( t.property.gameObject );
        t.property = null;
        Create( t, newType );
    }
    
}
