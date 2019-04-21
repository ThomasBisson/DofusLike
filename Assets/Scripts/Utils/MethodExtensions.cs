using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MethodExtensions
{
    public static string RemoveQuotes(this string Value)
    {
        return Value.Replace("\"", "");
    }

    public static string RemoveHeader(this string Value)
    {
        int i = Value.IndexOf('{');
        string s = Value.Substring(i, Value.Length-i-1);
        return s;
    }
}
