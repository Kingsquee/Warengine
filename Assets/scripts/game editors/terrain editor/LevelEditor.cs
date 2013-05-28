using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour
{
    //public UnitType chosenUnitType;
    //public UIGrid unitUIGrid;

    public TerrainType chosenTerrainType;
    public UIGrid terrainUIGrid;

    public PropertyType chosenPropertyType;
    public UIGrid propertyUIGrid;

    public SquadType chosenSquadType;
    public UIGrid squadUIGrid;

    public string DebugTerrainType;

    private enum LevelEditorMode { Terrain, Property, Squad }
    private LevelEditorMode mode = LevelEditorMode.Terrain;

    Ray ray;
    RaycastHit cursorInfo;
    Tile tile;

    public void PopulateTypeContainers()
    {
        //load all the things
        //HACK: These loaders are hard coded for now. They should load from IO for mod support.
        LoadTerrainTypes();
        LoadPropertyTypes();
        LoadSquadTypes();

        chosenTerrainType = TypeContainer<TerrainType>.get( "Water" );
        DebugTerrainType = chosenTerrainType.gameplayData.name;
    }

    public void LoadSquadTypes()
    {
        new SquadType( new SquadStaticData( "placeholder squad", 4, SquadMovementType.Tread, Altitude.land, 5, 30, 4, 0, 1, 10, new List<Ability>() { new AttackAbility(), new MoveAbility() } ) );

        PopulateSquadUIGrid( TypeContainer<SquadType>.getAll() );
    }

    public void LoadPropertyTypes()
    {
        new PropertyType( "property_redteam_city", new TerrainStaticData( "RedTeam City", 3, Altitude.land, Capturable.True ) );
        new PropertyType( "property_redteam_factory", new TerrainStaticData( "RedTeam Factory", 3, Altitude.land, Capturable.True ) );
        //placeholder

        //After I've been populated, alert the terrainEditor that the UI needs to be updated.
        PopulatePropertyUIGrid( TypeContainer<PropertyType>.getAll() );
    }

    public void LoadTerrainTypes()
    {
        TerrainType terra = new TerrainType( new TerrainStaticData( "Plain", 1, Altitude.land, Capturable.False ) );
        TerrainType water = new TerrainType( new TerrainStaticData( "Water", 1, Altitude.sea, Capturable.False ) );

        //After I've been populated, alert the terrainEditor that the UI needs to be updated.
        PopulateTerrainUIGrid( TypeContainer<TerrainType>.getAll() );
    }

    public void PopulateSquadUIGrid(SquadType[] squadTypes)
    {
        foreach (Transform t in squadUIGrid.transform)
        {
            Destroy( t.gameObject );
        }

        foreach (SquadType s in squadTypes)
        {
            GameObject entry = Instantiate( Resources.Load( "prefabs/gui/Entry" ) ) as GameObject;
            Vector3 scale = entry.transform.localScale;

            entry.transform.parent = squadUIGrid.transform;
            entry.transform.localScale = scale;

            entry.name = s.staticData.name;

            UIEventListener.Get( entry ).onClick += OnChooseSquad;
        }
    }

    private GameObject CreateUIEntry(Transform parent, string name)
    {
        GameObject entry = Instantiate( Resources.Load( "prefabs/gui/Entry" ) ) as GameObject;
        Vector3 scale = entry.transform.localScale;

        entry.transform.parent = parent;
        entry.transform.localScale = scale;

        entry.name = name;

        return entry;
    }

    public void PopulateTerrainUIGrid(TerrainType[] terrainTypes)
    {
        foreach (Transform t in terrainUIGrid.transform)
        {
            Destroy( t.gameObject );
        }

        foreach (TerrainType t in terrainTypes)
        {
            //create entry prefab
            GameObject entry = Instantiate( Resources.Load( "prefabs/gui/Entry" ) ) as GameObject;
            Vector3 scale = entry.transform.localScale;

            entry.transform.parent = terrainUIGrid.transform;
            entry.transform.localScale = scale;

            //give its tooltip the terrainTypes[i].name
            entry.name = t.gameplayData.name;

            //reference it to the terrainType
            UIEventListener.Get( entry ).onClick += OnChooseTerrain;
        }

        terrainUIGrid.Reposition();
        EditorManager.instance.editorObjectListUI.GetComponent<UITable>().Reposition();
    }

    public void PopulatePropertyUIGrid(PropertyType[] propertyTypes)
    {
        foreach (Transform t in propertyUIGrid.transform)
        {
            Destroy( t.gameObject );
        }

        foreach (PropertyType p in propertyTypes)
        {
            //create entry prefab
            GameObject entry = Instantiate( Resources.Load( "prefabs/gui/Entry" ) ) as GameObject;
            Vector3 scale = entry.transform.localScale;

            entry.transform.parent = propertyUIGrid.transform;
            entry.transform.localScale = scale;

            //give its tooltip the terrainTypes[i].name
            entry.name = p.gameplayData.name;

            //reference it to the terrainType
            UIEventListener.Get( entry ).onClick += OnChooseProperty;
        }

        propertyUIGrid.Reposition();
        EditorManager.instance.editorObjectListUI.GetComponent<UITable>().Reposition();
    }

    private void OnChooseProperty(GameObject go)
    {
        string text = go.name;
        if (text == null) return;
        chosenPropertyType = TypeContainer<PropertyType>.get( text );
        mode = LevelEditorMode.Property;
    }

    private void OnChooseTerrain(GameObject go)
    {
        string text = go.name; //go.GetComponent<UITooltip>().text.text;
        if (text == null) return;
        chosenTerrainType = TypeContainer<TerrainType>.get( text );
        mode = LevelEditorMode.Terrain;
    }

    private void OnChooseSquad(GameObject go)
    {
        string text = go.name;
        if (text == null) return;
        chosenSquadType = TypeContainer<SquadType>.get( text );
        mode = LevelEditorMode.Squad;
    }



    void Update()
    {
        ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        Physics.Raycast( ray, out cursorInfo, Mathf.Infinity );

        switch (mode)
        {
            case LevelEditorMode.Terrain: TerrainEditorUpdate(); return;
            case LevelEditorMode.Property: PropertyEditorUpdate(); return;
            case LevelEditorMode.Squad: SquadEditorUpdate(); return;
        }

    }

    void TerrainEditorUpdate()
    {
        if (!Input.GetMouseButton( 0 ))
            return;

        if (cursorInfo.transform != null && UICamera.hoveredObject == null)
        {
            tile = cursorInfo.transform.GetComponent<Tile>();

            if (tile != null)
            {
                if (tile.node.terrain == null)
                    TerrainInstance.Create( tile.node, chosenTerrainType ); //this should never be necessary.
                else if (tile.node.terrain.type != chosenTerrainType)
                    TerrainInstance.Replace( tile.node.terrain, chosenTerrainType );
            }
        }
    }

    private void PropertyEditorUpdate()
    {
        if (!Input.GetMouseButton( 0 ))
            return;

        if (cursorInfo.transform != null && UICamera.hoveredObject == null)
        {
            tile = cursorInfo.transform.GetComponent<Tile>();

            if (tile != null)
            {
                if (chosenPropertyType.gameplayData.altitude != tile.node.terrain.staticData.altitude)
                    return;

                if (tile.node.terrain.property == null)
                    PropertyInstance.Create( tile.node.terrain, chosenPropertyType );
                else if (tile.node.terrain.property.type != chosenPropertyType)
                    PropertyInstance.Replace( tile.node.terrain.property, chosenPropertyType );
            }
        }
    }

    private void SquadEditorUpdate()
    {
        if (!Input.GetMouseButton( 0 ))
            return;

        if (cursorInfo.transform != null && UICamera.hoveredObject == null)
        {
            tile = cursorInfo.transform.GetComponent<Tile>();

            if (tile != null)
            {
                //TODO: Allow multiple altitudes OR just make a damn lookup table.
                if (chosenSquadType.staticData.altitude != tile.node.terrain.staticData.altitude)
                    return;

                if (tile.node.obstruction == null)
                    SquadInstance.Create( tile.node, chosenSquadType );
                else if (tile.node.obstruction.GetComponent<SquadInstance>().type != chosenSquadType)
                    SquadInstance.Replace( tile.node.obstruction.GetComponent<SquadInstance>(), chosenSquadType );
            }
        }
    }





    private static LevelEditor _instance = null;
    public static LevelEditor instance
    {
        get
        {
            if (!_instance)
            {
                //check if a TerrainEditor is already available in the scene graph
                _instance = FindObjectOfType( typeof( LevelEditor ) ) as LevelEditor;

                //nope, create a new one
                if (!_instance)
                {
                    var obj = new GameObject( "TerrainEditor" );
                    _instance = obj.AddComponent<LevelEditor>();
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
