//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using System.Reflection;
//using System.Text;
//using System.IO;
//using CBMSystem.Controllers;
//using CBMSystem.Helper;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Text.Json;

//namespace CBMSystem.Helper
//{
//    public class ReportExplorer
//    {
//        public static string ExportCsv<T>(IEnumerable<T> data)
//        {
//            var sb = new StringBuilder();
//            var props = typeof(T).GetProperties();

//            sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

//            foreach (var item in data)
//            {
//                var values = props.Select(p => p.GetValue(item, null));
//                sb.AppendLine(string.Join(",", values));
//            }

//            return sb.ToString();
//        }

//        public static string ExportText<T>(IEnumerable<T> data)
//        {
//            var sb = new StringBuilder();
//            foreach (var item in data)
//            {
//                sb.AppendLine(JsonSerializer.Serialize(item));
//            }
//            return sb.ToString();
//        }

//        public static byte[] ExportPdf<T>(IEnumerable<T> data)
//        {
//            using var stream = new MemoryStream();
//            var writer = new PdfWriter(stream);
//            var pdf = new PdfDocument(writer);
//            var doc = new Document(pdf);

//            var props = typeof(T).GetProperties();

//            foreach (var item in data)
//            {
//                foreach (var prop in props)
//                {
//                    var value = prop.GetValue(item)?.ToString() ?? "";
//                    doc.Add(new Paragraph($"{prop.Name}: {value}"));
//                }
//                doc.Add(new Paragraph(" "));
//            }

//            doc.Close();
//            return stream.ToArray();
//        }


        //public static string ExportCsv<T>(IEnumerable<T> data)
        //{
        //    var sb = new StringBuilder();
        //    var props = typeof(T).GetProperties();
        //    sb.AppendLine(string.Join(",", props.Select(p => p.Name)));
        //    foreach (var item in data)
        //    {
        //        var line = string.Join(",", props.Select(p => p.GetValue(item)?.ToString()?.Replace(",", " ")));
        //        sb.AppendLine(line);
        //    }
        //    return sb.ToString();
        //}

        //public static string ExportText<T>(IEnumerable<T> data)
        //{
        //    var sb = new StringBuilder();
        //    foreach (var item in data)
        //    {
        //        foreach (var prop in typeof(T).GetProperties())
        //        {
        //            sb.AppendLine($"{prop.Name}: {prop.GetValue(item)}");
        //        }
        //        sb.AppendLine(new string('-', 40));
        //    }
        //    return sb.ToString();
        //}

        //public static byte[] ExportPdf<T>(IEnumerable<T> data)
        //{
        //    using var stream = new MemoryStream();
        //    var doc = new Document();
        //    PdfWriter.GetInstance(doc, stream);
        //    doc.Open();

        //    var props = typeof(T).GetProperties();
        //    var table = new PdfPTable(props.Length);
        //    foreach (var prop in props)
        //    {
        //        table.AddCell(new Phrase(prop.Name));
        //    }

        //    foreach (var item in data)
        //    {
        //        foreach (var prop in props)
        //        {
        //            table.AddCell(new Phrase(prop.GetValue(item)?.ToString() ?? ""));
        //        }
        //    }

        //    doc.Add(table);
        //    doc.Close();
        //    return stream.ToArray();
        //}
//    }
//}
