using System.Text.Json.Serialization;

namespace RVAutoTest.Models
{
    public class IsoExample1Params
    {
        [JsonPropertyName("input")]
        public IsoExample1Input Input { get; set; } = new();

        [JsonPropertyName("expectedOutput")]
        public IsoExpectedOutput ExpectedOutput { get; set; } = new();
    }

    public class IsoExample1Input
    {
        [JsonPropertyName("geometry")] public IsoGeometry Geometry { get; set; } = new();
        [JsonPropertyName("quality")] public IsoQuality Quality { get; set; } = new();
        [JsonPropertyName("material")] public IsoMaterial Material { get; set; } = new();
        [JsonPropertyName("application")] public IsoApplication Application { get; set; } = new();
        [JsonPropertyName("load")] public IsoLoad Load { get; set; } = new();
    }

    public class IsoGeometry
    {
        [JsonPropertyName("z1")] public double? z1 { get; set; }
        [JsonPropertyName("z2")] public double? z2 { get; set; }
        [JsonPropertyName("mn")] public double? mn { get; set; }
        [JsonPropertyName("αn")] public double? αn { get; set; }
        [JsonPropertyName("β")] public double? β { get; set; }
        [JsonPropertyName("b1")] public double? b1 { get; set; }
        [JsonPropertyName("b2")] public double? b2 { get; set; }
        [JsonPropertyName("a")] public double? a { get; set; }
        [JsonPropertyName("Wk1")] public double? Wk1 { get; set; }
        [JsonPropertyName("Wk2")] public double? Wk2 { get; set; }
        [JsonPropertyName("k1")] public double? k1 { get; set; }
        [JsonPropertyName("k2")] public double? k2 { get; set; }
        [JsonPropertyName("x1")] public double? x1 { get; set; }
        [JsonPropertyName("x2")] public double? x2 { get; set; }
        [JsonPropertyName("da1")] public double? da1 { get; set; }
        [JsonPropertyName("da2")] public double? da2 { get; set; }
        [JsonPropertyName("hfP_mn")] public double? hfP_mn { get; set; }
        [JsonPropertyName("ρfP_mn")] public double? ρfP_mn { get; set; }
        [JsonPropertyName("Ca")] public double? Ca { get; set; }
        [JsonPropertyName("Cf")] public double? Cf { get; set; }
        [JsonPropertyName("cx")] public double? cx { get; set; }
        [JsonPropertyName("E_sns")] public double? E_sns { get; set; }
        // ── kisssoft 优化参数（MAIN-Optimization.cpp DefineGearPairGeometryZJ）──
        /// <summary>小轮齿厚上偏差 E_sns（mm），缺省回退 E_sns</summary>
        [JsonPropertyName("E_sns1")] public double? E_sns1 { get; set; }
        /// <summary>小轮齿厚下偏差 E_sni（mm），缺省回退 E_sns1</summary>
        [JsonPropertyName("E_sni1")] public double? E_sni1 { get; set; }
        /// <summary>大轮齿厚上偏差 E_sns（mm），缺省回退 E_sns</summary>
        [JsonPropertyName("E_sns2")] public double? E_sns2 { get; set; }
        /// <summary>大轮齿厚下偏差 E_sni（mm），缺省回退 E_sns2</summary>
        [JsonPropertyName("E_sni2")] public double? E_sni2 { get; set; }
        /// <summary>Y 圆直径 d_y（mm），缺省用 InvGearInput 默认 25</summary>
        [JsonPropertyName("d_y")] public double? d_y { get; set; }
    }

    public class IsoQuality
    {
        [JsonPropertyName("Q")] public double? Q { get; set; }
        [JsonPropertyName("fpt1")] public double? fpt1 { get; set; }
        [JsonPropertyName("fpt2")] public double? fpt2 { get; set; }
        [JsonPropertyName("ffα")] public double? ffα { get; set; }
        [JsonPropertyName("fHβ1")] public double? fHβ1 { get; set; }
        [JsonPropertyName("fHβ2")] public double? fHβ2 { get; set; }
        [JsonPropertyName("RaH")] public double? RaH { get; set; }
        [JsonPropertyName("RaF")] public double? RaF { get; set; }
    }

    public class IsoMaterial
    {
        [JsonPropertyName("E")] public double? E { get; set; }
        [JsonPropertyName("ν")] public double? ν { get; set; }
        [JsonPropertyName("σS")] public double? σS { get; set; }
        [JsonPropertyName("ρ")] public double? ρ { get; set; }
        [JsonPropertyName("σHlim")] public double? σHlim { get; set; }
        [JsonPropertyName("σFlim")] public double? σFlim { get; set; }
        [JsonPropertyName("HRC")] public double? HRC { get; set; }
        [JsonPropertyName("HRC_core")] public double? HRC_core { get; set; }
    }

    public class IsoApplication
    {
        [JsonPropertyName("KA")] public double? KA { get; set; }
        [JsonPropertyName("Kγ")] public double? Kγ { get; set; }
        [JsonPropertyName("l")] public double? l { get; set; }
        [JsonPropertyName("s")] public double? s { get; set; }
        [JsonPropertyName("dsh")] public double? dsh { get; set; }
        [JsonPropertyName("SHmin")] public double? SHmin { get; set; }
        [JsonPropertyName("SFmin")] public double? SFmin { get; set; }
        [JsonPropertyName("ν40")] public double? ν40 { get; set; }
        [JsonPropertyName("fcp")] public double? fcp { get; set; }
        [JsonPropertyName("hm")] public double? hm { get; set; }
        [JsonPropertyName("pmc")] public double? pmc { get; set; }
    }

    public class IsoLoad
    {
        [JsonPropertyName("T1")] public double? T1 { get; set; }
        [JsonPropertyName("n1")] public double? n1 { get; set; }
        [JsonPropertyName("Lh")] public double? Lh { get; set; }
    }

    public class IsoExpectedOutput
    {
        [JsonPropertyName("σH")] public double? σH { get; set; }
        [JsonPropertyName("σH0")] public double? σH0 { get; set; }
        [JsonPropertyName("σHP")] public Dictionary<string, double>? σHP { get; set; }
        [JsonPropertyName("σF")] public Dictionary<string, double>? σF { get; set; }
        [JsonPropertyName("σF0")] public Dictionary<string, double>? σF0 { get; set; }
        [JsonPropertyName("σFP")] public Dictionary<string, double>? σFP { get; set; }
        [JsonPropertyName("d")] public Dictionary<string, double>? d { get; set; }
        [JsonPropertyName("da")] public Dictionary<string, double>? da { get; set; }
        [JsonPropertyName("db")] public Dictionary<string, double>? db { get; set; }
        [JsonPropertyName("dw")] public Dictionary<string, double>? dw { get; set; }
    }
}
