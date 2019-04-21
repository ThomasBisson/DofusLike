
using UnityEngine;

public class MathsUtils
{

    #region CONVERTIONS
    public static long MapUlongToLong(ulong ulongValue)
    {
        return unchecked((long)ulongValue + long.MinValue);
    }

    public static ulong MapLongToUlong(long longValue)
    {
        return unchecked((ulong)(longValue - long.MinValue));
    }
    #endregion

    #region PERCENT_CALCUL
    public static float PercentValueFromAnotherValue(float value, float total)
    {
        return ((value * 100f) / total);
    }

    public static int PercentValueFromAnotherValue(int value, int total)
    {
        return ((value * 100) / total);
    }

    public static uint PercentValueFromAnotherValue(uint value, uint total)
    {
        return ((value * 100) / total);
    }

    public static long PercentValueFromAnotherValue(long value, long total)
    {
        return ((value * 100) / total);
    }

    public static ulong PercentValueFromAnotherValue(ulong value, ulong total)
    {
        return ((value * 100) / total);
    }

    public static int PercentValue(int percent, int total)
    {
        return (percent / 100) * total;
    }

    public static ulong PercentValue(ulong percent, ulong total)
    {
        float percentDivebyHundred = (percent / 100f);
        return System.Convert.ToUInt64(percentDivebyHundred * total);
    }
    #endregion

    #region ARRAY_CALCUL

    public static float CircleDistance(Vector2 a, Vector2 b)
    {
        //Manhattan distance
        return (Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y));
    }

    #endregion

    #region IA

    /// <summary>
    /// The standard sigmoid function.
    /// </summary>
    /// <param name="xValue">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static double SigmoidFunction(double xValue)
    {
        if (xValue > 10) return 1.0;
        else if (xValue < -10) return 0.0;
        else return 1.0 / (1.0 + System.Math.Exp(-xValue));
    }

    /// <summary>
    /// The standard TanH function.
    /// </summary>
    /// <param name="xValue">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static double TanHFunction(double xValue)
    {
        if (xValue > 10) return 1.0;
        else if (xValue < -10) return -1.0;
        else return System.Math.Tanh(xValue);
    }

    /// <summary>
    /// The SoftSign function as proposed by Xavier Glorot and Yoshua Bengio (2010): 
    /// "Understanding the difficulty of training deep feedforward neural networks".
    /// </summary>
    /// <param name="xValue">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static double SoftSignFunction(double xValue)
    {
        return xValue / (1 + System.Math.Abs(xValue));
    }

    #endregion
}