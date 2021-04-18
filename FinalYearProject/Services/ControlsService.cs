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
            Word.Application application = new();
        
            application.ShowAnimation = false;
            application.Visible = false;

            object missing = System.Reflection.Missing.Value;
            
            const string templateUrl = "WordTemplate/Control_Evaluation.doc";
            string controlName = $"ControlEvaluation-{controlEvaluation.AuditName}";
            string controlUrl = $"WordControlEvaluations/{controlName}";
            Word.Document templateDocument = application.Documents.Open(templateUrl);

            templateDocument = ReplaceBookmarkText(templateDocument, "headerAuditor", controlEvaluation.LeadAuditor);
            templateDocument = ReplaceBookmarkText(templateDocument, "headerDateCreated", DateTimeOffset.Now.ToString());
            templateDocument = ReplaceBookmarkText(templateDocument, "headerJobName", controlEvaluation.AuditName);
            templateDocument = ReplaceBookmarkText(templateDocument, "headerJobRef", controlEvaluation.Id.ToString());
            
            templateDocument.SaveAs(controlUrl);

            await _jsRuntime.InvokeAsync<object>("FileSaveAs", controlUrl, controlName);
        }

        private Word.Document ReplaceBookmarkText(Word.Document document, string bookmarkName, string textToReplace)
        {
            if (document.Bookmarks.Exists(bookmarkName))
            {
                Object name = bookmarkName;
                Word.Range range = document.Bookmarks.get_Item(ref name).Range;

                range.Text = textToReplace;

                object newRange = range;
                document.Bookmarks.Add(bookmarkName, ref newRange);

                return document;
            }

            return document;
        }

    }
}