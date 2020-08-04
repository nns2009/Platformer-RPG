using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static Vector2 Flat(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 Vol(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
    public static Vector3 DropY(this Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }
}
