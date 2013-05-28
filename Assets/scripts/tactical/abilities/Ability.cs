using UnityEngine;
using System.Collections;

/// <summary>
///  Dictates data and process for abilityContainer.
/// </summary>
public abstract class Ability 
{
    /// <summary>
    ///  The Selectable that cast the sendingAbility.
    /// </summary>
    public Selectable caster;

    /// <summary>
    ///  The container the instantiated sendingAbility belongs to.
    /// </summary>
    //public AbilityContainer abilityContainer;

    /// <summary>
    ///  The user-facing name of the sendingAbility. 
    /// </summary>
    public string name;

    /// <summary>
    ///  The color that tiles of this ability will have. 
    /// </summary>
    public Color tileColor;

    public Ability()
    {
        Tile.onMouseEnterTile += onMouseEnterTile;
        Tile.onMouseExitTile += onMouseExitTile;
    }
    /*
    public virtual void initData(Selectable caster , AbilityContainer abilityContainer)
    {
        this.caster = caster;
        this.abilityContainer = abilityContainer;
        Tile.onMouseEnterTile += onMouseEnterTile;
        Tile.onMouseExitTile += onMouseExitTile;
    }
    */

    /*
    public virtual void kill()
    {
        Tile.onMouseEnterTile -= onMouseEnterTile;
        Tile.onMouseExitTile -= onMouseExitTile;
    }
    */

    protected virtual void onMouseEnterTile(Tile tile)
    {
        if ( GameManager.selected != caster )
            return;

        if ( tile.isAbilitySubscribed( this ) )
        {
            //Debug.Log( caster.name + "'s " + name + " claims you entered a subscribed tile. Type is " + this.GetType().Name );
            onMouseEnterSubscribedTile( tile );
        }
        else
        {
            //Debug.Log( caster.name + "'s " + name + " claims you entered an unsubscribed tile. Type is " + this.GetType().Name );
            onMouseEnterUnsubscribedTile( tile );
        }
    }

    void onMouseExitTile(Tile tile)
    {
        if ( GameManager.selected != caster )
            return;

        if ( tile.isAbilitySubscribed( this ) )
        {
            onMouseExitSubscribedTile( tile );
        }
        else
        {
            onMouseExitUnsubscribedTile( tile );
        }
    }

    public virtual void setup( Selectable caster )
    {
        this.caster = caster;
    }

    public virtual IEnumerator execute()
    {
        yield return null;
    }

    public virtual void cancel()
    {
        caster = null;
    }

    public virtual void update()
    {

    }
    /*
    protected void disableOtherAbilities()
    {
        if ( abilityContainer == null )
        {
            Debug.Log( caster.gameObject.name + "'s " + name + " sendingAbility tried to disable other abilityContainer, but its AbilityConatainer was null!" );
            return;
        }

        abilityContainer.cancelAllExcept( this );
    }
    */
    
    #region Tile events

    public virtual void onMouseEnterSubscribedTile(Tile tile ) { }
    public virtual void onMouseExitSubscribedTile(Tile tile ) { }
    public virtual void onMouseDownSubscribedTile(Tile tile) { }

    public virtual void onMouseEnterUnsubscribedTile(Tile tile ) { }
    public virtual void onMouseExitUnsubscribedTile(Tile tile ) { }
    //public virtual void onMouseDownUnsubscribedTile(Tile tile) { }

    #endregion
    
}
