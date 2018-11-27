using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EWork.Models;
using EWork.Services.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.CodeAnalysis;
using Document = iTextSharp.text.Document;

namespace EWork.Services
{
    public class PdfReportGenerator : IReportGenerator
    {
        public byte[] JobsReport(User receiver, IEnumerable<Job> jobs)
        {
            var pageSize = new iTextSharp.text.Rectangle(PageSize.A4.Width, PageSize.A4.Height);
            var doc = new Document(pageSize, 45, 25, 50, 10);
            byte[] byteArray;
            using (var ms = new MemoryStream())
            {
                var pw = PdfWriter.GetInstance(doc, ms);
                doc.Open();
                doc.Add(WriteDocumentTitle(receiver));
                {
                    var jobNumber = 1;
                    foreach (var job in jobs)
                    {
                        WriteJobInfo(doc, job, jobNumber);
                        ++jobNumber;
                    }
                }
                doc.Close();
                byteArray = ms.ToArray();
                ms.Flush();
                ms.Close();
            }
            //Response.Clear();
            //Response.AddHeader("Content-Disposition", "attachment; filename=myfile.pdf");
            //Response.AddHeader("Content-Length", byteArray.Length.ToString());
            //Response.ContentType = "application/octet-stream";
            //Response.BinaryWrite(byteArray);
            return byteArray;
        }

        private Paragraph WriteDocumentTitle(User receiver)
        {
            var title = new Paragraph { SpacingAfter = 20 };
            title.Font.SetFamily("Arial");
            title.Alignment = Element.ALIGN_CENTER;
            title.Font.Size = 15f;
            title.Font.SetColor(0, 0, 0);
            title.Add($"{receiver.UserName} contracts report from {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");
            return title;
        }

        private Document WriteJobInfo(Document doc, Job job, int jobNumber)
        {
            doc.Add(AddParagraphHeader($"{jobNumber}.{job.Title}"));
            doc.Add(AddEmptyRow());
            var dateFont = new Font { Color = BaseColor.Gray };
            dateFont.SetFamily("Helvetica");
            dateFont.Size = 10f;

            doc.Add(AddParagraph(job.CreationDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture), dateFont, -15, 0));
            doc.Add(AddParagraph($"Budget: {job.Budget}$", 0, 0));

            var descriptionTitleFont = new Font();
            descriptionTitleFont.SetFamily("Courier");
            descriptionTitleFont.Size = 15f;
            descriptionTitleFont.SetColor(0, 0, 0);
            doc.Add(AddParagraph("Description:", descriptionTitleFont, spacingAfter: 5));
            doc.Add(AddParagraph(job.Description, 0));
            doc.Add(AddParagraph("Tags: " + string.Join(", ", job.JobTags.Select(jt => jt.Tag.Text))));
            doc.Add(AddEmptyRow());
            return doc;
        }

        private Paragraph AddParagraph(string paragraphText, float spacingBefore = 10f, float spacingAfter = 15f)
        {
            var p = new Paragraph { SpacingBefore = spacingBefore, SpacingAfter = spacingAfter };
            p.Font.SetFamily("Courier");
            p.Alignment = Element.ALIGN_JUSTIFIED;
            p.Font.Size = 13f;
            p.Font.SetColor(0, 0, 0);
            p.Add(paragraphText);
            return p;
        }

        private Paragraph AddParagraph(string paragraphText, Font font, float spacingBefore = 10f, float spacingAfter = 15f)
        {
            var p = new Paragraph { SpacingBefore = spacingBefore, SpacingAfter = spacingAfter, Font = font };
            p.Add(paragraphText);
            return p;
        }

        private Chunk AddParagraphHeader(string headingText)
        {
            var ch = new Chunk(headingText) { Font = { Size = 16f } };
            ch.Font.SetStyle("bold");
            ch.Font.SetColor(0, 0, 0);
            return ch;
        }

        private Paragraph AddEmptyRow() => new Paragraph { Environment.NewLine };
    }
}