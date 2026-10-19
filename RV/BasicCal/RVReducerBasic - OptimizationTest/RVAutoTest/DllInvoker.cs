using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace RVAutoTest
{
    /// <summary>
    /// RVReducerBasic.dll 的 P/Invoke 封装。
    /// 直接调用 DLL 计算函数，绕过 UI，获取精确计算结果。
    /// </summary>
    public static class DllInvoker
    {
        // ═══════════════════ 齿轮推荐 ═══════════════════
        // Indata[0..8]: 固定部件, 总传动比, 输出力矩, 输出转速, 目标应力, 设计寿命, 使用系数, 许用接触应力, 行星轮个数
        // Indata[9..11]: 摆线轮最小齿宽, 摆线轮最大齿宽, 是否启用约束
        // OutData: 推荐的法向模数, 压力角, 螺旋角, 中心距, ... (约 2000 个 double)
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RecommondPara(ref double inData, ref double outData);

        // ═══════════════════ 轴推荐 ═══════════════════
        // OutData[0]=推荐轴直径, OutData[1]=推荐轴长度
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RecommondShaftPara(
            ref int inputIntIndexDividIndex,
            ref int inputIntData,
            ref double inputDoubleData,
            ref double outputData,
            ref double outData);

        // ═══════════════════ 自定义轴承寿命 ═══════════════════
        // InData[0..36]: 37 元素输入数组
        // OutData[10]=L10h 寿命（小时）, OutData[4]=S0 静强度安全系数
        // 返回: 0=成功, 非0=失败
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CalCustomBearingLife(ref double inData, ref double outData);

        // ═══════════════════ 标准轴承寿命 ═══════════════════
        // Indata[0..8]: 轴承类型, 转速, 轴向力, 径向力, Cr, C0r, e, Y, Y0
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CalBearingLife(ref double inData, ref double outData);

        // ═══════════════════ 轴承推荐 ═══════════════════
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int RecommondBearingLife(
            ref double inData, ref double outData,
            IntPtr[] outCodes, int maxCount, int maxLen);

        // ═══════════════════ 渐开线齿轮副计算 ═══════════════════
        // InDivideIndex: 34 元素分割索引
        // InInputData: 174 元素输入
        // OutData: 284 元素输出 (142×2 齿轮)
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CalInvGearStress2(ref int inDivideIndex, ref double inInputData, ref double outData);

        // ═══════════════════ 摆线针轮副计算 ═══════════════════
        // InDividIndex: 5 元素分割索引
        // InData: 60 元素输入 (针轮 20 + 摆线轮 2×20)
        // OutData: 9 元素输出
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CalCycloidGearStressByObj(ref int inDividIndex, ref double inData, ref double outData);

        // ═══════════════════ 曲轴系统计算 ═══════════════════
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CalCrankShaftSystem2(
            ref int inputIntIndexDividIndex,
            ref int inputIntData,
            ref double inputDoubleData,
            ref double outputData);

        // 读取曲轴应力/位置
        [DllImport("RVReducerBasic.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetForceAndForceLocation(
            ref int coordinateNumber, ref double outputDataX, ref double outputDataY);

        // ═══════════════════ 便捷封装 ═══════════════════

        /// <summary>齿轮推荐计算</summary>
        public static double[] CallRecommondPara(double[] input)
        {
            if (input.Length < 12)
                throw new ArgumentException("齿轮推荐输入数组长度至少为 12");

            double[] output = new double[2000];
            RecommondPara(ref input[0], ref output[0]);
            return output;
        }

        /// <summary>自定义轴承寿命计算</summary>
        public static double[] CallCalCustomBearingLife(double[] input)
        {
            if (input.Length < 37)
                throw new ArgumentException("自定义轴承输入数组长度至少为 37");

            double[] output = new double[20];
            int result = CalCustomBearingLife(ref input[0], ref output[0]);
            if (result != 0)
                throw new InvalidOperationException($"自定义轴承寿命计算失败，返回码: {result}");
            return output;
        }

        /// <summary>标准轴承寿命计算</summary>
        public static double[] CallCalBearingLife(double[] input)
        {
            if (input.Length < 9)
                throw new ArgumentException("标准轴承输入数组长度至少为 9");

            double[] output = new double[20];
            CalBearingLife(ref input[0], ref output[0]);
            return output;
        }

        /// <summary>渐开线齿轮副计算（174 输入 → 284 输出）</summary>
        public static double[] CallCalInvGearStress2(int[] divideIndex, double[] input)
        {
            if (divideIndex.Length < 34)
                throw new ArgumentException("渐开线分割索引数组长度至少为 34");
            if (input.Length < 174)
                throw new ArgumentException("渐开线输入数组长度至少为 174");

            double[] output = new double[284];
            CalInvGearStress2(ref divideIndex[0], ref input[0], ref output[0]);
            return output;
        }

        /// <summary>摆线针轮副计算（60 输入 → 9 输出）</summary>
        public static double[] CallCalCycloidGearStressByObj(int[] divideIndex, double[] input)
        {
            if (divideIndex.Length < 5)
                throw new ArgumentException("摆线分割索引数组长度至少为 5");
            if (input.Length < 60)
                throw new ArgumentException("摆线输入数组长度至少为 60");

            double[] output = new double[9];
            CalCycloidGearStressByObj(ref divideIndex[0], ref input[0], ref output[0]);
            return output;
        }

        /// <summary>曲轴系统计算</summary>
        public static void CallCalCrankShaftSystem2(int[] mainDivideIndex, int[] intData, double[] doubleData)
        {
            CalCrankShaftSystem2(ref mainDivideIndex[0], ref intData[0], ref doubleData[0], ref doubleData[0]);
        }

        /// <summary>读取曲轴应力数据（Y[0..n]=弯曲正应力, Y[n..2n]=剪切应力, X=节点坐标）</summary>
        public static (double[] x, double[] bending, double[] shear) CallGetForceAndForceLocation()
        {
            int coordinateNumber = 0;
            double[] x = new double[200];
            double[] y = new double[200];
            GetForceAndForceLocation(ref coordinateNumber, ref x[0], ref y[0]);

            int n = coordinateNumber;
            double[] bending = new double[n];
            double[] shear = new double[n];
            Array.Copy(y, 0, bending, 0, n);
            Array.Copy(y, n, shear, 0, n);
            var loc = new double[n];
            Array.Copy(x, 0, loc, 0, n);
            return (loc, bending, shear);
        }
    }

    /// <summary>
    /// 角度转换工具：DLL 内部用弧度，C++ 的 Degree = 0.01745329252。
    /// </summary>
    public static class AngleUtil
    {
        public const double Degree = 0.01745329252;   // 与 C++ #define Degree 一致

        /// <summary>角度(°) → 弧度(rad)</summary>
        public static double ToRad(double degrees) => degrees * Degree;

        /// <summary>弧度(rad) → 角度(°)</summary>
        public static double ToDeg(double radians) => radians / Degree;
    }
}
