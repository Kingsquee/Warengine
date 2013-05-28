using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

    public void select()
    {
        if ( GameManager.selected )
        {
            if ( GameManager.selected == this )
                return;
            else
                GameManager.selected.deselect();
        }

        //JobManager.instance.Clear();
        GameManager.selected = this;
        GameManager.cursor.Focus( transform );
        Debug.Log( name + " selected!" );

        BroadcastMessage( "onSelect" , SendMessageOptions.DontRequireReceiver );

    }

    public void deselect()
    {
        GameManager.selected = null;
        GameManager.cursor.Defocus();

        //Debug.Log( name + " deselected!" );

        BroadcastMessage( "onDeselect" , SendMessageOptions.DontRequireReceiver );
    }
}
