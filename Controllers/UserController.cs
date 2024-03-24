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

        [HttpGet("update-offer")]
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

*/   /*
        [HttpGet("dry-cycles")]
        public IActionResult GetDryCycles()
        {
            try
            {
                var cycle = new DryCycle();
                cycle.LoadDryCycles();
                return Ok(cycle.DryCycles);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");

            }

        }*/

      /*  [HttpGet("wash-cycles")]

        public IActionResult GetWashCycles()
        {
            var washCycle = new WashCycle();
            washCycle.LoadAllWashCycles();
            return Ok(washCycle.WashCycles);
        }*/
/*
        [HttpGet("machine-capacities")]
        public IActionResult GetAllCapacities()
        {
            var capacity = new MachineCapacity();
            capacity.LoadMachineCapacities();
            return Ok(capacity.MachineCapacities);
        }
*/
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
                var booking = await _dbContext.Bookings.Where(b=>b.UserId == user.Id).Select(b => new BookingDto
                {
                    Id = b.Id,
                    Date = b.BookingDate,
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
/*

        [HttpGet("getmachines")]
        public async Task<IActionResult> GetAvailableMachineInfo([FromBody] ReceiveMachineRequestDto request)
        {
            try
            {
                var response = new List<AvailableMachinesDto>();
                var branches = await _dbContext.Branches.ToListAsync();

                var allMachines = await _dbContext.Machines.Where(m => m.Status == "Active").ToListAsync();

                foreach (var br in branches)
                {
                    bool conditionMet = false;
                    for (int i = 0; i < request.Requirements!.Count; i++)
                    {
                        var machines = allMachines.Where(m => m.BranchId == br.Id && m.LoadCapacity == request.Requirements[i].Capacity);
                        if (machines == null)
                        {
                            conditionMet = false;
                        }
                        conditionMet = true;
                    }

                    if (conditionMet)
                    {
                        var distance = _methods.GetDistance(request.Longitude, request.Latitude, br.Longitude, br.Latitude);
                        var availableMachinesDto = new AvailableMachinesDto
                        {
                            BranchName = br.Name,
                            Distance = distance,
                            BranchId = br.Id
                        };
                        response.Add(availableMachinesDto);
                    }
                }

                return Ok(response.OrderBy(r=>r.Distance));
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }*/
    }
}
