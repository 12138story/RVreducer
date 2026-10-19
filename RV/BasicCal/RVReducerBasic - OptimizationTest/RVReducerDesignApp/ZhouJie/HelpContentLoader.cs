using System;
using Markdig;

namespace RVReducerDesignApp
{
    public static class HelpContentLoader
    {
        // 依赖：Install-Package Markdig
        public static string RenderMarkdownToHtml(string markdown)
        {
            if (markdown == null) markdown = "";
            var pipeline = new MarkdownPipelineBuilder()
                           .UseAdvancedExtensions()
                           .Build();
            string body = Markdown.ToHtml(markdown, pipeline);
            // 加一个简单样式；你也可以加载外部 CSS 文件
            string html = "<!doctype html><html><head><meta charset='utf-8'/>" +
                "<style>body{font-family:Segoe UI, Arial; margin:18px;} pre{background:#f4f4f4;padding:10px;border-radius:4px;} h1,h2,h3{color:#333;} code{background:#eee;padding:2px 4px;border-radius:3px;}</style>" +
                "</head><body>" + body + "</body></html>";
            return html;
        }
    }
}