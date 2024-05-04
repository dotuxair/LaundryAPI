using FYP.API.Data;
using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rjf.API;
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
        [HttpGet("profile-data")]
        public async Task<IActionResult> GetProfileInfo()
        {
            try
            {

                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();
                var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return BadRequest(new { ErrorMsg = "Wrong Crendentials" });
                }
                return Ok(user);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("profile-data")]
        public async Task<IActionResult> UpdateProfileInfo(string phoneNumber, string name)
        {
            try
            {

                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();
                var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return BadRequest(new { ErrorMsg = "Wrong Crendentials" });
                }

                user.PhoneNumber = phoneNumber.ToString();
                user.Name = name;
                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = "Profile Updated Successfully" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }

        }
        /*
                [HttpGet("retailers")]
                public async Task<IActionResult> GetAllRetailers()
                {
                    try
                    {
                        var retailers = new List<RetailerDto>();
                        var users = await _dbContext.Users.ToListAsync();
                        foreach (var user in users)
                        {
                            var retailer = await _dbContext.BranchManagers.SingleOrDefaultAsync(r => r.UserId == user.Id);

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
                }*/
        /*
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
                }*/

        [HttpDelete("retailers/{id}")]
        public async Task<IActionResult> RemoveRetailer(int id)
        {
            try
            {

                var userToDelete = _dbContext.Users.Include(u => u.BranchManager).FirstOrDefault(u => u.Id == id);
                if (userToDelete == null)
                {
                    return NotFound(new { Error = "Retailer Not Found" });
                }

                _dbContext.Users.Remove(userToDelete);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Retailer Deleted Successfully" });
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

                var retailer = new BranchManager()
                {
                    UserId = retailorUser.Id,
                    BranchId = branch.Id,
                };
                await _dbContext.BranchManagers.AddAsync(retailer);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Retailer account created successfully" });

            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        /*  [HttpPut("retailers/{id}")]
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
  */
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
                var branches = await _dbContext.Branches
                    .Select(branch => new BranchDto
                    {
                        Id = branch.Id,
                        Name = branch.Name,
                        Latitude = branch.Latitude,
                        Longitude = branch.Longitude,
                        IsAssigned = _dbContext.BranchManagers.Any(r => r.BranchId == branch.Id)
                    })
                    .ToListAsync();

                return Ok(branches);
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
        /*
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
        */

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
                offer.OffPercentage = request.OffPercentage;

                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = "Offer Update Successfully with Id : " + offer.Id });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("offers")]
        public async Task<IActionResult> GetOffers()
        {
            try
            {
                var offers = await _dbContext.Offers.ToListAsync();

                var offerDtos = offers.Select(offer => new OfferDto
                {
                    Id = offer.Id,
                    Name = offer.Name,
                    StartDate = offer.StartDate,
                    EndDate = offer.EndDate,
                    OffPercentage = offer.OffPercentage,
                    Status = offer.Status,
                    ProgramId = offer.LaundryProgramId ?? 0,
                    ProgramName = GetProgramName(offer.LaundryProgramId ?? 0)
                }).ToList();

                return Ok(offerDtos);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        private string GetProgramName(int id)
        {
            var p = _dbContext.LaundryPrograms.SingleOrDefault(p => p.Id == id);
            return p!.Name;
        }
        [HttpPost("offers")]
        public async Task<IActionResult> AddOffer([FromBody] OfferDto request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                var admin = await _dbContext.Admins.SingleOrDefaultAsync(a => a.UserId == user!.Id);
                if (admin == null)
                {
                    return BadRequest(new { ErrorMsg = "Wrong Credentials" });
                }

                var off = await _dbContext.Offers.SingleOrDefaultAsync(o => o.LaundryProgramId == request.ProgramId);
                if (off != null)
                {
                    return Conflict(new { ErrorMsg = "For This Program, Offer Already Exist" });
                }
                var offer = new Offer()
                {
                    Name = request.Name,
                    EndDate = request.EndDate,
                    StartDate = request.StartDate,
                    OffPercentage = request.OffPercentage,
                    Status = "Scheduled",

                    LaundryProgramId = request.ProgramId

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
                    Price = booking.Price,
                    Status = booking.Status,
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
                var programs = await _dbContext.LaundryPrograms.ToListAsync();
                var allPrograms = new List<ProgramDto>();
                foreach (var program in programs)
                {
                    var p = new ProgramDto()
                    {
                        Id = program.Id,
                        Name = program.Name,
                        Type = program.Type,
                        Duration = program.Duration,
                        SpinSpeed = program.SpinSpeed,
                        Temprature = program.Temprature,
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
                var program = await _dbContext.LaundryPrograms.SingleOrDefaultAsync(p => p.Id == id);
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
                var program = await _dbContext.LaundryPrograms.SingleOrDefaultAsync(p => p.Id == id);
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
                var program = await _dbContext.LaundryPrograms.SingleOrDefaultAsync(p => p.Id == id);

                if (program == null)
                {
                    return NotFound(new { Error = "Program not found" });
                }

                program.Name = request.Name;
                program.Duration = Convert.ToInt32(request.Duration);
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

                var program = new LaundryProgram()
                {
                    Name = request.Name,
                    Temprature = request.Temprature,
                    Type = request.Type,
                    Duration = request.Duration,
                    SpinSpeed = request.SpinSpeed,
                    Price = request.Price,
                    AdminId = admin?.Id,
                };
                await _dbContext.LaundryPrograms.AddAsync(program);
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
                                !_dbContext.BranchManagers.Any(r => r.UserId == u.Id))
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
                    .Where(u => _dbContext.Admins.Any(a => a.UserId == u.Id) || _dbContext.BranchManagers.Any(r => r.UserId == u.Id))
                    .Select(u => new SystemUserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Role = _dbContext.Admins.Any(a => a.UserId == u.Id) ? "Admin" : "Manager"
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
                var branchesList = new List<BranchData>();
                var branches = await _dbContext.Branches.ToListAsync();
                foreach (var b in branches)
                {
                    var isAssigned = _dbContext.BranchManagers.Any(r => r.BranchId == b.Id);
                    if (!isAssigned)
                    {
                        var br = new BranchData
                        {
                            BranchId = b.Id,
                            BranchName = b.Name
                        };
                        branchesList.Add(br);
                    }
                }

                return Ok(branchesList);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpDelete("load-capacity/{id}")]
        public async Task<IActionResult> RemoveLoadCapacity(int id)
        {
            try
            {
                var loadCapacity = await _dbContext.LoadCapacity.SingleOrDefaultAsync(p => p.Id == id);
                if (loadCapacity == null)
                {
                    return NotFound(new { ErrorMsg = "Load Capacity does not exist" });
                }
                _dbContext.Remove(loadCapacity);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }

        [HttpGet("load-capacity")]
        public async Task<IActionResult> GetAllLoadCapacities()
        {
            try
            {
                var loadCapacities = await _dbContext.LoadCapacity.ToListAsync();
                return Ok(loadCapacities);
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }


        [HttpGet("load-capacity/{id}")]
        public async Task<IActionResult> GetLoadCapacity(int id)
        {
            try
            {
                var loadCapacity = await _dbContext.LoadCapacity.SingleOrDefaultAsync(p => p.Id == id);
                if (loadCapacity == null)
                {
                    return NotFound(new { ErrorMsg = "Load Capacity does not exist" });
                }
                return Ok(loadCapacity);
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }

        [HttpPut("load-capacity/{id}")]
        public async Task<IActionResult> UpdateLoadCapacity(int id, LoadCapacityDto load)
        {
            try
            {
                var loadCapacity = await _dbContext.LoadCapacity.SingleOrDefaultAsync(p => p.Id == id);
                if (loadCapacity == null)
                {
                    return NotFound(new { ErrorMsg = "Load Capacity does not exist" });
                }
                loadCapacity.Name = load.Name;
                loadCapacity.Price = load.Price;
                loadCapacity.Description = load.Description;
                await _dbContext.SaveChangesAsync();
                return Ok(new { SuccessMsg = $"LoadCapacity with Id : {loadCapacity.Id} updated successfully " });
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }


        [HttpPost("load-capacity")]
        public async Task<IActionResult> AddLoadCapacity(LoadCapacityDto load)
        {
            try
            {
                var loadCapacity = new LoadCapacity
                {
                    Name = load.Name,
                    Price = load.Price,
                    Capacity = load.Capacity,
                    Description = load.Description,
                    Type = load.Type
                };
                await _dbContext.LoadCapacity.AddAsync(loadCapacity);
                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = $"LoadCapacity with Id : {loadCapacity.Id} Added successfully " });
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }

    }
}
