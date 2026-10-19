# -*- coding: utf-8 -*-
"""docx -> PDF(Word COM) -> PNG(pymupdf) 预览排版。"""
import os, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
import win32com.client
import pymupdf

base = os.path.dirname(os.path.abspath(__file__))
docx_path = os.path.join(os.path.dirname(base),
                         "RV减速器关键部件设计原理（基于C++计算引擎）.docx")
pdf_path = os.path.join(base, "preview.pdf")
png_dir = os.path.join(base, "preview")
os.makedirs(png_dir, exist_ok=True)

word = win32com.client.DispatchEx("Word.Application")
word.Visible = False
try:
    doc = word.Documents.Open(docx_path, ReadOnly=True)
    doc.SaveAs(pdf_path, FileFormat=17)  # wdFormatPDF
    doc.Close(False)
finally:
    word.Quit()
print("pdf saved")

pymupdf_doc = pymupdf.open(pdf_path)
print("pages:", pymupdf_doc.page_count)
for i in range(pymupdf_doc.page_count):
    page = pymupdf_doc[i]
    pix = page.get_pixmap(dpi=100)
    out = os.path.join(png_dir, "page_%02d.png" % (i + 1))
    pix.save(out)
print("png saved:", pymupdf_doc.page_count)
