using UnityEngine;
using System.Collections;

public enum ProgramMode { Game, Editor };

public static class Program
{

    private static ProgramMode _mode;
    public static ProgramMode mode
    {
        get { return _mode; }

        set
        {
            _mode = value;
            if (_mode == ProgramMode.Editor)
                EnableEditor();
            else
                DisableEditor();
        }
    }

    public static void DisableEditor()
    {
        EditorManager.instance.editorToggleObject.SetActive(false);
        EditorManager.instance.editorObjectListUI.SetActive(false);
    }

    public static void EnableEditor()
    {
        EditorManager.instance.editorToggleObject.SetActive(true);
        EditorManager.instance.editorObjectListUI.SetActive(true);
    }

    /// <summary>
    /// Toggles the program mode and returns the new mode.
    /// </summary>
    /// <returns>The new mode.</returns>
    public static ProgramMode ToggleMode()
    {
        if (_mode == ProgramMode.Editor) 
            mode = ProgramMode.Game; 
        else 
            mode = ProgramMode.Editor;
        return mode;
    }
}

public class EditorManager : MonoBehaviour {

    public GameObject editorToggleObject;
    public GameObject editorObjectListUI;

    void Start()
    {
        Program.mode = ProgramMode.Game;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Program.ToggleMode();
        }
	}

    private static EditorManager _instance = null;
    public static EditorManager instance
    {
        get
        {
            if (!_instance)
            {
                //check if a EditorManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(EditorManager)) as EditorManager;

                //nope, create a new one
                if (!_instance)
                {
                    var obj = new GameObject("EditorManager");
                    _instance = obj.AddComponent<EditorManager>();
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
