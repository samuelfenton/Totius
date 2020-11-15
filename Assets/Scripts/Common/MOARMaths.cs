using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MOARMaths : MonoBehaviour
{
    /// <summary>
    /// Get the sqr distance of a single vector
    /// Faster then using magnitude 
    /// </summary>
    /// <param name="p_lhs">first vector</param>
    /// <returns>sqr distance</returns>
    public static float SqrMagnitude(Vector3 p_val)
    {
        return p_val.x * p_val.x + p_val.y * p_val.y + p_val.z * p_val.z;
    }

    /// <summary>
    /// Get the sqr distance between two vectors
    /// Faster then using magnitude
    /// </summary>
    /// <param name="p_lhs">first vector</param>
    /// <param name="p_rhs">second vectos</param>
    /// <returns>sqr distance</returns>
    public static float SqrDistance(Vector3 p_lhs, Vector3 p_rhs)
    {
        return (p_rhs.x - p_lhs.x) * (p_rhs.x - p_lhs.x) + (p_rhs.y - p_lhs.y) * (p_rhs.y - p_lhs.y) + (p_rhs.z - p_lhs.z) * (p_rhs.z - p_lhs.z);
    }

    /// <summary>
    /// Used to determine just how "positive" a vector is
    /// That is, does it travel thorugh the positive quadrant, and its magnitude.
    /// Done by comparing its allignemnt to Vector(1,1,1) and its magnitude.
    /// </summary>
    /// <param name="p_vector">Vector to calculate</param>
    /// <returns>Range from Negitivie Infinity->Inifinity</returns>
    public static float GetPositiveAlignment(Vector3 p_vector)
    {
        float alignment = Vector3.Dot(Vector3.one, p_vector);
        return p_vector.magnitude * alignment;
    }

    /// <summary>
    /// Snap towards an increment
    /// </summary>
    /// <param name="p_originalValue">Value to use for sanpping</param>
    /// <param name="p_increment">Increments to snap towards</param>
    /// <returns>Snapped value</returns>
    public static float SnapTowardsIncrement(float p_originalValue, float p_increment)
    {
        float snappedVal = (Mathf.FloorToInt(p_originalValue / p_increment)) * p_increment;

        if (p_originalValue < 0.0f) //Due to how floor works, negative will always round down one extra
            snappedVal += p_increment;

        float leftover = p_originalValue - snappedVal;

        if(Mathf.Abs(leftover) > (p_increment/2.0f))
        {
            snappedVal += p_originalValue >= 0.0f ? p_increment : -p_increment; //For negative, add upwatds
        }
        return snappedVal;
    }

    /// <summary>
    /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
    /// </summary>
    public static bool DoesSceneExist(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Get a signed Vector3 of angles
    /// Found here "https://answers.unity.com/questions/1693936/how-do-i-rotate-using-a-negative-angle-valuehow-to.html"
    /// </summary>
    /// <param name="angles">Orginal angle</param>
    /// <returns>Angle using signs</returns>
    public static Vector3 GetSignedEulerAngles(Vector3 angles)
    {
        Vector3 signedAngles = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            signedAngles[i] = (angles[i] + 180f) % 360f - 180f;
        }
        return signedAngles;
    }
}
