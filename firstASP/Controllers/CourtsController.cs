using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using firstASP.Data;

namespace firstASP.Controllers;

public class CourtsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CourtsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? weekStart)
    {
        DateTime selectedDate = weekStart ?? DateTime.Today;

        DateTime monday = selectedDate.Date.AddDays(
            -(((int)selectedDate.DayOfWeek + 6) % 7));

        DateTime nextMonday = monday.AddDays(7);

        var reservations = await _context.Reservations
            .Where(r =>
                r.Status == "Approved" &&
                r.StartDateTime < nextMonday &&
                r.EndDateTime > monday)
            .OrderBy(r => r.StartDateTime)
            .ToListAsync();

        ViewBag.WeekStart = monday;
        ViewBag.PreviousWeek = monday.AddDays(-7);
        ViewBag.NextWeek = nextMonday;

        return View(reservations);
    }
}
