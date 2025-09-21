using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalHelper 
{
    public static string GenerateGlobalID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
