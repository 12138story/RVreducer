# -*- coding: utf-8 -*-
"""根据 RVReducerBasic 计算引擎代码生成三张原理示意图（PNG）。
 图1 摆线针轮副  图2 渐开线齿轮副  图3 曲轴系
齿廓曲线使用代码 TANPinionCycloidGear.cpp::calculateToothProfile 的方程绘制。
"""
import os
import numpy as np
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.patches import Circle, FancyArrow, Rectangle, FancyBboxPatch, Polygon
from matplotlib.lines import Line2D

plt.rcParams["font.sans-serif"] = ["Microsoft YaHei", "SimHei", "DejaVu Sans"]
plt.rcParams["axes.unicode_minus"] = False

OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "figs")
os.makedirs(OUT, exist_ok=True)

C_BLUE = "#2f6fb2"
C_RED = "#c0392b"
C_GREEN = "#1e8449"
C_GRAY = "#bdc3c7"
C_ORANGE = "#d35400"
C_DARK = "#17202a"


# ---------------------------------------------------------------- 摆线齿廓
def cycloid_profile(zb, zc, Rz, rz, e, n=4000):
    """代码 TANPinionCycloidGear.cpp calculateToothProfile 齿廓方程。
    zb-针轮齿数, zc-摆线轮齿数, Rz-针齿中心圆半径, rz-针齿半径, e-偏心距
    返回摆线轮(自身坐标系)闭合轮廓点 (x,y)。"""
    iH = zb / zc
    K1 = e * zb / Rz
    phi = np.linspace(0, 2 * np.pi * zc, n * zc)
    F = 1.0 / np.sqrt(1.0 + K1 * K1 - 2.0 * K1 * np.cos(phi))
    t1 = Rz - rz * F
    t2 = e - K1 * rz * F
    x = t1 * np.cos((1 - iH) * phi) - t2 * np.cos(iH * phi)
    y = t1 * np.sin((1 - iH) * phi) + t2 * np.sin(iH * phi)
    return x, y


def _rot(x, y, ang):
    ca, sa = np.cos(ang), np.sin(ang)
    return x * ca - y * sa, x * sa + y * ca


# ---------------------------------------------------------------- 渐开线
def involute_profile(z, mn, alpha_deg=20.0):
    """单个渐开线齿轮（直齿）外轮廓多边形点集。返回 (x, y)。"""
    d = z * mn
    db = d * np.cos(np.radians(alpha_deg))
    da = d + 2 * mn          # 齿顶(ha=mn)
    df = d - 2.25 * mn       # 齿根(hf=1.25mn)
    rb, ra, rf = db / 2, da / 2, df / 2
    n = 220
    # 齿槽半角在分度圆上：齿厚=s=πmn/2，齿槽=e=πmn/2 → 半齿厚角=π/(2z)
    pitch_ang = np.pi / z
    # 渐开线齿面点：从基圆半径展开 到 齿顶
    def inv(a):
        return np.tan(a) - a
    alpha_a = np.arccos(rb / ra)
    inv_aa = inv(alpha_a)
    # 从基圆到齿顶的渐开线（端面齿厚半角控制，简化用基圆齿厚半角）
    # 基圆上的齿半角：分度圆齿厚角在基圆上的展角 = π/(2z)+inv(α)
    half_ang = pitch_ang / 2.0 + inv(np.radians(alpha_deg))
    t = np.linspace(0.0, np.sqrt(ra**2 - rb**2) / rb, n)  # 展角变量
    # 以x轴为齿对称线
    pts = []
    # 一侧齿面
    for tt in t:
        # 基圆上点的位置角 phi
        ang_inv = tt
        # 半径
        r = rb * np.sqrt(1 + tt**2)
        if r < rb * 1.0001:
            continue
        # 极角(相对基圆齿中心)
        ang = half_ang + np.radians(alpha_deg) - (tt - np.arctan(tt))
        # 推导：极角 = 齿中心基准角 - invAngle(r)
        invr = np.tan(np.arccos(rb / r)) - np.arccos(rb / r)
        ang = half_ang - invr
        pts.append((r * np.cos(ang), r * np.sin(ang)))
    # 加上齿顶
    # 过渡到根圆：径向下降
    # 齿顶圆弧（两侧齿面末端）
    a_top_r = ra
    # 齿顶角
    top_ang = np.arccos(rb / ra)
    top_inv = inv(top_ang)
    top_half_ang = half_ang - top_inv
    # 齿顶宽半角
    ang_tip = np.linspace(top_half_ang, -top_half_ang, 30)
    for a in ang_tip:
        pts.append((ra * np.cos(a), ra * np.sin(a)))
    # 另一侧齿面（反向）
    side = []
    for tt in t:
        r = rb * np.sqrt(1 + tt**2)
        if r < rb * 1.0001:
            continue
        invr = np.tan(np.arccos(rb / r)) - np.arccos(rb / r)
        ang = -(half_ang - invr)
        side.append((r * np.cos(ang), r * np.sin(ang)))
    pts.extend(reversed(side))
    arr = np.array(pts)
    # 闭合并转成“从齿根到齿顶再到齿根的一整个齿”，随后与齿根圆弧拼接
    # 直接把该齿形从根处缩放到齿根半径：径向插值
    angs = np.arctan2(arr[:, 1], arr[:, 0])
    rr = np.hypot(arr[:, 0], arr[:, 1])
    # 齿根以下过渡（线性收缩到齿根圆）
    rf = df / 2.0
    # 构造根部过渡：把低于rf的点全设为rf(近似)
    # 直接用一个完整齿+齿槽的构造替代：以基圆构造渐开线到齿顶，齿根用圆角
    return arr


def make_gear_polygon(z, mn, alpha_deg=20.0, h_a=1.0, h_f=1.25):
    """返回整轮(外啮合渐开线直齿)封闭多边形点集(x,y)。

    算法：
      分度圆半径 r = z·m/2，基圆 r_b = r·cosα，齿顶 r_a = r + h_a·m，
      齿根 r_f = r − h_f·m。
      对半径为 R 的圆，其渐开线展开角 inv(α_R) = tan α_R − α_R，
      α_R = arccos(r_b/R)。
      以齿中心线为极角 0，齿面任一点极角 θ(R) = π/(2z) + inv(α) − inv(α_R)。
    """
    import numpy as np
    d = z * mn
    r = d / 2.0
    alpha = np.radians(alpha_deg)
    rb = r * np.cos(alpha)
    ra = r + h_a * mn
    rf = r - h_f * mn

    def inv(a):
        return np.tan(a) - a

    inv_al = inv(alpha)
    tooth_half = np.pi / (2.0 * z)          # 分度圆半齿厚角(无变位)
    N = 60                                   # 每侧齿面点数
    pol_tip = tooth_half + inv_al - inv(np.arccos(np.clip(rb / ra, -1, 1)))

    def side_face(sgn):
        """返回从基圆(或齿根)到齿顶的一侧齿面 [(R,θ)]，θ符号由 sgn 决定。"""
        out = []
        for k in range(N + 1):
            u = k / N
            R = rb + (ra - rb) * u
            if R <= rb:
                continue
            al = np.arccos(np.clip(rb / R, -1, 1))
            pol = tooth_half + inv_al - inv(al)
            out.append((R, sgn * pol))
        return out

    def radial(x, y, to_r):
        """把点沿径向缩放到 to_r(用于齿根直线过渡/圆角近似)。"""
        rr = np.hypot(x, y)
        if rr < 1e-12:
            return x, y
        return x / rr * to_r, y / rr * to_r

    allp = []
    for i in range(z):
        c0 = 2 * np.pi * i / z
        left = side_face(-1)      # 从齿根到齿顶(θ由负趋0)
        right = side_face(1)      # 从齿根到齿顶(θ由正趋0)
        # 构建单齿：右侧面(自齿根起升到齿顶) → 齿顶 → 左侧面(自齿顶降到齿根)
        poly = []
        for (R, th) in right:                      # 右面 根→顶
            poly.append((R * np.cos(c0 + th), R * np.sin(c0 + th)))
        # 齿顶（左右两侧已各达齿顶，直接相连即可，齿顶很窄不必加圆弧点）
        for (R, th) in reversed(left):             # 左面 顶→根
            poly.append((R * np.cos(c0 + th), R * np.sin(c0 + th)))
        arr = np.array(poly)
        # 齿根处理：齿面最低点为基圆处(rb)，若 rf<rb 则沿径向缩至 rf(视觉近似圆角)
        if rf < rb * 0.999:
            # 首尾两点(齿根两侧)径向缩放到 rf，中间不处理
            arr[0, 0], arr[0, 1] = radial(arr[0, 0], arr[0, 1], rf)
            arr[-1, 0], arr[-1, 1] = radial(arr[-1, 0], arr[-1, 1], rf)
        # 齿根圆弧：连接本齿尾(左面根部)到下一齿首(右面根部)沿 r_root 圆
        r_root = max(rf, rb)
        a0 = np.arctan2(arr[0, 1], arr[0, 0])
        a1 = np.arctan2(arr[-1, 1], arr[-1, 0])
        # 从 a1 沿增加方向绕到 a0+2π (穿过齿槽)
        a1w = a1
        while a1w < a0 + 2 * np.pi - 1e-9:
            a1w += 2 * np.pi / z / 20
        arc = []
        t = a1
        while t < a1w - 1e-9:
            arc.append((r_root * np.cos(t), r_root * np.sin(t)))
            t += (2 * np.pi / z / 20)
        arc = np.array(arc)
        tooth = np.vstack([arr, arc])
        allp.append(tooth)
    return np.vstack(allp)


# ================================================================= 图1
def fig1():
    # ---- 装配参数
    zb = 10          # 针轮(针齿)数
    zc = 9           # 摆线轮齿数
    Rz = 100.0       # 针齿中心圆半径
    rz = 9.0         # 针齿半径
    e = 6.5          # 偏心距
    K1 = e * zb / Rz

    fig = plt.figure(figsize=(12.5, 5.6), dpi=160)
    # ---- (a) 摆线轮齿廓与几何
    ax1 = fig.add_axes([0.02, 0.05, 0.46, 0.9])
    x, y = cycloid_profile(zb, zc, Rz, rz, e)
    ax1.fill(x, y, color="#dbe9f7", lw=1.2, edgecolor=C_BLUE, zorder=2)
    # 曲轴孔(偏心轴套孔) ×3（示意位置）
    for ang in (np.pi / 2, np.pi / 2 + 2 * np.pi / 3, np.pi / 2 + 4 * np.pi / 3):
        pass
    ax1.set_aspect("equal")
    ax1.axis("off")
    # 标注齿顶/齿根
    ax1.text(0.0, Rz - rz + e - 6, "齿顶 $r_a=R_z+e-r_z$", ha="center", fontsize=9.5, color=C_RED)
    ax1.text(0.0, -(Rz - rz + e + 8), "齿根 $r_f=R_z-e-r_z$", ha="center", fontsize=9.5, color=C_GREEN)
    ax1.text(0.0, 0, "摆线轮\n(行星轮)\n$z_g=%d$" % zc, ha="center", va="center",
             fontsize=12, color=C_BLUE)
    ax1.set_title("(a) 摆线轮齿廓及其基本几何", fontsize=12.5, pad=6)

    # ---- (b) 针轮 + 摆线轮装配啮合
    ax2 = fig.add_axes([0.52, 0.05, 0.46, 0.9])
    # 针齿(绕环中心 O)
    O = (0.0, 0.0)
    angs = np.linspace(0, 2 * np.pi, zb, endpoint=False)
    # 外壳(针齿圈)
    shell = Circle(O, Rz + rz + 10, fill=False, lw=2.0, color=C_DARK)
    ax2.add_patch(shell)
    for a in angs:
        c = Circle((Rz * np.cos(a), Rz * np.sin(a)), rz, facecolor="#f5cba7",
                   edgecolor=C_ORANGE, lw=1.1, zorder=3)
        ax2.add_patch(c)
    # 摆线轮：中心在 Oc = (e,0)，齿廓自身坐标系平移到 Oc 并旋转使啮合
    # 旋转量：齿尖应指向 x+，旋转使一齿尖对准齿槽
    px, py = cycloid_profile(zb, zc, Rz, rz, e)
    # 找齿尖方向：齿顶半径处一点角度
    rr = np.hypot(px, py)
    idx = np.argmax(rr)
    ph = np.arctan2(py[idx], px[idx])
    rot = np.pi / 2 - ph          # 使一个齿尖朝上(便于看)
    px, py = _rot(px, py, rot)
    # 旋转到让齿尖对齿槽：齿尖应落于针齿空隙
    # 粗略：再转半个齿槽角使尖对槽
    px = px + O[0] + e
    py = py + O[1]
    ax2.fill(px, py, color="#aed6f1", lw=1.2, edgecolor=C_BLUE, alpha=0.85, zorder=2)
    # 中心与偏心连线
    ax2.add_patch(Circle(O, 2.5, facecolor=C_GRAY, edgecolor=C_DARK, zorder=5))
    ax2.add_patch(Circle((e, 0), 2.5, facecolor="#f9e79f", edgecolor=C_ORANGE, zorder=5))
    ax2.plot([O[0], e], [0, 0], lw=1.4, ls="--", color=C_RED, zorder=4)
    ax2.text(e / 2, 5.5, "偏心距 $e$", ha="center", fontsize=10, color=C_RED)
    ax2.text(O[0] + 2.2, 6.5, "环中心 $O$", fontsize=9, color=C_DARK)
    ax2.text(e + 3.5, 8, "摆线轮中心 $O_c$", fontsize=9, color=C_ORANGE)
    # 针齿中心圆
    ax2.add_patch(Circle(O, Rz, fill=False, ls="--", lw=1.1, color=C_ORANGE))
    ax2.text(-Rz - 13, -Rz + 4, "针齿中心圆 $R_z$", fontsize=9.5, color=C_ORANGE, rotation=90)
    # 一个针齿半径标注
    ap = np.pi / 2 + 2 * np.pi / zb
    c0 = (Rz * np.cos(ap), Rz * np.sin(ap))
    ax2.add_patch(Circle(c0, rz, fill=False, ls=":", lw=1.0, color=C_ORANGE))
    ax2.annotate("针齿 $r_z$", xy=(c0[0] + rz, c0[1]), xytext=(c0[0] + 20, c0[1] + 30),
                 fontsize=9, color=C_ORANGE,
                 arrowprops=dict(arrowstyle="->", color=C_ORANGE, lw=0.9))
    ax2.text(Rz + rz + 8, Rz + 4, "针齿圈(针轮)\n$z_b=%d$ 齿" % zb, fontsize=10,
             color=C_ORANGE, va="top")
    ax2.text(0, 8, "输出(曲轴/摆线轮)", fontsize=8.5, color=C_DARK, ha="center")
    ax2.set_aspect("equal")
    ax2.axis("off")
    ax2.set_title("(b) 摆线轮与针轮啮合(短幅外摆线—针齿啮合)", fontsize=12.5, pad=6)

    fig.suptitle("摆线针轮副（RV 第Ⅱ级）原理图   $K_1=ez_b/R_z=%.2f$" % K1,
                 fontsize=13.5, color=C_DARK)
    fig.savefig(os.path.join(OUT, "fig1_cycloid.png"), bbox_inches="tight",
                facecolor="white")
    plt.close(fig)
    print("fig1 saved")


# ================================================================= 图2
def fig2():
    m = 4.0            # 模数(示意)
    z1, z2 = 13, 17    # 小/大轮齿数(示意)
    alpha = np.radians(20)
    d1, d2 = z1 * m, z2 * m
    aw = (d1 + d2) / 2
    db1, db2 = d1 * np.cos(alpha), d2 * np.cos(alpha)

    p1 = make_gear_polygon(z1, m)
    p2 = make_gear_polygon(z2, m)

    fig, ax = plt.subplots(figsize=(10.5, 7.2), dpi=160)
    # 大轮中心在原点, 小轮在左侧
    cx1, cy1 = -aw, 0.0
    cx2, cy2 = 0.0, 0.0

    ax.plot(p1[:, 0] + cx1, p1[:, 1] + cy1, lw=1.0, color=C_BLUE)
    ax.fill(p1[:, 0] + cx1, p1[:, 1] + cy1, color="#d6eaf8", alpha=0.9, lw=0.6,
            edgecolor=C_BLUE)
    ax.plot(p2[:, 0] + cx2, p2[:, 1] + cy2, lw=1.0, color=C_GREEN)
    ax.fill(p2[:, 0] + cx2, p2[:, 1] + cy2, color="#d5f5e3", alpha=0.9, lw=0.6,
            edgecolor=C_GREEN)

    # 圆
    def add_circle(cx, cy, r, ls, c, lw=1.2):
        ax.add_patch(Circle((cx, cy), r, fill=False, ls=ls, lw=lw, color=c))

    # 小轮
    add_circle(cx1, 0, db1 / 2, "--", C_BLUE)      # 基圆
    add_circle(cx1, 0, d1 / 2, "-", C_BLUE, 1.6)   # 分度圆
    # 大轮
    add_circle(cx2, 0, db2 / 2, "--", C_GREEN)
    add_circle(cx2, 0, d2 / 2, "-", C_GREEN, 1.6)
    # 齿顶圆/齿根圆(点线)
    add_circle(cx1, 0, (d1 / 2 + m) * 0.999, ":", C_BLUE, 0.9)
    add_circle(cx2, 0, (d2 / 2 + m) * 0.999, ":", C_GREEN, 0.9)

    # 啮合线(两基圆内公切线): 斜率 tan(alpha)
    # 公切线从左基圆(-aw,0, r=rb1) 到右基圆(0,0,r=rb2)
    # 内公切线: 与连心线夹角 alpha, 距离每个基圆 rb*... 
    # 画法:过两分度圆切点(节圆点P)作与连心线夹角 α 的线
    P = (cx1 + d1 / 2, 0)   # 节点
    # 啮合线方向角 (斜向右上)
    phi_l = np.pi / 2 - alpha
    # 从节点向两侧延伸
    L = 90
    ax.plot([P[0], P[0] + L * np.cos(phi_l)], [0, L * np.sin(phi_l)],
            color=C_RED, lw=1.6, zorder=6)
    ax.plot([P[0], P[0] - L * np.cos(phi_l)], [0, -L * np.sin(phi_l)],
            color=C_RED, lw=1.6, zorder=6)
    ax.text(P[0] + L * np.cos(phi_l) + 4, L * np.sin(phi_l) + 4,
            "啮合线 $g_\\alpha$", fontsize=10, color=C_RED)

    # 中心距与压力角标注
    ax.annotate("", xy=(cx2, 6), xytext=(cx1, 6),
                arrowprops=dict(arrowstyle="<->", color=C_DARK, lw=1.2))
    ax.text((cx1 + cx2) / 2, 11, "中心距 $a_w=\\frac{m_n}{2}(z_1+z_2)$", ha="center",
            fontsize=10.5, color=C_DARK)
    # 压力角弧
    rp = 40
    th = np.linspace(0, alpha, 40)
    ax.plot(P[0] + rp * np.cos(np.pi - th) * (-1) * 0 + (P[0] + rp * np.cos(np.pi - th + np.pi)) * 0,
            [0] * 40, lw=0)
    # 在节点画压力角(用两条线夹) —— 画简化弧
    aarc = np.linspace(-alpha, 0, 30)
    ax.plot(P[0] + 34 * np.cos(np.pi / 2 + aarc),
            34 * np.sin(np.pi / 2 + aarc), color=C_DARK, lw=1.0)
    ax.text(P[0] + 40, 12, "$\\alpha$", fontsize=11, color=C_DARK)

    ax.text(cx1, -38, "小齿轮 $z_1$, $d_1$", ha="center", fontsize=10.5, color=C_BLUE)
    ax.text(cx2, -38, "大齿轮 $z_2$, $d_2$", ha="center", fontsize=10.5, color=C_GREEN)
    ax.plot([cx1, cx1 + d1 / 2], [-8, -8], color=C_BLUE, lw=3)
    ax.text(cx1 + d1 / 4, -15, "$d_1$", color=C_BLUE, fontsize=9, ha="center")
    # 基圆图例
    legend = [Line2D([0], [0], color=C_BLUE, lw=2, label="小轮(渐开线齿廓)"),
              Line2D([0], [0], color=C_GREEN, lw=2, label="大轮(渐开线齿廓)"),
              Line2D([0], [0], color=C_DARK, lw=1.4, ls="--", label="基圆 $d_b=d\\cos\\alpha_t$"),
              Line2D([0], [0], color=C_RED, lw=1.6, label="啮合线/作用线")]
    ax.legend(handles=legend, loc="upper right", fontsize=9, framealpha=0.9)
    ax.set_aspect("equal")
    ax.axis("off")
    ax.set_title("渐开线齿轮副（RV 第Ⅰ级行星传动）啮合原理图", fontsize=13, pad=10,
                 color=C_DARK)
    fig.savefig(os.path.join(OUT, "fig2_involute.png"), bbox_inches="tight",
                facecolor="white")
    plt.close(fig)
    print("fig2 saved")


# ================================================================= 图3
def fig3():
    fig = plt.figure(figsize=(12.8, 6.4), dpi=160)
    # ---------- (a) RV 两级功率流/布置
    ax = fig.add_axes([0.02, 0.08, 0.47, 0.86])
    ax.set_xlim(-100, 100)
    ax.set_ylim(-100, 100)
    ax.set_aspect("equal")
    ax.axis("off")
    # 针轮壳
    ax.add_patch(Circle((0, 0), 92, fill=False, lw=2.4, color=C_DARK))
    ax.add_patch(Circle((0, 0), 88, fill=False, lw=0.8, color=C_ORANGE, ls="--"))
    # 摆线轮盘 ×2 (同心，相位差)
    ax.add_patch(Circle((0, 0), 70, fill=True, facecolor="#d6eaf8",
                        edgecolor=C_BLUE, lw=1.2, alpha=0.55))
    ax.add_patch(Circle((0, 0), 66, fill=False, edgecolor=C_BLUE, lw=0.7, alpha=0.5,
                        ls="--"))
    # 针齿示意
    for a in np.linspace(0, 2 * np.pi, 24, endpoint=False):
        ax.add_patch(Circle((84 * np.cos(a), 84 * np.sin(a)), 3.4, facecolor=C_ORANGE,
                            edgecolor="none", alpha=0.85))
    # 三个曲轴(相位120°)
    for k, col in enumerate([C_RED, C_GREEN, "#8e44ad"]):
        a0 = np.pi / 2 + 2 * np.pi * k / 3
        # 曲轴自身轴线(小圆)
        ax.add_patch(Circle((0, 0), 10, facecolor="none", edgecolor=col, lw=1.2,
                            ls="--"))
        # 曲柄偏心(偏心量 e)
        cx, cy = 10 * np.cos(a0), 10 * np.sin(a0)
        # 相位差显示3个曲柄
        ax.add_patch(Circle((cx, cy), 3.5, facecolor=col, edgecolor=C_DARK, zorder=5))
        ax.plot([0, cx], [0, cy], lw=0.9, color=col, ls=":")
    # 中央渐开线中心轮(输入)
    ax.add_patch(Circle((0, 0), 16, facecolor="#fdebd0", edgecolor=C_ORANGE, lw=1.5,
                        zorder=4))
    ax.text(0, 0, "输入\n太阳轮", ha="center", va="center", fontsize=7.5, zorder=6)
    ax.text(0, -78, "输入：渐开线中心轮—行星轮啮合", ha="center", fontsize=9.5)
    ax.text(-93, -40, "固定针轮\n(针齿圈)", ha="center", fontsize=9, color=C_ORANGE,
            rotation=90)
    ax.text(0, 60, "摆线轮×2\n(相位错开半齿)", ha="center", fontsize=9.5, color=C_BLUE)
    for k, col in enumerate([C_RED, C_GREEN, "#8e44ad"]):
        a0 = np.pi / 2 + 2 * np.pi * k / 3
        x0, y0 = 34 * np.cos(a0), 34 * np.sin(a0)
        ax.text(x0, y0, "曲轴%d" % (k + 1), ha="center", fontsize=8.5, color=col)
    ax.set_title("(a) RV 两级传动与多曲轴布置(俯视示意)", fontsize=12, pad=8)

    # ---------- (b) 单根曲轴(偏心轴)+轴系梁模型
    ax2 = fig.add_axes([0.53, 0.08, 0.45, 0.86])
    ax2.set_xlim(0, 100)
    ax2.set_ylim(-60, 70)
    ax2.axis("off")
    # 轴向分段
    z0, z1b, z2b, z3b, z4, z5 = 8, 20, 38, 62, 80, 92   # 轴段边界
    def seg(xa, xb, ymid, h, col, ls="-", label=None):
        ax2.add_patch(Rectangle((xa, ymid - h / 2), xb - xa, h, facecolor=col,
                                edgecolor=C_DARK, lw=0.9, alpha=0.85))
    # 主轴颈（左右）与偏心段
    seg(0, z0, 0, 14, "#bdc3c7")
    seg(z0, z1b, 0, 14, "#bdc3c7")
    # 左摆线轮偏心段(偏心量 e)
    ax2.add_patch(Rectangle((z1b, 4 - 6), z2b - z1b, 12, facecolor="#aed6f1",
                            edgecolor=C_BLUE, lw=1.0))
    ax2.text((z1b + z2b) / 2, 2, "偏心段(左)\n套摆线轮1", ha="center", fontsize=7.5,
             color=C_BLUE)
    # 中轴段(装渐开线行星轮 或 过渡)
    seg(z2b, z3b, 0, 14, "#f9e79f", label="行星齿轮段")
    ax2.text((z2b + z3b) / 2, 14, "渐开线行星齿轮段\n(与中心轮啮合)", ha="center",
             fontsize=7.5, color=C_DARK)
    # 右摆线轮偏心段
    ax2.add_patch(Rectangle((z3b, 4 - 6), z4 - z3b, 12, facecolor="#aed6f1",
                            edgecolor=C_BLUE, lw=1.0))
    ax2.text((z3b + z4) / 2, 2, "偏心段(右)\n套摆线轮2", ha="center", fontsize=7.5,
             color=C_BLUE)
    seg(z4, z5, 0, 14, "#bdc3c7")
    seg(z5, 100, 0, 14, "#bdc3c7")
    # 偏心轴线示意
    ax2.plot([z1b, z2b, z3b, z4], [5, 5, 5, 5], color=C_RED, lw=1.0, ls="--")
    ax2.text(50, 26, "曲轴转动中心线(主轴颈轴线)", fontsize=8, color=C_DARK, ha="center")
    ax2.plot([0, 100], [0, 0], color=C_DARK, lw=1.2)
    # 支撑轴承
    for xb in (4, 96):
        ax2.add_patch(Rectangle((xb - 3, -30), 6, 14, facecolor="#d35400",
                                edgecolor=C_DARK, lw=0.9))
    ax2.text(4, -44, "主轴承(支承)", ha="center", fontsize=8.5, color=C_ORANGE)
    ax2.text(96, -44, "主轴承(支承)", ha="center", fontsize=8.5, color=C_ORANGE)
    # 受力箭头
    ax2.annotate("", xy=(40, 34), xytext=(40, 16),
                 arrowprops=dict(arrowstyle="->", lw=1.8, color=C_RED))
    ax2.text(43, 30, "齿轮力", color=C_RED, fontsize=8.5)
    ax2.annotate("", xy=(50, -12), xytext=(50, 0),
                 arrowprops=dict(arrowstyle="->", lw=1.8, color=C_RED))
    ax2.text(53, -10, "摆线轮力", color=C_RED, fontsize=8.5)
    # 扭矩箭头
    ax2.annotate("", xy=(98, 34), xytext=(90, 34),
                 arrowprops=dict(arrowstyle="->", lw=1.8, color=C_GREEN))
    ax2.text(88, 40, "输出扭矩 $T_{out}$", color=C_GREEN, fontsize=8.5, ha="right")
    ax2.text(50, -58, "建模：一维欧拉梁有限元（节点坐标 + 圆/偏心圆截面实常数）",
             ha="center", fontsize=9, color=C_DARK)
    ax2.set_title("(b) 单根曲轴结构示意与轴系梁模型(侧视)", fontsize=12, pad=8)

    fig.suptitle("曲轴系（RV 传动输出级）结构与受力原理图", fontsize=13.5,
                 color=C_DARK)
    fig.savefig(os.path.join(OUT, "fig3_crankshaft.png"), bbox_inches="tight",
                facecolor="white")
    plt.close(fig)
    print("fig3 saved")


if __name__ == "__main__":
    fig1()
    fig2()
    fig3()
