using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Modified from Prime31's https://github.com/prime31/P31UnityAddOns/blob/master/Scripts/Misc/JobManager.cs

//JobManager is just a proxy object so we have a launcher for the coroutines. Also gives access to a globally accessable JobSequencer.
//Could have been done by adding a JobSequencer to the GameManager object, but this felt less spaghetticoady.
public class JobManager : MonoBehaviour
{
    //only one JobManager can exist. We use a singleton pattern to enforce this.
    static JobManager _instance = null;
    public static JobManager instance
    {
        get
        {
            if ( !_instance )
            {
                //check if a JobManager is already available in the scene graph
                _instance = FindObjectOfType( typeof( JobManager ) ) as JobManager;

                //nope, create a new one
                if ( !_instance )
                {
                    var obj = new GameObject( "JobManager" );
                    _instance = obj.AddComponent<JobManager>();
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

    private JobSequencer _jobSequencer = new JobSequencer();

    public Job EnqueueJob(IEnumerator e)
    {
        return _jobSequencer.EnqueueJob( e );
    }

    public void Clear()
    {
        _jobSequencer.Clear();
    }
}

//Simple wrapper class to add sequential coroutines.
public class JobSequencer
{
    private Job _currentJob;

    public Job EnqueueJob( IEnumerator e )
    {
        if ( _currentJob == null )
        {
            _currentJob = new Job( e , true );
            Debug.Log( "Added new job" );
        }
        else
        {
            _currentJob.addChildJob( new Job( e, false ) );
            Debug.Log( "Added new childjob" );

            if ( _currentJob.running == false )
            {
                _currentJob.start();
                Debug.Log( "Restarted main job" );
            }
        }
        return _currentJob;
    }
    
    public void Clear()
    {
        if ( _currentJob == null )
            return;

        _currentJob.kill();
        _currentJob = null;
    }
    
}

public class Job
{
    public event System.Action<bool> jobComplete;

    private bool _running;
    public bool running { get { return _running; } }

    private bool _paused;
    public bool paused { get { return _paused; } }

    private Queue<Job> _childJobQueue;
    private IEnumerator _coroutine;
    private bool _jobWasKilled;
    private Job _currentChildJob;

    #region constructors

    public Job( IEnumerator coroutine )
        : this( coroutine , true )
    { }

    public Job( IEnumerator coroutine , bool shouldStart )
    {
        _coroutine = coroutine;

        if ( shouldStart )
            start();
    }

    #endregion

    #region static Job makers

    public static Job make( IEnumerator coroutine )
    {
        return new Job( coroutine );
    }

    public static Job make( IEnumerator coroutine , bool shouldStart )
    {
        return new Job( coroutine , shouldStart );
    }

    #endregion

    #region Public API

    public Job addChildJob( IEnumerator coroutine )
    {
        var j = new Job( coroutine , false );
        addChildJob( j );
        return j;
    }

    public void addChildJob( Job childJob )
    {
        if ( _childJobQueue == null )
            _childJobQueue = new Queue<Job>();
        _childJobQueue.Enqueue( childJob );
    }

    public void removeChildJob( Job childJob )
    {
        if ( _childJobQueue.Contains( childJob ) )
        {
            var childStack = new Queue<Job>( _childJobQueue.Count - 1 );
            var allCurrentChildren = _childJobQueue.ToArray();
            System.Array.Reverse( allCurrentChildren );

            for ( var i = 0 ; i < allCurrentChildren.Length ; i++ )
            {
                var j = allCurrentChildren[i];
                if ( j != childJob )
                    childStack.Enqueue( j );
            }

            //assign the new stack
            _childJobQueue = childStack;
        }
    }

    public void start()
    {
        _running = true;
        JobManager.instance.StartCoroutine( doWork() );
    }

    public IEnumerator startAsCoroutine()
    {
        _running = true;
        yield return JobManager.instance.StartCoroutine( doWork() );
    }

    public void pause()
    {
        _paused = true;
        syncChildJob();
    }

    public void unpause()
    {
        _paused = false;
        syncChildJob();
    }

    public void kill()
    {
        _jobWasKilled = true;
        _running = false;
        _paused = false;
        syncChildJob();
    }

    public void kill( float delayInSeconds )
    {
        var delay = (int) ( delayInSeconds * 1000 );
        new System.Threading.Timer( obj =>
        {
            lock ( this )
            {
                kill();
            }
        } , null , delay , System.Threading.Timeout.Infinite );
    }

    #endregion

    private IEnumerator doWork()
    {
        // null out the first run through in case we start paused
        yield return null;

        while ( _running )
        {
            if ( _paused )
            {
                yield return null;
            }
            else
            {
                // run the next iteration and stop if we are done
                if ( _coroutine.MoveNext() )
                {
                    yield return _coroutine.Current;
                }
                else
                {
                    // run our child jobs if we have any
                    if ( _childJobQueue != null )
                        yield return JobManager.instance.StartCoroutine( runChildJobs() );
                    _running = false;
                }
            }
        }

        // fire off a complete event
        if ( jobComplete != null )
            jobComplete( _jobWasKilled );
    }

    private IEnumerator runChildJobs()
    {
        if ( _childJobQueue != null && _childJobQueue.Count > 0 )
        {
            while ( _running && _childJobQueue.Count > 0 )
            {
                _currentChildJob = _childJobQueue.Dequeue();
                syncChildJob();
                yield return JobManager.instance.StartCoroutine( _currentChildJob.startAsCoroutine() );
            }
        }
    }

    private void syncChildJob()
    {
        if ( _currentChildJob == null )
            return;
        _currentChildJob._jobWasKilled = _jobWasKilled;
        _currentChildJob._running = _running;
        _currentChildJob._paused = _paused;
    }
}