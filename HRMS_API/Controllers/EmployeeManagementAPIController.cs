using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeManagementAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EmployeeManagementAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/CategoryAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetApplicationUser()
        {
            var users = await _db.ApplicationUser.Select(u => new Employee
            {
                EmployeeID = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Address = u.Address,
                DOB = (DateTime)u.DOB,
                join_date = (DateTime)u.join_date, // Assuming JoinDate is a property in ApplicationUser
                MobileNo = u.PhoneNumber
            }).ToListAsync();

            return Ok(users);
        }

        // GET: api/CategoryAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
            var user = await _db.ApplicationUser.FindAsync(id);

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

        // POST: api/CategoryAPI
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostApplicationUser(ApplicationUser user)
        {
            _db.ApplicationUser.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplicationUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationUser(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _db.Entry(user).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
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

        // DELETE: api/CategoryAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {
            var user = await _db.ApplicationUser.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _db.ApplicationUser.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicationUserExists(string id)
        {
            return _db.ApplicationUser.Any(e => e.Id == id);
        }

    }
}
