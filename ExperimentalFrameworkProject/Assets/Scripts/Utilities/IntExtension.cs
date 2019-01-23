using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntExtension {
    public static bool IsWithin(this int value, int minimum, int maximum) {
        return value >= minimum && value <= maximum;
    }
}
