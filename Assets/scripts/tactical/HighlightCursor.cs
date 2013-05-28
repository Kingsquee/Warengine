using UnityEngine;
using System.Collections;

public class HighlightCursor : MonoBehaviour {

    bool focused = false;
    bool animating = false;
    float animateSpeed = 0.4f;
    public HighlightCorner[] corners = new HighlightCorner[ 4 ];

    void Start()
    {
        Tile.onMouseEnterTile += onMouseEnterTile;
        Animate();
    }
    //Need a -= in OnDisable?

    public void Focus(Transform target)
    {

        transform.parent = target;
        transform.localPosition = new Vector3( 0f , 0.001f , 0f );
        focused = true;
    }

    public void Defocus()
    {
        transform.parent = null;
        //transform.localPosition = Vector3.zero;     
        focused = false;        
    }

    public void OnDisableUnitSelection()
    {
        foreach ( HighlightCorner c in corners )
            if ( c.gameObject.activeInHierarchy == true )
                c.gameObject.SetActive( false );
    }

    public void OnEnableUnitSelection()
    {
        foreach ( HighlightCorner c in corners )
            if ( c.gameObject.activeInHierarchy == false )
                c.gameObject.SetActive( true );
    }

    void onMouseEnterTile(Tile tile)
    {
        transform.position = Mathfx.Round( tile.node.position );
    }
    /*
    void Update()
    {
        if ( focused == true )
            return;

        prevPosition = transform.position;
        transform.position = Mathfx.Round( GameManager.cursorInfo.point );
        
        if ( prevPosition == transform.position )
            return;

    }*/

    void Animate()
    {
        if ( animating )
            return;

        foreach ( HighlightCorner c in corners )
        {
            if ( c.tween != null )
                continue;
            TweenConfig tc = new TweenConfig().scale( new Vector3( 0.6f , 0.75f , 0.6f ) ).setEaseType( EaseType.CircIn ).setIterations( -1 );//.setDelay( animateSpeed * 0.5f );
            tc.loopType = LoopType.PingPong;
            if ( c.tween != null )
                c.tween.destroy();
            c.tween = Go.to( c.transform , animateSpeed , tc );
        }
        animating = true;
    }

    void StopAnimate()
    {
        foreach ( HighlightCorner c in corners )
        {
            if ( c.tween == null )
                continue;
            //c.tween.complete();
            //c.tween.rewind();
            c.tween.destroy();
            c.transform.localScale = new Vector3( 0.75f , 0.75f , 0.75f );
        }
        animating = false;
    }
     
}
