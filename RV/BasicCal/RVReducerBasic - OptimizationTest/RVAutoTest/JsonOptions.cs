using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RVAutoTest
{
    /// <summary>
    /// 共享 JSON 序列化选项。
    /// 使用 UnsafeRelaxedJsonEscaping 让中文等非 ASCII 字符直接输出（而非 \uXXXX 转义），
    /// 使齿轮推荐字段名（中文）在 JSON 文件中可读。
    /// </summary>
    public static class JsonOptions
    {
        /// <summary>缩进 + 中文直接输出 + 允许 NaN/Infinity 命名浮点字面量</summary>
        /// <remarks>
        /// NumberHandling.AllowNamedFloatingPointLiterals 使 DLL 输出中的 NaN/±Infinity
        /// （如直齿轮 β=0 时轴向模数[mx]=∞）能正常序列化为 "NaN"/"Infinity"，避免抛异常。
        /// </remarks>
        public static readonly JsonSerializerOptions Pretty = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
        };
    }
}
