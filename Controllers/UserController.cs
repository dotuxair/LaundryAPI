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
        [HttpPost("bookings")]
        public async Task<IActionResult> AddBooking([FromBody] BookingData request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("User email not found.");
                }

                var user = await _dbContext.Users
                    .SingleOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var booking = new Booking
                {
                    BookingDate = request.DateTime,
                    Price = request.PriceAfterDiscount,
                    BranchId = request.BranchId,
                    UserId = user.Id,
                    Status = "Scheduled"
                };



                await _dbContext.Bookings.AddAsync(booking);
                await _dbContext.SaveChangesAsync();
                int bookingId = booking.Id;

                // Update product quantities and add BookingProducts
                foreach (var productDto in request.Items!)
                {
                    var product = await _dbContext.Products
                        .SingleOrDefaultAsync(p => p.Id == productDto.ProductId);

                    if (product != null)
                    {
                        product.Quantity -= productDto.Quantity;

                        var purchasedProduct = new PurchasedProduct
                        {
                            BookingId = bookingId,
                            Quantity = productDto.Quantity,
                        };
                        await _dbContext.PurchasedProducts.AddAsync(purchasedProduct);
                    }
                }

                var bookingDetail = new BookingDetail
                {
                    StartTime = TimeOnly.FromDateTime(request.DateTime),
                    EndTime = TimeOnly.FromDateTime(request.DateTime),
                    MachineId = request.MachineId,
                    BookingId = bookingId,
                };
                await _dbContext.BookingDetails.AddAsync(bookingDetail);

                await _dbContext.SaveChangesAsync();
                return Ok("Booking Done Successfully");
            }
            catch
            {

                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpGet("programs/{type}")]
        public async Task<IActionResult> GetPrograms(string type)
        {
            try
            {
                var programs = await _dbContext.LaundryPrograms.Where(p => p.Type == type).ToListAsync();
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
                    .Where(m => m.Type == type).ToListAsync();

                /*
                                var capacities = machines
                                    .Select(m => new LoadCapacityDto
                                    {
                                        Id = m.Id,
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
                                    .DistinctBy(m => m.LoadCapacity);*/

                return Ok();
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
                var items = await _dbContext.Products.Where(i => i.ProductType == type && i.Quantity > 0).Select(item => new
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

                    Price = b.Price,
                    Status = b.Status
                }).ToListAsync();
                return Ok(booking);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }




        /*   [HttpPost("getmachines")]
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
                       var m = allMachines.SingleOrDefault(m => m.BranchId == branch.Id);
                       var availableMachinesDto = new AvailableMachinesDto
                       {
                           BranchName = branch.Name,
                           BranchId = branch.Id,
                           Distance = _methods.GetDistance(request.Latitude, request.Longitude, branch.Latitude, branch.Longitude),
                           Price = price,
                           MachineId = m != null ? m.Id :0 , 
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
           }*/

        [HttpPost("getmachines")]
        public async Task<IActionResult> GetAvailableMachineInfo([FromBody] ReceivedRequestDto request)
        {
            try
            {
                double price = 0;
                double discountedAmount = 0;

                var program = await _dbContext.LaundryPrograms.SingleOrDefaultAsync(p => p.Id == request.ProgramId);
                if (program == null)
                {
                    return BadRequest(new { ErrorMsg = "Bad Request" });
                }


                var machine = await _dbContext.Machines.SingleOrDefaultAsync(m => m.Id == request.ProgramId && m.Type == request.SelectedOption);
                if (machine == null)
                {
                    return BadRequest(new { ErrorMsg = "Bad Request" });
                }

                // managing Items Price With Quantity
                double itemsPrice = 0;
                var allItems = await _dbContext.Products.ToListAsync();

                if (request.Items != null)
                {
                    foreach (var i in request.Items)
                    {
                        var itemExist = allItems.SingleOrDefault(o => o.Id == i.ProductId);
                        if (itemExist != null)
                        {
                            itemsPrice = itemsPrice + (itemExist.Price * i.Quantity);
                        }
                    }
                }

                price = (program.Price * request.Cycles) + machine.Price + itemsPrice; // Managing Price

                var offer = await _dbContext.Offers.SingleOrDefaultAsync(o => o.LaundryProgramId == program.Id && o.Status == "Active");
                if (offer != null)
                {
                    discountedAmount = (price * offer.OffPercentage) / 100;
                }

                var totalBookingTime = request.BookingDate.AddMinutes(program.Duration * request.Cycles);
                var bookingStartTime = TimeOnly.FromDateTime(request.BookingDate);
                var bookingDate = new DateTime(request.BookingDate.Year, request.BookingDate.Month, request.BookingDate.Day,
                                                 bookingStartTime.Hour, bookingStartTime.Minute, bookingStartTime.Second);

                var bookingEndTime = TimeOnly.FromDateTime(totalBookingTime);
                var currentDate = DateTime.Now;

                // Assuming bookingDate and bookingEndTime are already defined
                DateTime endDate = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day,
                                                 bookingEndTime.Hour, bookingEndTime.Minute, bookingEndTime.Second);

                // Ai Smjh. Mry Apny Dimag Ky Opr Sy Gia Tha.

                if (currentDate.Date > bookingDate)
                {
                    return BadRequest(new { ErrorMsg = "Please choose a correct date." });
                }
                else if (currentDate.Date == bookingDate && currentDate.TimeOfDay >= request.BookingDate.TimeOfDay)
                {
                    return BadRequest(new { ErrorMsg = "Please choose a correct time." });
                }

                // Check if the selected machine is available at the given time
                var isMachineAvailable = await IsMachineAvailableAsync(machine.Id, bookingStartTime, bookingEndTime);
                if (!isMachineAvailable)
                {
                    return BadRequest(new { ErrorMsg = "Selected machine is not available at the chosen time." });
                }

                var branches = await _dbContext.Branches
             .Where(b => _dbContext.Machines.Any(m => m.BranchId == b.Id && m.Status == "Active" &&
                                                      (request.SelectedOption == "washer" ? m.Type == "Washer" : m.Type == "Dryer") &&
                                                      m.LoadCapacity == machine.LoadCapacity))
             .ToListAsync();


                var response = new AvailableMachinesDto
                {
                    Price = price,
                    DiscountedPrice = discountedAmount,
                    BookingDate = bookingDate,
                    EndDate = endDate
                };

                var allBranches = new List<BranchesData>();
                foreach (var branch in branches)
                {
                    var branchDistance = _methods.GetDistance(request.Latitude, request.Longitude, branch.Latitude, branch.Longitude);
                    if (branchDistance <= request.Distance)
                    {
                        var branchesData = new BranchesData
                        {
                            BranchName = branch.Name,
                            BranchId = branch.Id,
                            Distance = branchDistance,
                            MachineId = machine.Id,
                        };
                        allBranches.Add(branchesData);
                    }
                }
                allBranches.OrderBy(r => r.Distance);
                response.ReceivedMachines = allBranches;

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        private async Task<bool> IsMachineAvailableAsync(int machineId, TimeOnly bookingStartTime, TimeOnly bookingEndTime)
        {
            var existingBookings = await _dbContext.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.BookingDetails!.Any(bd => bd.MachineId == machineId && bd.StartTime <= bookingEndTime && bd.EndTime >= bookingStartTime))
                .ToListAsync();

            return existingBookings.Count == 0;
        }

        [HttpGet("getAllBranches")]
        public async Task<IActionResult> GetAllBranches(double latitude, double longitude)
        {
            try
            {
                var response = new List<BulkRequestBranches>();
                var branches = await _dbContext.Branches.ToListAsync();
                if (branches != null)
                {
                    foreach (var b in branches)
                    {
                        var distance = _methods.GetDistance(latitude, longitude, b.Latitude, b.Longitude);
                        var r = new BulkRequestBranches
                        {
                            BranchId = b.Id,
                            BranchName = b.Name,
                            Distance = distance,
                            Latitude = b.Latitude,
                            Longitude = b.Longitude
                        };
                        response.Add(r);
                    }
                    return Ok(response);
                }
                else
                {
                    return BadRequest(new { ErrorMsg = "Can't find branches right now." });
                }


            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }

        }

        [HttpPost("bulkCloth")]
        public async Task<IActionResult> RequestBulkRequest(BulkClothRequestDto requestDto )
        {
            try
            {
              
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { ErrorMsg = "User not found" });
                }
                else
                {
                    var bulkRequests = await _dbContext.BulkClothes.SingleOrDefaultAsync(b => b.UserId == user.Id && b.Status == "Submitted");
                    if (bulkRequests == null)
                    {
                        var request = new BulkCloth
                        {
                            RequestName = requestDto.RequestName,
                            Description = requestDto.Description,
                            PriceOffered = requestDto.Price,
                            BranchId = requestDto.BranchId,
                            PickUpDate = requestDto.PickUpDate.Date,
                            DateRequested = DateTime.Now.Date,
                            Status = "Submitted",
                            UserId = user.Id,

                        };
                        await _dbContext.BulkClothes.AddAsync(request);
                        return Ok(new { SuccessMsg = "Request Submitted Successfully. Wait for Branch Manager Response." });
                    }
                    else
                    {
                        return BadRequest(new { ErrorMsg = "Request Already Submitted." });
                    }
                }
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }

        [HttpPost("bulkCloth/{id}")]
        public async Task<IActionResult> getBulkCloth(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(new { ErrorMsg = "Id Cannt be null and empty.." });

                };
                var bulk = await _dbContext.BulkClothes.SingleOrDefaultAsync(b => b.Id == id);
                if (bulk == null)
                {
                    return NotFound(new { ErrorMsg = "Bulk Cloth request does not exist" });
                }
                else
                {
                    bulk.AcceptedPrice = bulk.PriceOffered;
                    bulk.DateAccepted = DateTime.Now.Date;
                    bulk.Status = "Accepted";
                    bulk.PaymentStatus = "Not Paid";
                    await _dbContext.SaveChangesAsync();
                    return Ok(new { SuccessMsg = "Bulk Cloth Requested Accepted Successfully." });
                }
            }

            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error," });

            }
        }
    }

}
