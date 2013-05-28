using UnityEngine;
using System.Collections;

public static class ComponentExtensions
{
    /// <summary>
    /// Tries to get the component on this gameobject: if fails, adds and returns it.
    /// </summary>
    /// <typeparam name="T">The type of the component we're trying to get.</typeparam>
    /// <param name="script">Extension method parameter.</param>
    /// <returns>The instance of the component.</returns>
    public static T AddGetComponent<T>(this Component script) where T : Component
    {
        T requiredComponent = script.GetComponent<T>();
        if (requiredComponent == null)
            requiredComponent = script.gameObject.AddComponent<T>();
        return requiredComponent;
    }
}

public static class RigidbodyExtensions
{
    /// <summary>
    /// Wrapper class for rigidbody.rotation.SetLookRotation.
    /// Creates a rotation that looks along forward with the the head upwards along upwards.
    /// </summary>
    /// <param name="rigidbody">Extension method param.</param>
    /// <param name="target">Vector3 to look at along the forward axis.</param>
    /// <param name="up">Vector3 to look at along the up axis.</param>
    /// <returns></returns>
    public static Rigidbody LookAt(this Rigidbody rigidbody, Vector3 target , Vector3 up)
    {
        Quaternion q = rigidbody.rotation;
        q.SetLookRotation( target , up );
        rigidbody.rotation = q;
        return rigidbody;
    }

    /// <summary>
    /// Wrapper class for rigidbody.rotation.SetLookRotation.
    /// Creates a rotation that looks along forward with the the head upwards along upwards.
    /// </summary>
    /// <param name="rigidbody">Extension method param.</param>
    /// <param name="rigidbodyRotation">Rigidbody rotation, in case you've cached it for efficiency.</param>
    /// <param name="target">Vector3 to look at along the forward axis.</param>
    /// <param name="up">Vector3 to look at along the up axis.</param>
    /// <returns></returns>
    public static Rigidbody LookAt(this Rigidbody rigidbody , Quaternion rigidbodyRotation, Vector3 target , Vector3 up)
    {
        rigidbodyRotation.SetLookRotation( target , up );
        rigidbody.rotation = rigidbodyRotation;
        return rigidbody;
    }
}




public static class FloatExtensions
{
    /// <summary>
    /// Returns a Vector3 with this float's value as the X axis
    /// </summary>
    public static Vector3 ToVector3X(this float mFloat)
    {
        return new Vector3( mFloat , 0f , 0f );
    }

    /// <summary>
    /// Returns a Vector3 with this float's value as the Y axis
    /// </summary>
    public static Vector3 ToVector3Y(this float mFloat)
    {
        return new Vector3( 0f , mFloat , 0f );
    }

    /// <summary>
    /// Returns a Vector3 with this float's value as the Z axis
    /// </summary>
    public static Vector3 ToVector3Z(this float mFloat)
    {
        return new Vector3( 0f , 0f , mFloat );
    }
}




public static class Vector3Extensions
{
    /// <summary>
    /// Returns this Vector3's X value as a Vector3
    /// </summary>
    public static Vector3 X(this Vector3 mVec)
    {
        return new Vector3( mVec.x , 0f , 0f );
    }

    /// <summary>
    /// Returns this Vector3's Y value as a Vector3
    /// </summary>
    public static Vector3 Y(this Vector3 mVec)
    {
        return new Vector3( 0f , mVec.y , 0f );
    }

    /// <summary>
    /// Returns this Vector3's Z value as a Vector3
    /// </summary>
    public static Vector3 Z(this Vector3 mVec)
    {
        return new Vector3( 0f , 0f , mVec.z );
    }

    /// <summary>
    /// Returns this Vector3's X and Y values as a Vector3
    /// </summary>
    public static Vector3 XY(this Vector3 mVec)
    {
        return new Vector3( mVec.x , mVec.y , 0f );
    }

    /// <summary>
    /// Returns this Vector3's Y and Z values as a Vector3
    /// </summary>
    public static Vector3 YZ(this Vector3 mVec)
    {
        return new Vector3( 0f , mVec.y , mVec.z );
    }

    /// <summary>
    /// Returns this Vector3's X and Z values as a Vector3
    /// </summary>
    public static Vector3 XZ(this Vector3 mVec)
    {
        return new Vector3( mVec.x , 0f , mVec.z );
    }
}




public static class GameObjectExtensions
{
    /// <summary>
    /// Tries to get the component on this gameobject: if fails, adds and returns it.
    /// </summary>
    /// <typeparam name="T">The type of the component we're trying to get.</typeparam>
    /// <param name="script">Extension method parameter.</param>
    /// <returns>The instance of the component.</returns>
    public static T AddGetComponent<T>(this GameObject gameObject) where T : Component
    {
        T requiredComponent = gameObject.GetComponent<T>();
        if ( requiredComponent == null )
            requiredComponent = gameObject.AddComponent<T>();
        return requiredComponent;
    }

    public static IEnumerator IteratorDestroy(GameObject target)
    {
        Debug.Log( " Killing " + target.name );
        GameObject.Destroy( target );
        yield return null;
    }
}


 public static class Mathfx 
 {

     /// <summary>
     /// Rounds a Vector3's elements to the nearest integers
     /// </summary>
     /// <param name="vec"></param>
     /// <returns></returns>
    public static Vector3 Round(Vector3 vec )
    {
        return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z)); 
    }


     /// <summary>
     /// Compares if the X and Z values of the two Vector3 parameters are equal.
     /// </summary>
     /// <param name="a"></param>
     /// <param name="b"></param>
     /// <returns></returns>
    public static bool CompareXZ(Vector3 a , Vector3 b)
    {
        if ( a.x == b.x && a.z == b.z )
            return true;
        else
            return false;
    }
}
