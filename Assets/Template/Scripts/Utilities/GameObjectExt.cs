using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExt
{
    public static void SetLayer(this GameObject gameObject, int layer, bool setChildren = false)
    {
        gameObject.layer = layer;
        if(!setChildren)
            return;
        for (int i = 0; i < gameObject.transform.childCount; i++)
            gameObject.transform.GetChild(i).gameObject.SetLayer(layer, setChildren);
    }
}
