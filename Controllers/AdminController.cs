using FYP.API.Data;
using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FYP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Admin")]

    public class AdminController : ControllerBase
    {
        private readonly LaundaryDbContext _dbContext;
        public AdminController(LaundaryDbContext laundaryDbContext)
        {
            _dbContext = laundaryDbContext;
        }

        [HttpGet("retailers")]
        public async Task<IActionResult> GetAllRetailers()
        {
            try
            {
                var retailers = new List<RetailerDto>();
                var users = await _dbContext.Users.ToListAsync();
                foreach (var user in users)
                {
                    var retailer = await _dbContext.Retailers.SingleOrDefaultAsync(r => r.UserId == user.Id);

                    if (retailer != null)
                    {
                        var r = new RetailerDto()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Password = user.Password,
                            BranchId = retailer!.BranchId
                        };
                        retailers.Add(r);
                    }
                }

                return Ok(retailers);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("retailers/{id}")]
        public async Task<IActionResult> GetRetailer(int id)
        {
            try
            {
                var retailer = await _dbContext.Users.SingleOrDefaultAsync(r => r.Id == id);
                if (retailer == null)
                {
                    return NotFound(new { Error = "RetailerDto Not Found" });
                }
                return Ok(retailer);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("retailers/{id}")]
        public async Task<IActionResult> RemoveRetailer(int id)
        {
            try
            {
                var retailer = await _dbContext.Users.SingleOrDefaultAsync(r => r.Id == id);
                if (retailer == null)
                {
                    return NotFound(new { Error = "RetailerDto Not Found" });
                }

                _dbContext.Users.Remove(retailer);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "RetailerDto Deleted Successfully" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("retailers")]
        public async Task<IActionResult> AddRetailer([FromBody] RetailerDto request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == request.BranchId);
                    if (branch == null)
                    {
                        return NotFound(new { Error = "Branch Not Found" });
                    }
                    var retailers = await _dbContext.Users.SingleOrDefaultAsync(r => r.Email == request.Email);
                    if (retailers != null)
                    {
                        return Conflict(new { Error = "Email Already Exist" });
                    }
                    var retailorUser = new User()
                    {
                        Name = request.Name,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        Password = request.Password,
                    };
                    await _dbContext.Users.AddAsync(retailorUser);
                    await _dbContext.SaveChangesAsync();

                    var retailer = new Retailer()
                    {
                        UserId = retailorUser.Id,
                        BranchId = branch.Id,
                    };
                    await _dbContext.Retailers.AddAsync(retailer);
                    await _dbContext.SaveChangesAsync();

                    return Ok(new { Success = "Retailer account created successfully" });
                }
                return BadRequest(new { Error = "Model is not validated" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut("retailers/{id}")]
        public async Task<IActionResult> UpdateRetailer(int id, [FromBody] RetailerDto request)
        {
            try
            {
                var retailer = await _dbContext.Users.SingleOrDefaultAsync(r => r.Id == id);
                if (retailer == null)
                {
                    return NotFound(new { Error = "RetailerDto Not Found" });
                }

                retailer.Name = request.Name;
                retailer.Password = request.Password;
                retailer.PhoneNumber = request.PhoneNumber;

                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "RetailerDto Updated Successfully" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("branches")]
        public async Task<IActionResult> AddBranch([FromBody] BranchDto request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();
                    var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
                    var admin = await _dbContext.Admins.SingleOrDefaultAsync(a => a.UserId == user!.Id);

                    var branch = new Branch()
                    {
                        Name = request.Name,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        AdminId = admin!.Id,
                    };
                    await _dbContext.Branches.AddAsync(branch);

                    await _dbContext.SaveChangesAsync();

                    return Ok($"Brach Added With Name : " + branch.Name);

                }
                return BadRequest(new { Error = "Branch Model is not validated" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("branches")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                return Ok(await _dbContext.Branches.ToListAsync());
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("branches/{id}")]
        public async Task<IActionResult> GetBranch(int id)
        {
            try
            {
                var branch = await _dbContext.Branches.SingleOrDefaultAsync(x => x.Id == id);
                if (branch == null)
                {
                    return NotFound(new { Error = "Branch Not Found" });
                }
                return Ok(branch);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("branches/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                var branch = await _dbContext.Branches.SingleOrDefaultAsync(x => x.Id == id);
                if (branch == null)
                {
                    return NotFound(new { Error = "Branch Not Found" });
                }
                _dbContext.Branches.Remove(branch);

                await _dbContext.SaveChangesAsync();
                return Ok(new { Success = "Branch Deleted Successfully" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut("branches/{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchDto request)
        {
            try
            {
                var branch = await _dbContext.Branches.SingleOrDefaultAsync(x => x.Id == id);
                if (branch == null)
                {
                    return NotFound(new { Error = "Branch Not Found" });
                }
                branch.Name = request.Name;
                branch.Latitude = request.Latitude;
                branch.Longitude = request.Longitude;

                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Branch Updated Successfully" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("offers")]
        public async Task<IActionResult> GetAllOffers()
        {
            try
            {

                return Ok(await _dbContext.Offers.ToListAsync());
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("offers/{id}")]
        public async Task<IActionResult> GetOffer(int id)
        {
            try
            {
                var offer = await _dbContext.Offers.SingleOrDefaultAsync(o => o.Id == id);
                if (offer == null)
                {
                    return NotFound(new { Error = "Offer Not Found" });
                }
                return Ok(offer);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("offers/{id}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();
                var offer = await _dbContext.Offers.SingleOrDefaultAsync(o => o.Id == id);
                if (offer == null)
                {
                    return NotFound(new { Error = "Offer Not Found" });
                }
                _dbContext.Offers.Remove(offer);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Offer Delete Successfully with Id : " + offer.Id });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("offers/{id}")]
        public async Task<IActionResult> UpdateOffer(int id, [FromBody] OfferDto request)
        {
            try
            {

                var offer = await _dbContext.Offers.SingleOrDefaultAsync(o => o.Id == id);
                if (offer == null)
                {
                    return NotFound(new { Error = "Offer Not Found" });
                }
                offer.Name = request.Name;
                offer.StartDate = request.StartDate;
                offer.EndDate = request.EndDate;
                offer.Percentage = request.OffPercentage;

                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Offer Update Successfully with Id : " + offer.Id });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("offers")]
        public async Task<IActionResult> AddOffer([FromBody] OfferDto request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                /*var admin = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);*/

                var offers = await _dbContext.Offers.SingleOrDefaultAsync(o => o.Status == "Active");
                if (offers != null)
                {
                    return Conflict(new { Error = "Offer Already Exist" });
                }
                var offer = new Offer()
                {
                    Name = request.Name,
                    EndDate = request.EndDate,
                    StartDate = request.StartDate,
                    Percentage = request.OffPercentage,
                    Status = "Active",
                };

                await _dbContext.AddAsync(offer);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Offer Added Successfully with Id : " + offer.Id });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _dbContext.Bookings.ToListAsync();

                List<BookingDetailDto> bookingDetailDtos = bookings.Select(booking => new BookingDetailDto
                {
                    Id = booking.Id,
                    Price = booking.TotalPrice,
                    Status = booking.Status
                }).ToList();
                return Ok(bookingDetailDtos);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

       /* [HttpGet("bookings/{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            try
            {
                var booking = await _dbContext.Bookings.SingleOrDefaultAsync(b => b.Id == id);
                if (booking == null)
                {
                    return NotFound(new { Error = "Booking Not Found" });
                }

                var products = await _dbContext.Items
                    .Where(p => p.B == booking.Id)
                    .ToListAsync();

                var machines = await _dbContext.Items
                    .Where(p => p.BookingId == booking.Id)
                    .ToListAsync();

                var bookingDetailDto = new BookingDetailDto
                {
                    Id = booking.Id,
                    BookingDate = booking.BookingDate,

                    Price = booking.TotalPrice,
                    Status = booking.Status,
                    Products = products.Select(p => new PurchasedProducts { Id = p.Id, Quantity = p.Quantity }).ToList(),
                    // Machines = machines.Select(m => new MachineCycle { Id=m.Id,DryCycle = m.DryCycle , WashCycle = m.WashCycle}).ToList()
                };

                return Ok(bookingDetailDto);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
*/
        [HttpGet("programs")]
        public async Task<IActionResult> GetAllPrograms()
        {
            try
            {
                var programs = await _dbContext.Programs.ToListAsync();
                var allPrograms = new List<ProgramDto>();
                foreach (var program in programs)
                {
                    var p = new ProgramDto()
                    {
                        Id = program.Id,
                        Name = program.Name,
                        Type = program.Type,
                        Duration = program.Duration,

                        SpinSpeed = program.Type == "Washer" ? program.SpinSpeed : "",
                        AirSpeed = program.Type == "Dryer" ? program.SpinSpeed : "",
                        WaterTemp = program.Type == "Washer" ? program.Temprature : "",
                        AirTemp = program.Type == "Dryer" ? program.Temprature : "",
                        Price = program.Price
                    };

                    allPrograms.Add(p);
                }
                return Ok(allPrograms);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("programs/{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            try
            {
                var program = await _dbContext.Programs.SingleOrDefaultAsync(p => p.Id == id);
                if (program == null)
                {
                    return NotFound(new { Error = "Program does not exist" });
                }
                _dbContext.Remove(program);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("programs/{id}")]
        public async Task<IActionResult> GetSingleProgram(int id)
        {
            try
            {
                var program = await _dbContext.Programs.SingleOrDefaultAsync(p => p.Id == id);
                if (program == null)
                {
                    return NotFound(new { Error = "Program does not exist" });
                }
                return Ok(program);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("programs/{id}")]
        public async Task<IActionResult> UpdateProgram(int id, ProgramDto request)
        {
            try
            {
                var program = await _dbContext.Programs.SingleOrDefaultAsync(p => p.Id == id);

                if (program == null)
                {
                    return NotFound(new { Error = "Program not found" });
                }

                program.Name = request.Name;
                program.Temprature = request.Type == "washer" ? request.WaterTemp : request.AirTemp;
                program.Type = request.Type;
                program.Duration = request.Duration;
                program.SpinSpeed = request.SpinSpeed;
                program.Price = request.Price;

                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("programs")]
        public async Task<IActionResult> AddProgram(ProgramDto request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();
                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);

                var admin = await _dbContext.Admins.SingleOrDefaultAsync(a => a.UserId == user!.Id);

                var program = new Program()
                {
                    Name = request.Name,
                    Temprature = request.Type == "washer" ? request.WaterTemp : request.AirTemp,
                    Type = request.Type,
                    Duration = request.Duration,
                    SpinSpeed = request.Type == "washer" ? request.SpinSpeed : request.AirSpeed,
                    Price = request.Price,
                    AdminId = admin?.Id,
                };
                await _dbContext.Programs.AddAsync(program);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _dbContext.Users
                    .Where(u => !_dbContext.Admins.Any(a => a.UserId == u.Id) &&
                                !_dbContext.Retailers.Any(r => r.UserId == u.Id))
                    .ToListAsync();

                return Ok(users);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("admin-and-retailer-users")]
        public async Task<IActionResult> GetAdminAndRetailerUsers()
        {
            try
            {
                var adminAndRetailerUsers = await _dbContext.Users
                    .Where(u => _dbContext.Admins.Any(a => a.UserId == u.Id) || _dbContext.Retailers.Any(r => r.UserId == u.Id))
                    .Select(u => new SystemUserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Role = _dbContext.Admins.Any(a => a.UserId == u.Id) ? "Admin" : "Retailer"
                    })
                    .ToListAsync();

                return Ok(adminAndRetailerUsers);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpGet("unassigned-branches")]
        public async Task<IActionResult> GetUnAssignedBranches()
        {
            try
            {
                var unassignedBranches = await _dbContext.Branches
                    .Where(b => !_dbContext.Retailers.Any(r => r.BranchId == b.Id))
                    .Select(b => new
                    {
                        BranchName = b.Name,
                        BranchId = b.Id
                    })
                    .ToListAsync();

                return Ok(unassignedBranches);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}
