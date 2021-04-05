using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalYearProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Services
{
    public class ControlsService
    {

        protected readonly DatabaseContext _context;

        public ControlsService(DatabaseContext context)
        {
            _context = context;
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
    }
}