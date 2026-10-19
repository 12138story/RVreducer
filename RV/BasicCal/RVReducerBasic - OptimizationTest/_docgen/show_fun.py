# -*- coding: utf-8 -*-
"""按函数名精确抽取解码后源码中的函数体并打印（带行号）。
用法: python _docgen/show_fun.py <文件> <函数签名片段> [前导行数] [函数体最大行数]
"""
import sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")

path = sys.argv[1]
key = sys.argv[2]
lead = int(sys.argv[3]) if len(sys.argv) > 3 else 2
maxlines = int(sys.argv[4]) if len(sys.argv) > 4 else 120

lines = open(path, "r", encoding="utf-8").read().split("\n")
idx = None
for i, l in enumerate(lines):
    if key in l and not l.strip().startswith("//"):
        idx = i
        break
if idx is None:
    print("NOT FOUND:", key)
    sys.exit(1)
# 打印时去掉纯空行，便于阅读（行号基于原始行）
cnt = 0
for j in range(max(0, idx - lead), min(len(lines), idx + maxlines)):
    raw = lines[j]
    if raw.strip() == "":
        continue
    cnt += 1
    if cnt > maxlines:
        break
    print(f"{j+1:6d} | {raw}")
