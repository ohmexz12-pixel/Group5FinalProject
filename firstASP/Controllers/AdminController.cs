
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using firstASP.Data;

namespace firstASP.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalReservations =
            await _context.Reservations.CountAsync();

        ViewBag.PendingRequests =
            await _context.Reservations.CountAsync(
                r => r.Status == "Pending");

        ViewBag.ApprovedBookings =
            await _context.Reservations.CountAsync(
                r => r.Status == "Approved");

        ViewBag.RejectedBookings =
            await _context.Reservations.CountAsync(
                r => r.Status == "Rejected");

        return View();
    }
}
