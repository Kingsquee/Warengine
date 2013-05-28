using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 4/25/2013 4:49AM
 * This is the SpriteType class. Instances of this class serve as goto references for all shared data for a type of sprite.
 * Something is telling me the material ref might require some further work, but I'm so tired I have a stomachache, so do that when you can, please.
 */

public enum PlaybackDirection { Forward, Rewind }

public struct SpriteAnimationFrame
{
    public int x;
    public int y;
    public float duration;

    public SpriteAnimationFrame (int x, int y, float duration)
	{
        this.x = x;
        this.y = y;
        this.duration = duration;
	}
}

public class SpriteType
{
    /// <summary>
    /// Hard-coded spriteSize of 64x64.
    /// </summary>
    static public int spriteSize = 64;
    public readonly string name;
    public readonly Material material = new Material( Shader.Find( "Unlit/Transparet" ) );
    public Dictionary<string, SpriteAnimationFrame[]> animations = new Dictionary<string, SpriteAnimationFrame[]>(1);

    public SpriteType(string name, Texture2D atlas)
    {
        this.name = name;
        material.mainTexture = atlas;
    }

    public void addAnimation(string name, SpriteAnimationFrame[] animation)
    {
        animations.Add( name, animation );
    }

    public void removeAnimation(string name)
    {
        animations.Remove( name );
    }

    public SpriteAnimationFrame[] getAnimation(string name)
    {
        if (!animations.ContainsKey( name ))
            throw new System.Exception( "SpriteType " + this.name + " does not contain an animation called " + name );
        SpriteAnimationFrame[] r;
        animations.TryGetValue( name, out r );
        return r;
    }
}

[RequireComponent(typeof(Renderer))]
public class SpriteInstance : MonoBehaviour {

    /// <summary>
    /// spriteType reference
    /// </summary>
    private SpriteType spriteType;

    /// <summary>
    /// Caching the material ref on this gameobject
    /// </summary>
    private Material mMaterial;

    /// <summary>
    /// Is an animation currently playing?
    /// </summary>
    private bool isPlayingAnimation = false;

    /// <summary>
    /// Frame number of the current animation
    /// </summary>
    private int frameNum = 0;


    //TODO: check if setting renderer.material for each instance causes performance drops. If so, how to improve?
    public void setSpriteType(SpriteType spriteType)
    {
        this.spriteType = spriteType;
        renderer.material = spriteType.material;
        mMaterial = renderer.material;
    }

    /// <summary>
    /// Stops the current playing animation (if any) and plays the designated animation.
    /// </summary>
    /// <param name="name">The name of the animation to play</param>
    /// <param name="direction">Should the animation be played forward or backward?</param>
    /// <param name="fps">Speed at which the animation should be played</param>
    /// <returns></returns>
    private IEnumerator playAnimation(string name, PlaybackDirection direction, float fps = 1f)
    {
        stopAnimation();
        SpriteAnimationFrame[] frames;
        try
        {
            frames = spriteType.getAnimation( name );
        }
        catch (System.Exception)
        {
            Debug.LogError( gameObject.name + " could not load sprite animation " + name );
            throw;
        }

        float currentTime = 0;
        int dir = (direction == PlaybackDirection.Forward) ? dir = 1 : dir = -1;

        while (true)
        {
            if (!isPlayingAnimation)
                break;

            currentTime += Time.deltaTime;
            if (currentTime < (fps * frames[frameNum].duration))
                continue;

            mMaterial.mainTextureOffset = new Vector2( frames[frameNum].x * SpriteType.spriteSize, frames[frameNum].y * SpriteType.spriteSize);

            //Increment frame count
            frameNum += dir;
            if (frameNum >= frames.Length) //I think Length starts from 1, not 0. will confirm in morning, want to sleep now...
                frameNum = 0;
                
            currentTime = 0;
            yield return null;
        }
        
    }

    /*
    void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps)
    {

        // Calculate index
        int index = (int)(Time.time * fps);
        // Repeat when exhausting all cells
        index = index % totalCells;

        // Size of every cell
        float sizeX = 1.0f / colCount;
        float sizeY = 1.0f / rowCount;
        Vector2 size = new Vector2( sizeX, sizeY );

        // split into horizontal and vertical index
        var uIndex = index % colCount;
        var vIndex = index / colCount;

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        float offsetX = (uIndex + colNumber) * size.x;
        float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
        Vector2 offset = new Vector2( offsetX, offsetY );

        renderer.material.SetTextureOffset( "_MainTex", offset );
        renderer.material.SetTextureScale( "_MainTex", size );
    }
    */
    public IEnumerator stopAnimationEnumerator()
    {
        stopAnimation();
        yield return null;
    }

    public void stopAnimation()
    {
        isPlayingAnimation = false;
        frameNum = 0;
    }

    public IEnumerator pauseAnimationEnumerator()
    {
        pauseAnimation();
        yield return null;
    }

    public void pauseAnimation()
    {
        isPlayingAnimation = false;
    }
}
