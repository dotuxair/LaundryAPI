using FYP.API.Data;
using FYP.API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FYP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "User")]
    public class UserController : ControllerBase
    {
        private readonly LaundaryDbContext _dbContext;
        private readonly Custom _methods;
        public UserController(LaundaryDbContext context, Custom methods)
        {
            _dbContext = context;
            _methods = methods;
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }
                user.PhoneNumber = request.PhoneNumber;
                user.Name = request.Name;
                await _dbContext.SaveChangesAsync();

                return Ok($"Profile Updated Successfully");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /* [HttpGet("update-offer")]
         public async Task<IActionResult> UpdateOfferStatus()
         {
             try
             {
                 var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

                 var expiredOffers = await _dbContext.Offers
                     .Where(o => o.EndDate < currentDate && o.Status == "Active")
                     .ToListAsync();

                 foreach (var offer in expiredOffers)
                 {
                     offer.Status = "Expired";
                 }

                 await _dbContext.SaveChangesAsync();


                 return Ok("Offer status updated successfully");
             }
             catch
             {

                 return StatusCode(500, "Internal Server Error");
             }
         }
 */

        [HttpGet("get-offer")]
        public async Task<IActionResult> GetOffer()
        {
            try
            {
                var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

                var offer = await _dbContext.Offers
                    .Where(o => o.StartDate <= currentDate && o.EndDate >= currentDate && o.Status == "Active")
                    .FirstOrDefaultAsync();
                if (offer == null)
                {
                    return NoContent();
                }
                return Ok(offer);

            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }

        }
        /* [HttpPost("bookings")]
         public async Task<IActionResult> AddBooking([FromBody] AddBookingDto request)
         {
             try
             {
                 var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                 if (string.IsNullOrEmpty(email))
                 {
                     return BadRequest("User email not found.");
                 }

                 var user = await _dbContext.Users
                     .SingleOrDefaultAsync(x => x.Email == email && x.UserType == "User");

                 if (user == null)
                 {
                     return NotFound("User not found.");
                 }

                 var booking = new Booking
                 {
                     BookingDate = request.Date,
                     TotalPrice = request.TotalPrice,
                     BranchId = request.BranchId,
                     UserId = user.Id,
                     Status = request.Status
                 };

                 await _dbContext.Bookings.AddAsync(booking);
                 await _dbContext.SaveChangesAsync();

                 int bookingId = booking.Id;

                 // Update product quantities and add BookingProducts
                 foreach (var productDto in request.Products!)
                 {
                     var product = await _dbContext.Products
                         .SingleOrDefaultAsync(p => p.Id == productDto.ProductId);

                     if (product != null)
                     {
                         product.Quantity -= productDto.Quantity;

                         var purchasedProduct = new PurchasedItem
                         {
                             BookingId = bookingId,
                             Quantity = productDto.Quantity,
                             LaundryItemId = product.Id
                         };
                         await _dbContext.PurchasedProducts.AddAsync(purchasedProduct);
                     }
                 }

                 // Update machine status and add BookingMachines
                 foreach (var machineDto in request.Machines!)
                 {
                     var machine = await _dbContext.Machines
                         .FirstOrDefaultAsync(m => m.LoadCapacity == machineDto.Capacity && m.BranchId == request.BranchId);

                     if (machine != null)
                     {
                         machine.Status = "Busy";

                         var bookedMachine = new BookingDetail
                         {
                             BookingId = bookingId,
                             DryCycle = request.DryCycle,
                             WashCycle = request.WashCycle,
                             MachineId = machine.Id
                         };
                         await _dbContext.BookingMachines.AddAsync(bookedMachine);
                     }
                 }

                 await _dbContext.SaveChangesAsync();

                 return Ok("Booking Done Successfully");
             }
             catch 
             {

                 return StatusCode(500, "Internal Server Error");
             }
         }

 */
        [HttpGet("programs/{type}")]
        public async Task<IActionResult> GetPrograms(string type)
        {
            try
            {
                var programs = await _dbContext.Programs.Where(p => p.Type == type).ToListAsync();
                return Ok(programs);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");

            }
        }




        [HttpGet("machines-capacities/{type}")]
        public async Task<IActionResult> GetAllCapacities(string type)
        {
            try
            {
                var machines = await _dbContext.Machines
                    .Where(m => m.MachineType == type).ToListAsync();


                var capacities = machines
                    .Select(m => new LoadCapacityDto
                    {
                        Id = m.Id,
                        LoadCapacity = m.LoadCapacity,
                        Price = m.Price
                    })
                    .Select(m => new LoadCapacityDto
                    {
                        Id = m.Id,
                        LoadCapacity = m.LoadCapacity,
                        LoadCapacityDescription = m.LoadCapacity == 5 ? "Compact ( 5 KG )" :
                                                  m.LoadCapacity == 8 ? "Standard ( 8 KG )" :
                                                  m.LoadCapacity == 12 ? "Large ( 12 KG )" :
                                                  "Extra Large ( 15 KG + )",
                        Price = m.Price
                    })
                    .DistinctBy(m => m.LoadCapacity);

                return Ok(capacities);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error: ");
            }
        }

        [HttpGet("getBranches")]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var branches = await _dbContext.Branches.Select(b => new
                {
                    Id = b.Id,
                    Name = b.Name,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude
                }).ToListAsync();
                return Ok(branches);

            }
            catch
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }

        [HttpGet("items/{type}")]
        public async Task<IActionResult> GetLaundryItems(string type)
        {
            try
            {
                var items = await _dbContext.Items.Where(i => i.Type == type && i.Quantity > 0).Select(item => new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                }).ToListAsync();
                return Ok(items);
            }
            catch
            {
                return StatusCode(500, new { Error = "Internal Server Error" });
            }
        }


        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("User email not found.");
                }

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return NotFound("User not found.");
                }
                var booking = await _dbContext.Bookings.Where(b => b.UserId == user.Id).Select(b => new BookingDto
                {
                    Id = b.Id,

                    Price = b.TotalPrice,
                    Status = b.Status
                }).ToListAsync();
                return Ok(booking);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }




        [HttpPost("getmachines")]
        public async Task<IActionResult> GetAvailableMachineInfo([FromBody] ReceivedRequestDto request)
        {
            try
            {
                double price = 0;
                var program = await _dbContext.Programs.SingleOrDefaultAsync(p => p.Id == request.ProgramId);
                if (program == null)
                {
                    return BadRequest(new { ErrorMsg = "Bad Request" });
                }
                var machine = await _dbContext.Machines.SingleOrDefaultAsync(m => m.Id == request.ProgramId && m.MachineType == request.SelectedOption);
                if (machine == null)
                {
                    return BadRequest(new { ErrorMsg = "Bad Request" });
                }

                price = (program.Price * request.Cycles) + machine.Price; // Managing Price

                var totalBookingTime = request.BookingDate.AddMinutes(program.Duration * request.Cycles);
                var bookingDate = request.BookingDate.Date;
                var bookingStartTime = TimeOnly.FromDateTime(bookingDate);
                var bookingEndTime = TimeOnly.FromDateTime(totalBookingTime);
                var currentDate = DateTime.Now;

                if (currentDate.Date > bookingDate)
                {
                    return BadRequest(new { ErrorMsg = "Please choose a correct date." });
                }
                else if (currentDate.Date == bookingDate && currentDate.TimeOfDay >= request.BookingDate.TimeOfDay)
                {
                    return BadRequest(new { ErrorMsg = "Please choose a correct time." });
                }



                var existingBookings = await _dbContext.Bookings
     .Include(b => b.BookingDetails) 
     .Where(b => b.BookingDate.Date == bookingDate.Date && 
                 b.BookingDetails!.Any(bd => bd.StartTime <= bookingEndTime && bd.EndTime >= bookingStartTime))
     .ToListAsync();



                var response = new List<AvailableMachinesDto>();

                var branches = await _dbContext.Branches.ToListAsync();

                var allMachines = await _dbContext.Machines
    .Where(m => m.Status == "Active" && (request.SelectedOption == "washer" ? m.MachineType == "Washer" : m.MachineType == "Dryer") && m.LoadCapacity == machine.LoadCapacity)
    .ToListAsync();

                foreach (var branch in branches)
                {
                    var availableMachinesDto = new AvailableMachinesDto
                    {
                        BranchName = branch.Name,
                        BranchId = branch.Id,
                        Distance = _methods.GetDistance( request.Latitude, request.Longitude, branch.Latitude, branch.Longitude),
                        Price = price
                    };
                  //  Distance = await _methods.GetDrivingDistanceAsync(request.Longitude, request.Latitude, branch.Longitude, branch.Latitude)

                    response.Add(availableMachinesDto);
                    
                }
                return Ok(response.OrderBy(r => r.Distance));
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
