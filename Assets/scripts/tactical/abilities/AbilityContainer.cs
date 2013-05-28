using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  Containers and manages Abilities.
/// </summary>
public class AbilityContainer {

    public Selectable caster;

    private List<Ability> _abilities = new List<Ability>();

    public void add(Ability ability)
    {
        if ( !_abilities.Contains( ability ) )
            _abilities.Add( ability );
        //ability.initData( caster , this );
        Debug.Log( "Added " + ability.name + " to " + caster.name + "'s abilitycontainer" );
    }

    public void remove(Ability ability)
    {
        if ( _abilities.Contains( ability ) )
            _abilities.Remove( ability );
    }

    public void clear()
    {
        _abilities.Clear();
    }

    public void setupAll()
    {
        foreach ( Ability ability in _abilities )
        {
            ability.setup( caster );
        }
    }

    public void setupAllExcept(Ability chosenAbility)
    {
        if ( !_abilities.Contains( chosenAbility ) )
        {
            Debug.LogError( caster.gameObject.name + "'s abilityContainer did not contain + " + chosenAbility.name + ". Starting other abilities anyway..." );
            return;
        }
        foreach ( Ability ability in _abilities )
        {
            if ( ability != chosenAbility )
                ability.setup( caster );
        }
    }

    public void cancelAll()
    {
        foreach ( Ability ability in _abilities )
        {
            ability.cancel();
        }
    }

    public void cancelAllExcept(Ability chosenAbility)
    {
        if ( !_abilities.Contains( chosenAbility ) )
        {
            Debug.LogError( caster.gameObject.name + "'s abilityContainer did not contain + " + chosenAbility.name + ". Cancelling other abilities anyway..." );
            return;
        }
        foreach ( Ability ability in _abilities )
        {
            if (ability != chosenAbility)
                ability.cancel();
        }
    }
    /*
    public void sendEvent_onMouseEnterUnsubscribedTile_toAllExcept(Ability sendingAbility, Tile tile)
    {
        foreach (Ability ability in _abilities )
        {
            if ( ability != sendingAbility )
                ability.onMouseEnterUnsubscribedTile( tile );
        }
    }

    public void sendEvent_onMouseOverUnsubscribedTile_toAllExcept(Ability sendingAbility , Tile tile)
    {
        foreach ( Ability ability in _abilities )
        {
            if ( ability != sendingAbility )
                ability.onMouseOverUnsubscribedTile( tile );
        }
    }

    public void sendEvent_onMouseExitUnsubscribedTile_toAllExcept(Ability sendingAbility , Tile tile)
    {
        foreach ( Ability ability in _abilities )
        {
            if ( ability != sendingAbility )
                ability.onMouseExitUnsubscribedTile( tile );
        }
    }

    public void sendEvent_onMouseDownUnsubscribedTile_toAllExcept(Ability sendingAbility , Tile tile)
    {
        foreach ( Ability ability in _abilities )
        {
            if ( ability != sendingAbility )
                ability.onMouseDownUnsubscribedTile( tile );
        }
    }
    */
    public void onExecuteAbility(Ability executedAbility)
    {
        foreach ( Ability ability in _abilities )
        {
            if ( ability != executedAbility )
                ability.cancel();
        }
    }
    /*
    void Update()
    {
        if ( GameManager.selected != caster )
            return;

        foreach ( Ability ability in _abilities )
        {
            ability.update();
        }
    }
     */
}