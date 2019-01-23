using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtension {
    const string Ellipses = "..";
    public static string Truncate(this string value, int length) {
        if (value == null) {
            return null;
        }
        return length < 0 ? "" : value.Substring(0, Math.Min(value.Length, length-Ellipses.Length))+ Ellipses;
    }
}
