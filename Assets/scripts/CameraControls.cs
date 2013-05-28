using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    //References
    Transform mTransform;
    JobSequencer mJobSequencer;

    public bool acceptingInput = true;

    //Rotation variables
    public float moveSpeed = 50f;
    const float normalTime = 0.4f;
    const float heldTime = 0.3f;
    int numOfRotationCalls = 0;

    //Position variables
    float clampedX;
    float clampedZ;


    void Awake()
    {
        mTransform = transform;
        mJobSequencer = new JobSequencer();
    }

	// Update is called once per frame
	void Update () {
        if ( !acceptingInput )
            return;

        moveUpdate();
        rotateUpdate();
	}

    void rotateUpdate()
    {

        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (numOfRotationCalls >= 2)
                    return;
                else
                {
                    ++numOfRotationCalls;
                    //JobManager.instance.EnqueueJob( Rotate( 90f , normalTime ));
                    mJobSequencer.EnqueueJob(Rotate(90f, normalTime));
                }
            }
            else
            {
                if (numOfRotationCalls >= 1)
                    return;
                else
                {
                    ++numOfRotationCalls;
                    //JobManager.instance.EnqueueJob( Rotate( 90f , heldTime ) );
                    mJobSequencer.EnqueueJob(Rotate(90f, heldTime));
                }
            }
        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (numOfRotationCalls >= 2)
                    return;
                else
                {
                    ++numOfRotationCalls;
                    //JobManager.instance.EnqueueJob( Rotate( -90f , normalTime ) );
                    mJobSequencer.EnqueueJob(Rotate(-90f, normalTime));
                }
            }
            else
            {
                if (numOfRotationCalls >= 1)
                    return;
                else
                {
                    ++numOfRotationCalls;
                    //JobManager.instance.EnqueueJob( Rotate( -90f,  heldTime ) );
                    mJobSequencer.EnqueueJob(Rotate(-90f, heldTime));
                }
            }
        }
    }

    void moveUpdate()
    {
        //TODO: Seems to cause jitter?
        Vector3 inputVec = new Vector3( Input.GetAxis( "Horizontal" ) * moveSpeed * Time.deltaTime, 0, Input.GetAxis( "Vertical" ) * moveSpeed * Time.deltaTime );
        //TODO: Mouse edge movement rather than wasd

        Vector3 newPos = (transform.rotation * inputVec) + transform.position;
        clampedX = Mathf.Clamp( newPos.x, 0, Board.width );
        clampedZ = Mathf.Clamp( newPos.z, 0, Board.length );

        newPos = new Vector3( clampedX, 0, clampedZ );

        transform.position = newPos;
    }

    /*
    void OnGUI()
    {
        GUILayout.Label( numOfRotationCalls.ToString() );
    }*/

    public IEnumerator Rotate(float degrees, float time)
    {
        Go.to( transform , time , new TweenConfig().rotation( transform.eulerAngles + new Vector3( 0f , degrees , 0f ) ).setEaseType( EaseType.QuartOut ) );
        yield return new WaitForSeconds( time );
        --numOfRotationCalls;
        if ( numOfRotationCalls < 0 )
            numOfRotationCalls = 0;
            
    }
}
