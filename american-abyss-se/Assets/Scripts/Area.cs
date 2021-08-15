using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Area
{
    FOO,
    NORTHWEST,
    NORTH_CENTRAL,
    MIDWEST,
    NORTHEAST,
    SOUTHEAST, 
    SOUTH_CENTRAL,
    SOUTHWEST
}

public static class NameExtensionsArea
{
    public static string GetString(this Area name)
    {
        switch (name)
        {
            case Area.FOO:
                return "null";
            case Area.MIDWEST:
                return "Midwest";
            case Area.NORTHEAST:
                return "Northeast";
            case Area.NORTHWEST:
                return "Northwest";
            case Area.SOUTHEAST:
                return "Southeast";
            case Area.SOUTHWEST:
                return "Southwest";
            case Area.NORTH_CENTRAL:
                return "North Central";
            case Area.SOUTH_CENTRAL:
                return "South Central";
            default:
                return "error";
        }
    }
}

