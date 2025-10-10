using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Potions;

public static class GameObjectFinder
{
    public static IReadOnlyList<GameObject> FindByName(string name, bool includeInactive = true)
    {
        var results = new List<GameObject>(64);
        var active = SceneManager.GetActiveScene();
        if (active.IsValid() && active.isLoaded)
            FindByName(active, name, includeInactive, results);
        return results;
    }

    public static IReadOnlyList<GameObject> FindByNameAllLoadedScenes(string name, bool includeInactive = true)
    {
        var results = new List<GameObject>(128);
        var count = SceneManager.sceneCount;
        for (int i = 0; i < count; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.IsValid() && scene.isLoaded)
                FindByName(scene, name, includeInactive, results);
        }
        return results;
    }

    public static IReadOnlyList<GameObject> FindByName(Scene scene, string name, bool includeInactive = true)
    {
        var results = new List<GameObject>(64);
        FindByName(scene, name, includeInactive, results);
        return results;
    }

    static void FindByName(Scene scene, string name, bool includeInactive, List<GameObject> results)
    {
        var roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
            Collect(roots[i].transform, name, includeInactive, results);
    }

    static void Collect(Transform t, string name, bool includeInactive, List<GameObject> results)
    {
        var go = t.gameObject;

        if (!includeInactive && !go.activeInHierarchy)
            return;

        if (go.name == name)
            results.Add(go);

        var childCount = t.childCount;
        for (int i = 0; i < childCount; i++)
            Collect(t.GetChild(i), name, includeInactive, results);
    }
}