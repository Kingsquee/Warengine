using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static Selectable selected;
    public static HighlightCursor cursor;

    public static bool allowSelection { get { return _allowSelection; } }
    private static bool _allowSelection = true;

    string changelog;

    string getMostRecentChangelog()
    {
        string latest = "";

        string wholeChangelog = (Resources.Load( "changelog" ) as TextAsset).text;

        if ( wholeChangelog == null )
            return latest;

        int startPos = wholeChangelog.IndexOf( "[Build" );
        int endPos = wholeChangelog.IndexOf( "[Build" , startPos + 1 );
        //Debug.Log( "startpos is " + startPos );
        //Debug.Log( "endpos is " + endPos );

        if ( endPos > 0 )
            latest = wholeChangelog.Substring( startPos , endPos - startPos );
        else
            latest = wholeChangelog;

        return latest;
    }

    public static void DisableUnitSelection()
    {
        _allowSelection = false;
        cursor.OnDisableUnitSelection();
    }
    public static void EnableUnitSelection() 
    {

        _allowSelection = true;
        cursor.OnEnableUnitSelection();
    }
    void Awake()
    {
        cursor = Transform.FindObjectOfType( typeof( HighlightCursor ) ) as HighlightCursor;
        changelog = getMostRecentChangelog();
    }

    /*
    void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUILayout.Label( changelog );
    }
    */
	
    Ray ray;

    static GameManager _instance = null;
    public static GameManager instance
    {
        get
        {
            if ( !_instance )
            {
                //check if a GameManager is already available in the scene graph
                _instance = FindObjectOfType( typeof( GameManager ) ) as GameManager;

                //nope, create a new one
                if ( !_instance )
                {
                    var obj = new GameObject( "GameManager" );
                    _instance = obj.AddComponent<GameManager>();
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
