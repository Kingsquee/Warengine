using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Represents where Abilities can be executed.
/// </summary>
public class Tile : MonoBehaviour {

    public static GameObject tilePrefab;
    static Vector3 tileHeightOffset = new Vector3( 0f , 0.005f , 0f );

    public Node node;
    private List<Ability> _listeningAbilities = new List<Ability>();

    public List<string> debugAbilities = new List<string>();

    public Renderer tileRenderer;


    Color normalColor = Color.white;
    static Color highlightColor = Color.white;
    static Color multipleAbilitiesColor = Color.magenta;

    public Transform highlightObject;

    public static Tile previousFocusedTile;
    public delegate void MouseEnterTileHandler(Tile tile);
    public static event MouseEnterTileHandler onMouseEnterTile;

    public delegate void MouseExitTileHandler(Tile tile);
    public static event MouseExitTileHandler onMouseExitTile;

    /*
    public delegate void MouseClickTileHandler(Tile tile);
    public static event MouseClickTileHandler onMouseClickTile; */

    public static Tile Create(Node node)
    {
        if ( tilePrefab == null )
            tilePrefab = Resources.Load( "prefabs/Tile" ) as GameObject;

        GameObject g = GameObject.Instantiate( tilePrefab , node.position , Quaternion.identity ) as GameObject;
        Tile tile = g.GetComponent<Tile>();
        tile.node = node;
        node.tile = tile;
        tile.transform.position += tileHeightOffset;
        tile.transform.parent = Board.instance.transform;
        return tile;
    }

    #region Initialization

    void OnEnable()
    {
        if ( tileRenderer == null )
            tileRenderer = renderer;

        normalColor = tileRenderer.material.color;

        if ( _listeningAbilities.Count == 0 )
            deactivate();
    }

    #endregion

    #region Ability subscription functions

    public void subscribe(Ability ability)
    {
        if ( ability != null )
            activate();

        _listeningAbilities.Add( ability );

        if ( _listeningAbilities.Count > 1 )
        {
            normalColor = multipleAbilitiesColor;
            setTileColor(normalColor);
        }
        else
        {
            normalColor = ability.tileColor;
            setTileColor( normalColor );
        }

        debugAbilities.Add( ability.name );
    }

    public void unsubscribe(Ability ability)
    {
        if ( ability == null )
            return;

        if ( _listeningAbilities.Contains( ability ) )
        {
            _listeningAbilities.Remove( ability );
            debugAbilities.Remove( ability.name );
        }

        if ( _listeningAbilities.Count == 0 )
            deactivate();
    }

    public bool isAbilitySubscribed(Ability ability)
    {
        if ( _listeningAbilities.Contains( ability ) )
            return true;
        else
            return false;
    }

    #endregion

    #region Unity Events

    void OnMouseEnter()
    {
        if ( Program.mode == ProgramMode.Editor )
            return;

        if ( onMouseEnterTile != null )
            onMouseEnterTile( this );
        previousFocusedTile = this;
    }

    void OnMouseExit()
    {
        if ( Program.mode == ProgramMode.Editor )
            return;

        if ( onMouseExitTile != null )
            onMouseExitTile( this );
    }

    void OnMouseDown()
    {
        if ( Program.mode == ProgramMode.Editor )
            return;

        if ( _listeningAbilities.Count > 1 )
            displayAbiltySelectionWindow();
        else if ( _listeningAbilities.Count != 0 )
            _listeningAbilities[ 0 ].onMouseDownSubscribedTile( this );
        else 
        if ( node.obstruction )
        {
            Selectable b = node.obstruction.GetComponent<Selectable>();
            if ( b != null )
            {
                if ( GameManager.allowSelection )
                    b.select();
            }
        }
    }

    void displayAbiltySelectionWindow()
    {
        Debug.Log( "There's more than one sendingAbility listening to Tile! Accessing more than one sendingAbility is currently unimplemented." );
        throw new System.NotImplementedException();
    }
    #endregion

    #region Rendering functions

    private void setTileColor(Color color)
    {
        tileRenderer.material.color = color;
    }

    public void highlight()
    {
        setTileColor(highlightColor);
    }

    public void dehighlight()
    {
        setTileColor(normalColor);
    }

    public void setHighlightObject(Transform t)
    {
        removeHighlightObject();
        highlightObject = t;
        highlightObject.parent = transform;
    }

    public void removeHighlightObject()
    {
        if ( highlightObject != null )
            Destroy( highlightObject.gameObject );
    }
    #endregion

    public void activate()
    {
        tileRenderer.enabled = true;
    }

    public void deactivate()
    {
        tileRenderer.enabled = false;
        dehighlight();
        removeHighlightObject();
    }

    /*
    public virtual void kill()
    {
        node.tile = null;
        removeHighlightObject();
        Destroy( gameObject );
    }*/
}
