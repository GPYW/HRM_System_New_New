using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceManagementAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AttendanceManagementAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/CategoryAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceManagement>>> GetAttendanceManagementr()
        {
            return await _db.AttendanceTimeTable.ToListAsync();
        }

        // POST: api/CategoryAPI
        [HttpPost]
        public async Task<ActionResult<AttendanceManagement>> PostAttendanceManagement(AttendanceManagement att)
        {
            _db.AttendanceTimeTable.Add(att);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAttendanceManagementr), new { id = att.RecordId }, att);
        }
    }
}
