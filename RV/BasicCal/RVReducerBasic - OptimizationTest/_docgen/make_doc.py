# -*- coding: utf-8 -*-
"""生成《RV减速器关键部件设计原理》Word 文档。
原则：先放原理示意图，随后放置与该图对应的设计原理正文；正文依据
RVReducerBasic 计算引擎（C++）源码内容整理。
"""
import os, re
import docx
from docx import Document
from docx.shared import Pt, Inches, RGBColor, Cm
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_TAB_ALIGNMENT
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml.ns import qn
from docx.oxml import OxmlElement

BASE = os.path.dirname(os.path.abspath(__file__))
FIG = os.path.join(BASE, "figs")
OUTDOC = os.path.join(os.path.dirname(BASE),
                      "RV减速器关键部件设计原理（基于C++计算引擎）.docx")

# ---------------- 颜色 ----------------
C_HEAD = RGBColor(0x1F, 0x3B, 0x5C)   # 深蓝
C_SUB = RGBColor(0x2F, 0x5F, 0x8F)
C_BODY = RGBColor(0x20, 0x20, 0x20)

SONG = u"宋体"
HEI = u"黑体"
YH = u"微软雅黑"
KAI = u"楷体"
LATIN = "Times New Roman"


def set_run_font(run, east=SONG, latin=LATIN, size=11, bold=False, italic=False,
                 color=C_BODY, sub=False, sup=False):
    run.font.name = latin
    r = run._element.rPr.rFonts
    r.set(qn('w:eastAsia'), east)
    run.font.size = Pt(size)
    run.font.bold = bold
    run.font.italic = italic
    run.font.color.rgb = color
    if sub:
        run.font.subscript = True
    if sup:
        run.font.superscript = True


def set_style_font(style, east=SONG, latin=LATIN, size=11, bold=False, color=None):
    style.font.name = latin
    style._element.rPr.rFonts.set(qn('w:eastAsia'), east)
    style.font.size = Pt(size)
    style.font.bold = bold
    if color is not None:
        style.font.color.rgb = color


def add_par(doc, text="", size=11, bold=False, east=SONG, align=None, space_after=4,
            line=1.35, first_indent=None, color=C_BODY, italic=False):
    p = doc.add_paragraph()
    pf = p.paragraph_format
    if align is not None:
        pf.alignment = align
    pf.space_after = Pt(space_after)
    pf.line_spacing = line
    if first_indent is not None:
        pf.first_line_indent = Pt(first_indent)
    if text:
        run = p.add_run(text)
        set_run_font(run, east=east, size=size, bold=bold, italic=italic, color=color)
    return p


def add_mixed(doc, parts, align=None, size=11, space_after=4, line=1.35):
    """parts: list of (text, dict) 或 (text)。 dict 可含 east/bold/italic/color/sub/sup"""
    p = doc.add_paragraph()
    pf = p.paragraph_format
    if align is not None:
        pf.alignment = align
    pf.space_after = Pt(space_after)
    pf.line_spacing = line
    for item in parts:
        if isinstance(item, tuple):
            text, k = item
        else:
            text, k = item, {}
        run = p.add_run(text)
        set_run_font(run, east=k.get('east', SONG), size=k.get('size', size),
                     bold=k.get('bold', False), italic=k.get('italic', False),
                     color=k.get('color', C_BODY),
                     sub=k.get('sub', False), sup=k.get('sup', False))
    return p


# 匹配：_{...} / ^{...}（花括号） 与 无花括号单段上下标 _x 或 ^n
TOKEN = re.compile(
    r"(_\{[^}]*\}|\^\{[^}]*\}|"          # 花括号式
    r"_[A-Za-z0-9αβγδεζηθικλμνξπρστυφχψωΣ′]+"   # 无花括号下标
    r"|\^[A-Za-z0-9εαβγδεζθρστυχψωΣ′]+"         # 无花括号上标
    r")"
)


def add_formula(doc, content, num=None, size=12, font="Cambria Math", note=None):
    """公式段：content 支持 _{x}/^{x}(花括号) 与 _x/^n(无花括号) 上下标。num 为公式编号。"""
    p = doc.add_paragraph()
    pf = p.paragraph_format
    pf.space_before = Pt(4)
    pf.space_after = Pt(4)
    pf.line_spacing = 1.2
    pf.alignment = WD_ALIGN_PARAGRAPH.CENTER
    # 右侧编号制表位
    if num:
        pf.tab_stops.add_tab_stop(Inches(6.6), WD_TAB_ALIGNMENT.RIGHT)
    # 解析
    segs = TOKEN.split(content)
    for s in segs:
        if not s:
            continue
        if s.startswith("_{") and s.endswith("}"):
            run = p.add_run(s[2:-1]); set_run_font(run, east=SONG, latin=font,
                                                   size=size, sub=True)
        elif s.startswith("^{") and s.endswith("}"):
            run = p.add_run(s[2:-1]); set_run_font(run, east=SONG, latin=font,
                                                   size=size, sup=True)
        elif s.startswith("^"):
            run = p.add_run(s[1:]); set_run_font(run, east=SONG, latin=font,
                                                 size=size, sup=True)
        elif s.startswith("_"):
            run = p.add_run(s[1:]); set_run_font(run, east=SONG, latin=font,
                                                 size=size, sub=True)
        else:
            run = p.add_run(s); set_run_font(run, east=SONG, latin=font, size=size)
    if num:
        rn = p.add_run("\t(%s)" % num)
        set_run_font(rn, east=SONG, latin=LATIN, size=11)
    return p


def add_caption(doc, text):
    p = doc.add_paragraph()
    p.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(3)
    p.paragraph_format.space_after = Pt(10)
    run = p.add_run(text)
    set_run_font(run, east=HEI, latin=LATIN, size=10.5, bold=True, color=C_SUB)


def add_pic(doc, path, width_in=6.3):
    p = doc.add_paragraph()
    p.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(2)
    run = p.add_run()
    run.add_picture(path, width=Inches(width_in))


def add_h1(doc, text):
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(16)
    p.paragraph_format.space_after = Pt(8)
    run = p.add_run(text)
    set_run_font(run, east=YH, latin=LATIN, size=16, bold=True, color=C_HEAD)
    pPr = p._p.get_or_add_pPr()
    pBdr = OxmlElement('w:pBdr')
    bottom = OxmlElement('w:bottom')
    bottom.set(qn('w:val'), 'single'); bottom.set(qn('w:sz'), '12')
    bottom.set(qn('w:space'), '4'); bottom.set(qn('w:color'), '1F3B5C')
    pBdr.append(bottom); pPr.append(pBdr)
    return p


def add_h2(doc, text):
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(10)
    p.paragraph_format.space_after = Pt(5)
    run = p.add_run(text)
    set_run_font(run, east=YH, latin=LATIN, size=13, bold=True, color=C_SUB)
    return p


def add_h3(doc, text):
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(3)
    run = p.add_run(text)
    set_run_font(run, east=HEI, latin=LATIN, size=11.5, bold=True, color=C_BODY)
    return p


def _add_rich_runs(p, text, east=SONG, latin_font=LATIN, size=11, bold=False,
                   italic=False, color=C_BODY):
    """把含 _{x}/^{x} 或 _x/^n 上下标标记的文本解析为带格式 run 序列。"""
    for s in TOKEN.split(text):
        if not s:
            continue
        if s.startswith("_{") and s.endswith("}"):
            run = p.add_run(s[2:-1])
            set_run_font(run, east=east, latin=latin_font, size=size, bold=bold,
                         italic=italic, color=color, sub=True)
        elif s.startswith("^{") and s.endswith("}"):
            run = p.add_run(s[2:-1])
            set_run_font(run, east=east, latin=latin_font, size=size, bold=bold,
                         italic=italic, color=color, sup=True)
        elif s.startswith("^"):
            run = p.add_run(s[1:])
            set_run_font(run, east=east, latin=latin_font, size=size, bold=bold,
                         italic=italic, color=color, sup=True)
        elif s.startswith("_"):
            run = p.add_run(s[1:])
            set_run_font(run, east=east, latin=latin_font, size=size, bold=bold,
                         italic=italic, color=color, sub=True)
        else:
            run = p.add_run(s)
            set_run_font(run, east=east, latin=latin_font, size=size, bold=bold,
                         italic=italic, color=color)


def body(doc, text, **kw):
    size = kw.pop("size", 11)
    align = kw.pop("align", None)
    east = kw.pop("east", SONG)
    p = doc.add_paragraph()
    pf = p.paragraph_format
    pf.first_line_indent = Pt(22)
    pf.space_after = Pt(kw.pop("space_after", 4))
    pf.line_spacing = kw.pop("line", 1.35)
    if align is not None:
        pf.alignment = align
    _add_rich_runs(p, text, east=east, size=size)
    return p


def bullet(doc, text, indent=0.25):
    p = doc.add_paragraph()
    pf = p.paragraph_format
    pf.left_indent = Inches(indent)
    pf.space_after = Pt(3)
    pf.line_spacing = 1.3
    r0 = p.add_run(u"•  ")
    set_run_font(r0, size=11, bold=True, color=C_SUB)
    run = p.add_run(text)
    set_run_font(run, size=11)
    return p


def make_table(doc, header, rows, widths=None, fontsize=10.5, header_bg="1F3B5C"):
    t = doc.add_table(rows=1 + len(rows), cols=len(header))
    t.style = "Table Grid"
    t.alignment = WD_TABLE_ALIGNMENT.CENTER
    # 表头
    for j, h in enumerate(header):
        cell = t.cell(0, j)
        cell.text = ""
        p = cell.paragraphs[0]
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        run = p.add_run(h)
        set_run_font(run, east=HEI, size=fontsize, bold=True, color=RGBColor(0xFF, 0xFF, 0xFF))
        shd = OxmlElement('w:shd'); shd.set(qn('w:val'), 'clear')
        shd.set(qn('w:color'), 'auto'); shd.set(qn('w:fill'), header_bg)
        cell._tc.get_or_add_tcPr().append(shd)
    for i, row in enumerate(rows):
        for j, val in enumerate(row):
            cell = t.cell(i + 1, j)
            cell.text = ""
            p = cell.paragraphs[0]
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER if len(str(val)) <= 12 else WD_ALIGN_PARAGRAPH.LEFT
            run = p.add_run(str(val))
            set_run_font(run, size=fontsize)
            if i % 2 == 1:
                shd = OxmlElement('w:shd'); shd.set(qn('w:val'), 'clear')
                shd.set(qn('w:color'), 'auto'); shd.set(qn('w:fill'), 'EEF3F8')
                cell._tc.get_or_add_tcPr().append(shd)
    if widths:
        for j, w in enumerate(widths):
            for row in t.rows:
                row.cells[j].width = Inches(w)
    return t


def spacer(doc, pt=6):
    p = doc.add_paragraph()
    p.paragraph_format.space_after = Pt(pt)
    return p


# ================================================================ 构建文档
doc = Document()
# 页面设置
sec = doc.sections[0]
sec.page_width, sec.page_height = Inches(8.27), Inches(11.69)   # A4
sec.left_margin = sec.right_margin = Inches(1.0)
sec.top_margin = sec.bottom_margin = Inches(0.9)

# Normal 默认字体
set_style_font(doc.styles['Normal'], east=SONG, size=11)

# ---------------- 封面/标题 ----------------
for _ in range(4):
    spacer(doc, 12)
tp = add_par(doc, "RV 减速器关键部件设计原理", size=24, bold=True, east=YH,
             align=WD_ALIGN_PARAGRAPH.CENTER, color=C_HEAD, space_after=6)
add_par(doc, "—— 摆线针轮副 · 渐开线齿轮副 · 曲轴系", size=14, east=YH,
        align=WD_ALIGN_PARAGRAPH.CENTER, color=C_SUB, space_after=20)
add_par(doc, "（依据 RVReducerBasic C++ 计算引擎源码归纳整理）", size=11, east=KAI,
        align=WD_ALIGN_PARAGRAPH.CENTER, color=C_BODY, space_after=6)
add_par(doc, "文档说明：本文对计算引擎中摆线针轮副（CPinionCycloidGearTAN）、"
             "渐开线齿轮副（CInvoluteGearTAN）、曲轴系（CShaftSystemTAN / CShaftTAN）"
             "三大部分的设计原理进行概述；每部分先给出原理示意图，随后放置与之对应的设计原理正文、"
             "关键公式与代码实现对照。",
        size=10.5, east=KAI, align=WD_ALIGN_PARAGRAPH.LEFT, space_after=8)

# ---------------- 绪论 ----------------
add_h1(doc, u"绪　论")
body(doc,
     u"RV 减速器是“两级减速 + 摆线针轮少齿差行星传动”的组合。计算引擎 RVReducerBasic 中，"
     u"RV 减速器总体由 CRVReducerTAN（TANRVReducer.cpp）组织：第一级为渐开线齿轮副（高速级，"
     u"小传动比），第二级为摆线针轮副（低速级，大传动比），两级之间通过曲轴（曲柄轴）串联并输出动力。"
     u"三大部分各自承担不同功能，其设计原理分述如下表：")
make_table(doc,
           [u"部件", u"代码模块", u"在减速器中的角色", u"设计核心"],
           [
               [u"摆线针轮副", u"CPinionCycloidGearTAN", u"第Ⅱ级·少齿差行星大减速",
                u"摆线齿廓几何 + 接触强度 + 跨棒距测量"],
               [u"渐开线齿轮副", u"CInvoluteGearTAN", u"第Ⅰ级·行星/平行轴高速级",
                u"标准几何 + ISO6336 接触/弯曲强度"],
               [u"曲轴系", u"CShaftSystemTAN / CShaftTAN", u"动力输出·偏心驱动摆线轮",
                u"一维梁有限元强度 + 轴承支承 + 轴径推荐"],
           ], widths=[1.0, 1.9, 1.8, 2.0])
spacer(doc, 4)
body(doc,
     u"下文按用户要求的顺序分别阐述。各章的“图”均采用与代码齿廓/受力一致的方式绘制：例如摆线齿廓点直接取自"
     u" TANPinionCycloidGear.cpp::calculateToothProfile() 的短幅外摆线等距线方程，渐开线齿形由基圆展开生成，"
     u"曲轴系按代码中“多曲轴 + 一维梁模型 + 轴承支承”的结构示意。")

# ================================================================ 第1章
add_h1(doc, u"第 1 章　摆线针轮副设计原理")
body(doc,
     u"摆线针轮副是 RV 减速器的第Ⅱ级（低速大扭矩级）。摆线轮（行星轮）通过曲轴偏心获得转臂运动，"
     u"与固定在机壳上的针轮（针齿圈）实现“1 齿差”内啮合；当针轮固定时，摆线轮只能平动公转（无自转），"
     u"其自转角由曲柄孔导出，经曲轴对外输出大扭矩。其结构紧凑、承载大、传动比大、传动平稳，是 RV "
     u"减速器性能的核心。下图 1-1 为摆线针轮副原理图（含摆线轮齿廓与装配啮合）。")

add_pic(doc, os.path.join(FIG, "fig1_cycloid.png"), 6.5)
add_caption(doc, u"图 1-1　摆线针轮副原理图（a：摆线轮齿廓与几何；(b)：与针轮啮合示意）")

add_h2(doc, u"1.1　基本结构与几何参数")
body(doc,
     u"摆线针轮副的几何参数由代码 CPinionCycloidGearTAN 管理，其主要输入如下表，表中符号与源码字段对应：")
make_table(doc,
           [u"符号/字段", u"含义", u"说明"],
           [
               [u"zb (mGear.Z)", u"针轮(针齿)齿数", u"中心轮，即固定在机壳上的针齿圈齿数"],
               [u"zg (mPinion.Z)", u"摆线轮齿数", u"一般 zg = zb − 1（1 齿差）"],
               [u"Ng", u"摆线轮片数", u"默认 2 片，成对布置实现动平衡与均载"],
               [u"Rz (Rzf)", u"针齿中心圆(分度圆)半径", u"针齿分布圆半径"],
               [u"rz", u"针齿(滚针)半径", u"针齿齿廓圆半径"],
               [u"e (ee)", u"偏心距", u"摆线轮中心到针轮中心的距离，等于曲轴偏心量"],
               [u"K1", u"短幅系数", u"K1 = e·zb / Rz"],
               [u"K2", u"针径系数", u"K2 = Rz/rz · sin(π/zb)"],
           ], widths=[1.1, 1.8, 3.8])
spacer(doc, 4)
body(doc,
     u"代码 CalBasicGeometry() 给出的核心几何关系为：针轮节圆半径 r_b′ = e·zb，摆线轮节圆半径 r_g′ = e·zg；"
     u"外啮合滚圆半径 r = e/K1，基圆半径 R = Rz·zg/zb；摆线轮齿顶圆、齿根圆与齿全高为：")
add_formula(doc, u"r_a = R_z + e − r_z，　　r_f = R_z − e − r_z，　　h = 2e", num=u"1-1")
body(doc,
     u"可见摆线轮齿高仅由偏心距 e 决定（2e），与针齿直径无关。偏心距与针齿半径在给定 Rz 后可“圆整”处理："
     u"e = ⌈2K1Rz/zb⌉/2（按 0.5 的倍数取整，ChooseRoundEccentricCoef），针齿半径同理由 K2 反求并取整；"
     u"圆整后按实际值重算 K1、K2。")

add_h2(doc, u"1.2　摆线齿廓方程（短幅外摆线等距线）")
body(doc,
     u"摆线轮实际齿廓是针齿（半径为 rz 的滚针）包络线的等距曲线。代码 calculateToothProfile() "
     u"（用于跨棒距计算与齿廓点生成）直接实现该齿廓：设 iH = zb/zg，K1 = e·zb/Rz，令")
add_formula(doc, u"F(φ) = 1 / √(1 + K1^2 − 2·K1·cos φ)", num=u"1-2")
add_formula(doc,
            u"x = (R_z − r_z·F)·cos((1 − iH)φ) − (e − K1·r_z·F)·cos(iH·φ)"
            u"　　y = (R_z − r_z·F)·sin((1 − iH)φ) + (e − K1·r_z·F)·sin(iH·φ)",
            num=u"1-3")
body(doc,
     u"当 φ 在 [0, 2π·zg] 内变化即得到一条闭合、具有 zg 个齿的摆线轮完整齿廓（图 1-1a 即按该方程绘制）。"
     u"式中的 K1 即短幅系数，它决定齿廓形状：K1 越大齿形越“瘦长”。")

add_h2(doc, u"1.3　传动比与运动关系")
body(doc,
     u"摆线针轮副按输出构件不同，传动比由代码 FCalTransRatio() 给出（ExportNotation=1 销轴/曲轴输出，"
     u"=2 针壳/中心轮输出）：")
add_formula(doc, u"销轴输出：　i = z_g / (z_g − z_b)；　针壳(中心轮)输出：　i = z_b / (z_b − z_g)", num=u"1-4")
body(doc,
     u"由 zg = zb − 1 可知 |i| 数值等于摆线轮齿数（销轴输出）或针轮齿数（针壳输出），即“一齿差”带来的大减速比。"
     u"转速关系由 tagRVGearBasicTAN::CalGearRevoluteSpeed / CalPinionRevoluteSpeed 依据转臂转速与"
     u"齿轮/行星轮转速比换算（用于整机力矩—转速分配，见 TANRVReducer.cpp）。")

add_h2(doc, u"1.4　啮合受力分析")
body(doc,
     u"摆线轮承受的力矩先在各片摆线轮间按其啮合宽度比例分配（CalPinAndGearTorque：单片力矩 "
     u"T_i = (BB_i/BB_总)·T_总，并按啮合齿宽 BB 计算实际接触宽度）。随后求单个摆线轮与针齿啮合过程中的最大接触力：")
add_formula(doc, u"F_max = 4·(K_p·T) / (K_1·z_g·R_z)", num=u"1-5")
body(doc,
     u"式中 Kp 为均载/使用系数。啮合过程中各针齿所受法向力随相位 θ 变化，代码引入系数 S 与 T：")
add_formula(doc, u"S = 1 + K_1^2 − 2·K_1·cos θ，　　F_i(θ) = F_max·sin θ / √S", num=u"1-6")
add_formula(doc, u"T = K_1(1 + z_b)·cos θ − (1 + z_b·K_1^2)", num=u"1-7")
body(doc,
     u"由静力平衡，摆线轮对曲轴/轴承的作用力（CalBearingForce）进一步分解为圆周力与径向力（供曲轴系使用）：")
add_formula(doc, u"F_t = T·z_b / (K_1·R_z·z_g)·1000，　F_r = F_t·K_y，　F_总 = F_t·√(1 + K_y^2)", num=u"1-8")
body(doc,
     u"其中 K_y 与啮合角相关，由代码沿针轮圆周数值积分求得；单根曲轴分摊的轴承力按曲轴根数与载荷不均匀系数 "
     u"K_d 分配（见第 3 章）。")

add_h2(doc, u"1.5　接触强度校核（赫兹接触）")
body(doc,
     u"摆线针轮为线接触（摆线齿面—针齿圆柱面），按赫兹公式校核。先求摆线齿廓在啮合点的曲率半径：")
add_formula(doc, u"ρ_0 = R_z·S^{3/2} / T", num=u"1-9")
body(doc,
     u"接触点处的相对曲率（针齿外凸、摆线齿面内凹/外凸，代码取差值）、当量弹性模量及名义接触应力分别为：")
add_formula(doc, u"1/ρ_Σ = 1/r_z − 1/(r_z + ρ_0)，　　E′ = 2E_{1}E_{2}/(E_1 + E_2)", num=u"1-10")
add_formula(doc, u"σ_H(θ) = 0.418·√( F_i(θ)·E′·ρ_Σ / b )", num=u"1-11")
body(doc,
     u"式中 b 为接触宽度。代码对 θ∈(0, π) 以等步长扫描并嵌套黄金分割求极小（FindMaxContStress），"
     u"得到最大接触应力 σHmax 及其对应的摆线轮相位 θ 与片号；应力过大时以 1e10 作为失败标记供上层识别。"
     u"摆线轮“参数推荐”即反向求解：给定目标接触应力后，以 21·³√T 为初值，用二分/牛顿迭代反求针轮中心圆半径 Rz，"
     u"再按齿宽系数（默认 0.15）确定齿宽、按经验式 K2 = −0.0243421·zb + 3.14211 推荐针径系数并圆整。")

add_h2(doc, u"1.6　加工检测——跨棒距 M 值")
body(doc,
     u"摆线轮通常以跨棒距（M 值）作为加工检测尺寸。代码 CalcMValue() 给定摆线轮齿数、针齿半径、针齿中心圆直径、"
     u"偏心距、量棒直径与公差，先由齿廓拐点（φ_A 处法线）迭代求量棒中心圆半径 RL，再由跨测间隔齿数换算两量棒中心"
     u"夹角并叠加量棒直径得到 M 值，供齿廓加工测量与公差控制使用。")

# ================================================================ 第2章
add_h1(doc, u"第 2 章　渐开线齿轮副设计原理")
body(doc,
     u"渐开线齿轮副是 RV 减速器第Ⅰ级（高速级），可采用平行轴外啮合、行星传动（太阳轮—行星轮—内齿圈）"
     u"或人字齿等形式，代码以 CInvoluteGearTAN 统一实现（外啮合 Z2>0、内啮合 Z2<0 的约定，另有人字齿等"
     u"齿轮副类型枚举）。其强度校核完整遵循 ISO 6336 方法 B（接触强度 ISO 6336-2、齿根弯曲 ISO 6336-3）。"
     u"图 2-1 为渐开线齿轮副啮合几何原理图。")

add_pic(doc, os.path.join(FIG, "fig2_involute.png"), 6.3)
add_caption(doc, u"图 2-1　渐开线齿轮副啮合原理图（基圆、分度圆、啮合线与压力角）")

add_h2(doc, u"2.1　基本参数与齿轮类型")
make_table(doc,
           [u"参数", u"符号", u"默认/取值", u"说明"],
           [
               [u"法向模数", u"mn", u"按标准模数序列", u"ISO 第一/第二序列 0.1~50mm"],
               [u"法向压力角", u"αn", u"20°", u""],
               [u"螺旋角", u"β", u"0(直齿)", u"斜齿/人字齿时取 8°~20°"],
               [u"变位系数", u"x", u"推荐区间内", u"高变位，可等寿命优化"],
               [u"齿顶高系数", u"h_ax", u"1.0", u""],
               [u"顶隙系数", u"cx", u"0.25", u""],
               [u"齿宽", u"b", u"—", u"设计齿宽 b_Design/工作齿宽"],
               [u"行星轮个数", u"n_P / N_m", u"1", u"力按 N_m 均分"],
               [u"材料类型", u"MaterialType", u"14 类", u"碳钢/铸铁/渗碳/氮化等"],
           ], widths=[1.3, 1.2, 1.6, 2.6])
spacer(doc, 4)

add_h2(doc, u"2.2　单齿轮几何计算")
body(doc,
     u"单齿几何由 CSingleInvoluteTAN::CalSingleGearBasicGeometry() 计算。斜齿轮先换算端面参数：")
add_formula(doc, u"m_t = m_n/cos β，　α_t = arctan(tan α_n/cos β)，　β_b = arctan(tan β·cos α_t)", num=u"2-1")
add_formula(doc, u"d = z·m_t，　d_b = d·cos α_t，　inv α_t = tan α_t − α_t", num=u"2-2")
body(doc,
     u"齿顶高按名义齿顶高系数与高变位系数给出 ha = (h_ax + x)·mn，齿顶圆 da = d ± 2ha（外啮合取＋、内啮合取－）。"
     u"齿根部分代码区分“名义齿根”与“齿条刀加工齿根”：名义齿根高系数 h_fx = h_ax + cx，而实际齿根高由产形（加工）"
     u"变位 X_E 控制 h_f = (h_ax + cx)·mn − X_E·mn，从而 df = d ∓ 2h_f；由此同时得到渐开线起始圆直径 dFf 等。"
     u"当量齿数 zn、跨 k 齿法向公法线长度（代码 Wk，其中 X_E 可由实测公法线反求）为：")
add_formula(doc, u"z_n ≈ z/cos^{3} β_b，　W_k = m_n·cos α_n·[π(k − 1/2) + z·inv α_t] + 2·X_E·m_n·sin α_n", num=u"2-3")
body(doc,
     u"精度方面，由精度等级自动给出端面单个齿距偏差 f_pt、齿廓偏差 f_α、螺旋线偏差 f_Hβ 等；"
     u"无根切最小变位 CalMinProfileShift()、齿顶不变尖最大变位 CalMaxProfileShift() 则圈定变位系数的可行域。")

add_h2(doc, u"2.3　齿轮副啮合几何与变位协调")
body(doc,
     u"对齿轮副（CInvoluteGearTAN::CalGearPairGeometry）先统一端面参数、齿数比 u = z2/z1 与未变位中心距 "
     u"a_x = (d2 ± d1)/2。变位与中心距按渐开线啮合方程相互协调：")
add_formula(doc, u"由变位系数和求中心距：　inv α_wt = 2·x_Σ·tan α_n/Σz + inv α_t", num=u"2-4")
add_formula(doc, u"y_t = (Σz/2)·(cos α_t/cos α_wt − 1)，　a_w = a_x + y_t·m_t", num=u"2-5")
add_formula(doc, u"由中心距求变位系数和：　y_t = (a_w − a_x)/m_t，　α_wt = arccos(a_x·cos α_t/a_w)",
            num=u"2-6")
add_formula(doc, u"x_Σ = (inv α_wt − inv α_t)·Σz / (2·tan α_n)", num=u"2-7")
body(doc,
     u"工作节圆 dw = db/cos αwt（代码 CalGearPairGeometry）。接触线长度、重合度（端面重合度 εα、"
     u"纵向重合度 εβ 与总重合度 εγ）及齿面曲率半径由 CalContactPathLength()、CalSurfaceCurvatureRadius() "
     u"求出，供强度计算使用。")

add_h2(doc, u"2.4　载荷与力分析")
body(doc,
     u"以输入小轮扭矩/转速/设计寿命定义载荷（可指定按小轮或大轮、行星轮个数均分）。单齿受力分解（CalForce）为：")
add_formula(doc, u"F_t = 2000·T_1/(d_1·N_m)，　F_r ≈ F_t·tan α_n/cos β，　F_a = ±F_t·tan β，　F_n = F_t/(cos α_n·cos β)",
            num=u"2-8")
body(doc,
     u"强度计算所用的总载荷系数由 K_A（使用系数）、K_γ、K_v（动载系数，ISO 方法 B，考虑精度/共振）、"
     u"K_Hβ、K_Fβ（齿向载荷分布）、K_Hα、K_Fα（齿间载荷分配）构成，分别用于接触与弯曲校核。")

add_h2(doc, u"2.5　齿面接触强度（ISO 6336-2 方法 B）")
body(doc,
     u"代码 CalSurfaceDurability() 依次计算节点区域系数 Z_H、弹性系数 Z_E、重合度系数 Z_ε、螺旋角系数 Z_β：")
add_formula(doc, u"Z_H = √(2·cos β_b·cos α_wt / (cos^2 α_t·sin α_wt))", num=u"2-9")
add_formula(doc, u"Z_E = √( 1 / (π·[(1−ν_1^2)/E_1 + (1−ν_2^2)/E_2]) )，　Z_ε = √(1/ε_α)，　Z_β = √(1/cos β)",
            num=u"2-10")
add_formula(doc,
            u"σ_H0 = Z_H·Z_E·Z_ε·Z_β·√( (F_t/(d_1·b))·(u+1)/u )",
            num=u"2-11")
add_formula(doc, u"σ_H = Z_B 或 Z_D（单对齿啮合系数）· σ_H0·√(K_A·K_γ·K_v·K_Hβ·K_Hα)", num=u"2-12")
body(doc,
     u"许用接触应力与安全系数：σ_HP = σ_Hlim·Z_N·Z_W·Z_L·Z_v·Z_R·Z_X / S_Hmin，S_H = σ_HP·S_Hmin/σ_H。"
     u"其中 σHlim 为试验齿轮接触疲劳极限（按 14 类材料给定），Z_N 为寿命系数（按载荷循环次数分段并做对数插值），"
     u"Z_W 为工作硬化系数，Z_L/Z_v/Z_R 为润滑剂、速度、粗糙度系数，Z_X 为尺寸系数（ISO 取 1）。")

add_h2(doc, u"2.6　齿根弯曲强度（ISO 6336-3 方法 B）")
body(doc,
     u"弯曲强度按齿根 30° 切线法求危险截面。单齿齿形系数 Y_F 由齿轮 EE/GG/HH 三参数迭代求出载荷作用角 θ "
     u"后计算（EE 为齿根圆角起始参数等），再乘应力修正系数 Y_S、螺旋角系数 Y_β、轮缘(腹板)系数 Y_B、齿顶"
     u"载荷分担系数 Y_DT。窄齿轮对大齿轮的有效齿宽可按 ISO 取 min(b_小 + 2mn, b_大)。名义与计算齿根应力：")
add_formula(doc, u"σ_F0 = (F_t/b_eff)·Y_F·Y_S·Y_β·Y_B·Y_DT / m_n", num=u"2-13")
add_formula(doc, u"σ_F = K_A·K_γ·K_v·K_Fβ·K_Fα · σ_F0", num=u"2-14")
add_formula(doc,
            u"σ_FP = σ_Flim·Y_ST·Y_NT·Y_δrelT·Y_RrefT·Y_X / S_Fmin，　S_F = σ_FP·S_Fmin/σ_F",
            num=u"2-15")
body(doc,
     u"其中 Y_ST = 2.0（应力修正因子的基准值），Y_NT 为寿命系数，Y_δrelT/Y_RrefT 为相对齿根圆角敏感与表面状况系数，"
     u"Y_X 为尺寸系数。代码同时给出静强度（短时过载）校核结果，便于分析冲击工况。")

add_h2(doc, u"2.7　参数推荐与等寿命优化")
body(doc,
     u"依据机械设计手册接触疲劳设计公式，给定使用系数与目标接触应力后，可用 A_a（中心距修正系数，螺旋角为 0 时 483）"
     u"等经验系数初估最小中心距与小轮直径（RecommondMainGeometryParaByContact）；再按传动比与中心距、模数分配齿数"
     u"（OptimizeGearTooth，行星传动可要求互质避免干涉/均载），最后通过 OptimizeProfileShiftToEqualLife() "
     u"在“无根切—齿顶不变尖”可行域内迭代变位系数，使大小轮弯曲或接触寿命趋于一致，得到满足等寿命的推荐参数。"
     u"该模块曾按 ISO/TR 6336-30:2022 Example 1 对标验证：几何参数 100% 吻合、名义接触应力偏差约 0.04%，"
     u"计算接触/弯曲应力偏差在数个百分点内，表明实现与 ISO 方法 B 一致。")

# ================================================================ 第3章
add_h1(doc, u"第 3 章　曲轴系设计原理")
body(doc,
     u"曲轴（曲柄轴）系是 RV 减速器的输出级关键总成：它把渐开线第Ⅰ级行星轮的运动作为输入，通过其偏心段（偏心量 "
     u"e，等于摆线偏心距）驱动两片摆线轮绕针轮作行星运动，并把摆线轮通过曲柄孔传给曲轴的低速大扭矩汇合输出"
     u"（针轮壳固定时由曲轴输出）。整机通常布置 2~3 根曲轴并按相位均布，以均衡载荷、抵消惯性。"
     u"图 3-1 为曲轴系结构与受力原理图（a：RV 两级传动与多曲轴布置；(b)：单根曲轴与一维梁力学模型）。")

add_pic(doc, os.path.join(FIG, "fig3_crankshaft.png"), 6.6)
add_caption(doc, u"图 3-1　曲轴系结构与受力原理图（a：布置；(b)：单轴结构示意与梁模型）")

add_h2(doc, u"3.1　曲轴系组成与输入参数")
make_table(doc,
           [u"参数/构件", u"说明"],
           [
               [u"曲轴数目", u"默认 3 根，相位 120° 均布；多曲轴并联传递并均分载荷"],
               [u"偏心量 e、相位角", u"曲柄偏心量 = 摆线偏心距 e；各根曲轴相位按整机装配确定"],
               [u"轴段类型", u"主轴颈（圆截面）/ 齿轮段 / 偏心段（偏心圆截面，偏心量 e）"],
               [u"轴上构件", u"渐开线齿轮、摆线轮、主轴承/曲柄轴承（深沟球、圆柱滚子、滚针、圆锥、角接触）"],
               [u"材料", u"弹性模量 EX 与泊松比 PRXY（mShaftMat）"],
           ], widths=[1.7, 5.0])
spacer(doc, 4)
body(doc,
     u"代码以 CShaftTAN（一维梁有限元）为核心，CShaftSystemTAN 负责把整机各轴系上的“构件—作用点—约束”装配为"
     u"一个可求解的梁模型。截面以“类型 + 数据”描述：0—直接给实常数[Iz, Iy, J, A]；1—圆（1 个外径参数）；"
     u"2—偏心圆（外径、内径、偏心量、相位 4 参数，用于曲柄偏心段）。")

add_h2(doc, u"3.2　一维欧拉梁有限元建模")
body(doc,
     u"将曲轴沿轴向（z）离散为节点与梁单元：以各轴段分界和各构件作用点为节点坐标，相邻节点间为梁单元；"
     u"圆截面由 GenRealConstBySecType() 换算实常数：")
add_formula(doc, u"I = π·D^4/64，　J_p = 2·I = π·D^4/32，　A = π·(D/2)^2", num=u"3-1")
body(doc,
     u"每个节点 6 个自由度（ux、uy、uz、θx、θy、θz）。材料 EX、PRXY 由 DefineMat() 写入前处理器，"
     u"经 CFEMPreprocessor 组装、CFEMSolver 求解，得到各节点位移（TotalU）与节点反力/载荷（TotalF）。"
     u"作用点按“对称/非对称构件”分别换算到力作用面：齿轮与摆线轮按齿宽中点或安装面换算，角接触/圆锥轴承"
     u"按接触角位置（AssebleOrientation、AssembleForcePointDistPara）换算。")

add_h2(doc, u"3.3　边界条件与两级迭代求解")
body(doc,
     u"构件按载荷或约束分类加入：渐开线齿轮、摆线轮作为“载荷节点”，将其 ShaftForceVector 的 6 分量力/力矩"
     u"（Fx、Fy、Fz、Mx、My、Mz）施加到作用点（代码中统一 ×1000 归一到标准单位）；轴承作为“径向支承”"
     u"约束该点 ux = uy = 0；轴向力由角接触/圆锥轴承派生轴向力承担。若约束不足（缺平移/转动自由度），"
     u"自动在轴端附近补充约束（SupplementFixedFreedomToBCNode），防止刚体位移、保证方程组可解。")
body(doc,
     u"曲轴系按两步迭代求解（CalShaftSystem）：第 1 步（CalShaftSystemFirst）只加径向支承求各轴承径向反力 Fr；"
     u"由 Fr 计算圆锥/角接触轴承的派生轴向力 Fa_ex（CalAdditionalAxisForce）并汇总总轴向力；据此判定承担轴向的"
     u"“固定轴承”并在其上添加 uz=0 位移约束，非固定轴承则施加其派生轴向力作为边界；随后清除并重建模型进行"
     u"第 2 次求解，得到更准确的节点位移、轴承支反力与轴内力。")
body(doc,
     u"结果处理阶段 GetForceAndForceLocation() 对各节点作“右侧截断”平衡求和，得到沿轴向各截面的剪力/弯矩，"
     u"进而输出该轴段的弯曲正应力与剪应力分布（界面中绘制为“轴受力图”曲线），供设计者判断危险截面。")

add_h2(doc, u"3.4　轴径/轴长推荐与装配校核")
body(doc,
     u"“轴推荐”（RecommondShaftPara）以许用弯曲正应力 [σ]、许用剪应力 [τ] 和轴承设计寿命为目标：先给定一个较大"
     u"直径初值，通过“直径—装配—计算—校核—缩小”的迭代（UI 中 UpdateShaftDiameterAndLength → Assembly → 计算）"
     u"得到满足弯曲/扭转强度与支承寿命的最小轴直径与轴长度（轴长 = 各轴段总长 + 两端轴承宽度）。"
     u"推荐结果回填装配、驱动曲轴几何与 3D 建模。")

add_h2(doc, u"3.5　轴承支承校核")
body(doc,
     u"曲轴系的支承能力取决于所配轴承。代码 CBearingTAN 对深沟球、圆柱滚子、滚针、圆锥滚子、角接触球轴承统一"
     u"管理，由第 3.3 节解出的径向力 Fr 与轴向力 Fa 合成当量动载荷 P（含 e、X、Y 系数），进而按额定寿命公式校核：")
add_formula(doc, u"L_10 = (C/P)^ε × 10^6 / (60·n)　（小时）", num=u"3-2")
body(doc,
     u"式中 ε 为寿命指数（球轴承取 3、滚子轴承取 10/3），C 为基本额定动载荷，n 为该轴承相对转速（由整机转速分配"
     u"得到）。另作静强度校核（S0 = C0/P0 ≥ 许用安全系数）。代码还支持连接 MySQL 轴承数据库按内径/外径/载荷"
     u"检索标准轴承型号并反推寿命，实现“按寿命选轴承”的闭环设计。")

# ================================================================ 附录
add_h1(doc, u"附录　代码实现对照表")
body(doc, u"下表汇总三大部分设计原理对应的 C++ 源文件与关键函数，便于查证与二次开发：")
make_table(doc,
           [u"部件", u"源文件", u"关键函数 / 公式"],
           [
               [u"摆线针轮副", u"TANPinionCycloidGear.h/.cpp",
                u"CalBasicGeometry（几何）、FCalTransRatio（传动比）、CalContForceMax、CalCoefS/T（受力）、"
                u"CalCycloidCurveCurvatureRadius、CalNominalContStressBasic（接触应力）、"
                u"calculateToothProfile（齿廓）、CalcMValue（跨棒距）、RecommondMainGeometryParaByContact（推荐）"],
               [u"渐开线齿轮副", u"TANInvoluteGear.h/.cpp",
                u"CalSingleGearBasicGeometry（单齿）、CalGearPairGeometry / CalProfileShiftSumByCenterDistance（啮合/变位）、"
                u"CalContactPathLength（重合度）、CalSurfaceDurability（接触）、CalToothBendingStrength / Get_YF / Get_YS（弯曲）、"
                u"OptimizeProfileShiftToEqualLife / RecommondMainGeometryParaByContact（推荐）"],
               [u"曲轴系", u"TANShaftSystem.h/.cpp、TANShaft.h/.cpp",
                u"DefineOrginGeometry / InsertGeometyPara（几何）、GenRealConstBySecType（截面实常数）、"
                u"CalBeamStruct（梁单元装配求解）、CalShaftSystem / CalShaftSystemFirst（两级迭代）、"
                u"PushBackGear / PushBackBearing（构件与约束）、GetForceAndForceLocation（轴内力/应力）"],
           ], widths=[0.95, 1.75, 4.0], fontsize=9.5)
spacer(doc, 6)
body(doc,
     u"注：以上设计原理、公式与流程均据 RVReducerBasic 计算引擎源码归纳；图中摆线齿廓点直接采用代码方程生成，"
     u"渐开线齿形按基圆展开绘制，曲轴系按代码结构示意。部分实现细节（如单位约定：摆线接触应力公式力矩以 N·mm "
     u"计；轴系力统一 ×1000 归一等）在二次开发时需注意与上层调用保持一致。")

doc.save(OUTDOC)
print("saved:", OUTDOC)
