using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileSettingsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject ApplicationDbContext
        public ProfileSettingsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoryAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
            var user = _context.ApplicationUser.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var userModel = new Employee
            {
                EmployeeID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                DOB = (DateTime)user.DOB,
                join_date = (DateTime)user.join_date, // Assuming JoinDate is a property in ApplicationUser
                MobileNo = user.PhoneNumber
            };

            return Ok(userModel);
        }

        // PUT: api/CategoryAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }

}
