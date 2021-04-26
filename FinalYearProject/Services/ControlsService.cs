using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;
using System.Threading.Tasks;
using FinalYearProject.Enums;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Color = Syncfusion.Drawing.Color;

namespace FinalYearProject.Services
{
    public class ControlsService
    {

        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IJSRuntime _jsRuntime;

        public ControlsService(DatabaseContext context, IWebHostEnvironment environment, IJSRuntime jsRuntime)
        {
            _context = context;
            _environment = environment;
            _jsRuntime = jsRuntime;
        }
        
        // imports all new controls. User can force update already exists controls that match on control summary.
        public async Task ImportControls(List<Control> listOfControls, bool forceUpdate = true)
        {
            foreach (var control in listOfControls)
            {
                if (await _context.Controls.AnyAsync(x => x.ControlSummary == control.ControlSummary))
                {
                    if (forceUpdate)
                    {
                        var controlQuery = await _context.Controls.Where(x => x.ControlSummary == control.ControlSummary)
                            .ToListAsync();

                        foreach (var element in controlQuery)
                        {
                            element.ControlExpected = control.ControlExpected;
                            element.ControlTest = control.ControlTest;
                        }
                    }
                }
                else
                {
                    await _context.AddAsync(control);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Control>> FetchAllControls()
        {
            return await _context.Controls.ToListAsync();
        }

        public async Task<List<ControlEvaluations>> FetchAllInReviewControlEvals()
        {
            return await _context.ControlEvaluations.Where(x => x.ControlStage == ControlStage.InReview).ToListAsync();
        }

        public async Task CreateControlEvaluationDocument(ControlEvaluations controlEvaluation)
        {
            const string templateUrl = "WordTemplate/Control_Evaluation.doc";
            string controlName = $"ControlEvaluation-{controlEvaluation.AuditName}";
            string controlUrl = $"WordControlEvaluations/{controlName}";
            
            WordDocument wordDocument = new WordDocument();
            FileStream fileStreamPath =
                new FileStream($@"{templateUrl}", FileMode.Open, FileAccess.Read, FileShare.Write);
            wordDocument.Open(fileStreamPath, FormatType.Automatic);

            InsertHeadingsDocument(new BookmarksNavigator(wordDocument), controlEvaluation);
            InsertControlTableDocument(wordDocument.Sections[0], controlEvaluation);
            
            MemoryStream stream = new MemoryStream();
            wordDocument.Save(stream, FormatType.Docx);

            wordDocument.Close();
            stream.Position = 0;

            await _jsRuntime.SaveAs("ControlEvaluation.docx", stream.ToArray());
        }

        private void InsertHeadingsDocument(BookmarksNavigator bookmarksNavigator, ControlEvaluations controlEvaluation)
        {
            bookmarksNavigator.MoveToBookmark("headerAuditor");
            bookmarksNavigator.InsertText(controlEvaluation.LeadAuditor);
            bookmarksNavigator.MoveToBookmark("headerDateCreated");
            bookmarksNavigator.InsertText(DateTimeOffset.Now.ToString("d"));
            bookmarksNavigator.MoveToBookmark("headerJobName");
            bookmarksNavigator.InsertText(controlEvaluation.AuditName);
            bookmarksNavigator.MoveToBookmark("headerJobRef");
            bookmarksNavigator.InsertText(controlEvaluation.Id.ToString());
        }

        private void InsertControlTableDocument(WSection section, ControlEvaluations controlEvaluation)
        {
            var table = section.Tables[0] as WTable;
            int reference = 1;
            if (table == null)
            {
                throw new Exception("An error has occurred. Please try again.");
            }
            foreach (var controlList in controlEvaluation.ControlsList)
            {
                reference = InsertRowIntoTable(table.AddRow(), controlList.Control, reference);
            }
            
        }

        private int InsertRowIntoTable(WTableRow row, Control control, int reference)
        {
            var cellCollection = row.Cells;
            row.RowFormat.BackColor = Color.White;
            ChangeFontDetails(cellCollection[0].AddParagraph().AppendText(reference.ToString()));
            ChangeFontDetails(cellCollection[1].AddParagraph().AppendText(control.ControlSummary));
            ChangeFontDetails(cellCollection[2].AddParagraph().AppendText(control.ControlExpected));
            ChangeFontDetails(cellCollection[3].AddParagraph().AppendText(control.ControlTest));
            return ++reference; 
        }

        private void ChangeFontDetails(
            IWTextRange textRange, string font = "Century Gothic", int fontSize = 10, bool bold = false)
        {
            textRange.CharacterFormat.FontName = font;
            textRange.CharacterFormat.FontSize = fontSize;
            textRange.CharacterFormat.Bold = bold;
        }
    }
}