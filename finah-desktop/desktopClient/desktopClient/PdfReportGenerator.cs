using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace sprint_1_def
{
   public class PdfReportGenerator
    {
         public void reportToPdf(Report rep, int id, string wheretosave)
        {
            PdfDocument pdf = new PdfDocument();
            PdfPage pdfPage = pdf.AddPage();
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            XFont font = new XFont("Verdana", 14, XFontStyle.Regular);
            XFont titlefont = new XFont("Verdana", 14, XFontStyle.Bold);

            int currentheight = 0;
            int currentwidth = 0;

            graph.DrawString("Rapport #" + id, titlefont, XBrushes.Black, new XRect(currentwidth, currentheight, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopCenter);

            currentheight += 40;

            foreach (Question q in rep.QuestionList) 
            {
                currentwidth = 0;
                if(anyHelpRequest(q, rep.AnswerList))
                {

                    string[] textchunks;
                    textchunks = split(q.Text, 60).ToArray();
                    for (int i = 0; i < textchunks.Length; i++)
                    {
                        graph.DrawString(textchunks[i], font, XBrushes.Black, new XRect(currentwidth, currentheight, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                        currentheight += 20;
                    }
                      
                    
                    currentwidth += 20;
                    for(int i = 0; i < rep.AnswerList.Count && i < rep.ClientList.Count; i++)
                    {
                        for(int j = 0; j < rep.AnswerList[i].Count; j++)
                        {
                            if(rep.AnswerList[i][j].QuestionId == q.Id)
                            {
                                string toPrint = "";
                                toPrint += rep.ClientList[i].Function;
                                toPrint += " Score: ";
                                toPrint += rep.AnswerList[i][j].Score;
                                if (rep.AnswerList[i][j].Help)
                                    toPrint += " Hulp gevraagd";
                                else
                                    toPrint += " Geen hulp gevraagd";
                                graph.DrawString(toPrint, font, XBrushes.Black, new XRect(currentwidth, currentheight, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                                currentheight += 20;
                            }
                        }
                    }
                }
                
            }

            //Environment.SpecialFolder.UserProfile), id.ToString()
            pdf.Save(wheretosave);
        }

        private bool anyHelpRequest(Question q, List<List<Answer>> all)
        {
            foreach (List<Answer> al in all)
                foreach (Answer a in al)
                    if (a.QuestionId == q.Id && a.Help)
                        return true;

            return false;
        }

        private IEnumerable<string> split(string str, int maxlength)
        {
            for (int index = 0; index < str.Length; index += maxlength)
            {
                yield return str.Substring(index, Math.Min(maxlength, str.Length - index));
            }
        }
    }
    }


