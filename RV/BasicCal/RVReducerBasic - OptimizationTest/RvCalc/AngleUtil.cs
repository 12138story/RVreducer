namespace RvCalc;

/// <summary>
/// 角度转换工具：DLL 内部用弧度，C++ 的 Degree = 0.017453292519943295。
/// </summary>
public static class AngleUtil
{
    /// <summary>度 → 弧度系数，与 C++ #define Degree 一致</summary>
    public const double Degree = 0.01745329252;

    /// <summary>角度(°) → 弧度(rad)</summary>
    public static double ToRad(double degrees) => degrees * Degree;

    /// <summary>弧度(rad) → 角度(°)</summary>
    public static double ToDeg(double radians) => radians / Degree;
}
