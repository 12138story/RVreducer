# -*- coding: utf-8 -*-
"""将 GBK 编码的 C++ 源文件解码为 UTF-8 副本，便于阅读中文注释。
仅用于本文档生成任务，解码副本存放在 _docgen/decoded/ 下，不修改原文件。
"""
import os

SRC = r"d:\Workspace\softdevelop\RV\BasicCal\RVReducerBasic - OptimizationTest\RVReducerBasic"
DST = os.path.join(os.path.dirname(os.path.abspath(__file__)), "decoded")
os.makedirs(DST, exist_ok=True)

files = [
    "TANPinionCycloidGear.h",
    "TANPinionCycloidGear.cpp",
    "TANInvoluteGear.h",
    "TANInvoluteGear.cpp",
    "TANShaftSystem.h",
    "TANShaftSystem.cpp",
    "TANShaft.h",
    "TANShaft.cpp",
    "TANRVReducer.h",
    "TANRVReducer.cpp",
    "TANBearing.h",
    "TANBearing.cpp",
    "PreDefine.h",
    "Main_TAN.cpp",
]

for name in files:
    src = os.path.join(SRC, name)
    if not os.path.exists(src):
        print("MISSING:", name)
        continue
    data = open(src, "rb").read()
    # 尝试多种编码
    for enc in ("gbk", "gb18030", "utf-8", "latin-1"):
        try:
            text = data.decode(enc)
            used = enc
            break
        except Exception:
            continue
    else:
        text = data.decode("gb18030", errors="replace")
        used = "gb18030(replace)"
    out = os.path.join(DST, name)
    with open(out, "w", encoding="utf-8") as f:
        f.write(f"# SOURCE ENCODING: {used}\n")
        f.write(text)
    print("OK %-30s -> %s (%d chars)" % (name, used, len(text)))
print("ALL DONE")
