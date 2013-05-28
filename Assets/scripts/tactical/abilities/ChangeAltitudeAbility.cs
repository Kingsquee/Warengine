/*
using UnityEngine;
using System.Collections;

public class ChangeAltitudeAbility : Ability {

    public float moveSpeed = 1f;

    public override IEnumerator execute()
    {
        GameManager.DisableUnitSelection();
        if ( Board.height > 1 )
            JobManager.instance.EnqueueJob( changeAltitude() );
        yield return new WaitForSeconds( moveSpeed );
        GameManager.EnableUnitSelection();
    }

    IEnumerator changeAltitude()
    {
        Vector3 whereToGo = caster.transform.position;

        if ( Mathf.RoundToInt( caster.transform.position.y ) < 1 )
            whereToGo.y = 1;
        else
            whereToGo.y = 0;

        if ( Board.instance.GetNode( whereToGo ).obstructed )
        {
            yield return null;
        }
        else
        {
            Board.instance.clearVirtualGrid( caster.transform.position );
            Board.instance.occupyVirtualGrid( whereToGo , caster.transform );

            Go.to( caster.transform , moveSpeed , new TweenConfig().position( whereToGo ).setEaseType( EaseType.QuartInOut ) );
            yield return new WaitForSeconds( moveSpeed );
        }
    }
}
 */
