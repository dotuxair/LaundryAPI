using FYP.API.Data;
using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rjf.API;
using System;
using System.Reflection.PortableExecutable;
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
                var currentDate = DateTime.UtcNow;

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
                    BookingDate = request.Machines[0].BookingDate.Date,
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

                foreach (var m in request.Machines!.Where(mbox=>mbox.BranchId==request.BranchId))
                {
                    var bookingDetail = new BookingDetail
                    {
                        StartTime = TimeOnly.FromDateTime(m.BookingDate),
                        EndTime = TimeOnly.FromDateTime(m.EndDate),
                        Cycles = request.LaundryIntervals,
                        BookingId = bookingId,
                        MachineId = m.MachineId,
                        LaundryProgramId = m.ProgramId
                    };
                    await _dbContext.BookingDetails.AddAsync(bookingDetail);

                }

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
                
                return Ok(new { data = programs });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");

            }
        }




        [HttpGet("load-capacities/{type}")]
        public async Task<IActionResult> GetAllCapacities(string type)
        {
            try
            {
                var capacities = await _dbContext.LoadCapacity
                    .Where(m => m.Type == type).ToListAsync();


                var loads = capacities
                    .Select(m => new GetLoadCapacitiesDto
                    {
                        Name = m.Name,
                        Id = m.Id,
                        LoadCapacity = m.Capacity,
                        LoadCapacityDescription = m.Description,
                        Price = m.Price
                    });
               
                return Ok(new { data = loads });
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
                return Ok(new { data = branches });

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
                return Ok(new { data = items });
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

                return Ok(new { data = booking });
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

                var response = new AvailableMachinesDto() { };




                var neededMachinesList = new List<NeededMachineDto>();
                if (request.MachinesNeeded == 1)
                {
                    var m = new NeededMachineDto { ProgramId = request.ProgramIdOne, LoadCapacityId = request.CapacityIdOne };
                    neededMachinesList.Add(m);
                }
                else if (request.MachinesNeeded == 2)
                {

                    var m = new NeededMachineDto { ProgramId = request.ProgramIdOne, LoadCapacityId = request.CapacityIdOne };
                    var m2 = new NeededMachineDto { ProgramId = request.ProgramIdTwo, LoadCapacityId = request.CapacityIdTwo };
                    neededMachinesList.Add(m);
                    neededMachinesList.Add(m2);

                }
                else
                {
                    var m = new NeededMachineDto { ProgramId = request.ProgramIdOne, LoadCapacityId = request.CapacityIdOne };
                    var m2 = new NeededMachineDto { ProgramId = request.ProgramIdTwo, LoadCapacityId = request.CapacityIdTwo };
                    var m3 = new NeededMachineDto { ProgramId = request.ProgramIdThree, LoadCapacityId = request.CapacityIdThree };

                    neededMachinesList.Add(m);
                    neededMachinesList.Add(m2);
                    neededMachinesList.Add(m3);
                }

                var assignedBranches = await _dbContext.Branches
                    .Where(branch => branch.BranchManager != null && branch.BranchManager.BranchId != null)
                    .ToListAsync();


                double itemsPrices = 0;
                double totalPrice = 0;
                double totalPriceAfterOffer = 0;


                var allItems = await _dbContext.Products.ToListAsync();
                var allMachines = await _dbContext.Machines.ToListAsync();
                var allLoadCapacities = await _dbContext.LoadCapacity.ToListAsync();
                var allPrograms = await _dbContext.LaundryPrograms.ToListAsync();
                var allOffers = await _dbContext.Offers.ToListAsync();



                var currentDate = DateTime.Now;

                // Validate booking date and time
                if (currentDate >= request.BookingDate)
                    return BadRequest(new { ErrorMsg = "Please choose a future date and time." });

                var allBranches = new List<BranchesData>();
                var allRequestedMachines = new List<AvailableMachines>();


                // getting Branches List Where All Machines are available

                foreach (var b in assignedBranches)
                {


                    if (request.Items != null)
                    {
                        foreach (var i in request.Items)
                        {
                            var itemExist = allItems.SingleOrDefault(o => o.Id == i.ProductId && o.BranchId == b.Id);
                            if (itemExist != null)
                            {
                                itemsPrices = itemsPrices + (itemExist.Price * i.Quantity);
                            }
                        }
                    }

                    totalPrice = totalPrice + itemsPrices;

                    var selectedMachineType = request.SelectedOption == "washer" ? "Washer" : "Dryer";

                    var brachesMachines = allMachines.Where(m => m.BranchId == b.Id && m.Type == selectedMachineType).ToList();

                    int count = 0;
                    var mD = new List<AvailableMachines>();

                    foreach (var ma in neededMachinesList)
                    {

                        foreach (var m in brachesMachines)
                        {
                            var machineExist = brachesMachines.FirstOrDefault(machine => machine.LoadCapacityId == ma.LoadCapacityId && machine.Id == m.Id);
                            if (machineExist != null)
                            {
                                var loadCapacityExist = allLoadCapacities.SingleOrDefault(i => i.Id == ma.LoadCapacityId);
                                var programExist = allPrograms.SingleOrDefault(p => p.Id == ma.ProgramId);
                                var timeNeededForOneMachine = request.BookingDate.AddMinutes(programExist!.Duration * request.LaundryIntervals);
                                var bookingTime = TimeOnly.FromDateTime(request.BookingDate);
                                var date = new DateTime(request.BookingDate.Year, request.BookingDate.Month, request.BookingDate.Day,
                                                                 bookingTime.Hour, bookingTime.Minute, bookingTime.Second);
                                var endTime = TimeOnly.FromDateTime(timeNeededForOneMachine);
                                var todayDate = DateTime.Now;
                                DateTime endingDate = new DateTime(date.Year, date.Month, date.Day,
                                                                 endTime.Hour, endTime.Minute, endTime.Second);

                                var isMachineAvailable = await IsMachineAvailableAsync(machineExist!.Id, bookingTime, endTime);
                                if (count == neededMachinesList.Count)
                                {
                                    break;
                                }
                                totalPrice = totalPrice + (programExist.Price);
                                var offer = allOffers.FirstOrDefault(o => o.LaundryProgramId == programExist.Id);
                                if (offer != null)
                                {
                                    if (offer.StartDate <= todayDate && todayDate <= offer.EndDate)
                                    {
                                        var discount = (programExist.Price * offer!.OffPercentage) / 100;
                                        totalPriceAfterOffer = totalPrice - discount;
                                    }
                                }



                                if (isMachineAvailable)
                                {
                                    var mData = new AvailableMachines
                                    {
                                        MachineId = machineExist.Id,
                                        ProgramId = programExist.Id,
                                        BookingDate = date,
                                        EndDate = endingDate,
                                        BranchId = b.Id
                                    };
                                    mD.Add(mData);
                                    count += 1;
                                }
                            }

                        }

                    }
                    if (count == neededMachinesList.Count)
                    {
                        allRequestedMachines.AddRange(mD);
                        var branchDistance = await _methods.GetDrivingDistanceAsync(request.Latitude, request.Longitude, b.Latitude, b.Longitude);
                        if (branchDistance <= request.Distance)
                        {
                            allBranches.Add(new BranchesData
                            {
                                BranchName = b.Name,
                                BranchId = b.Id,
                                Distance = branchDistance,
                            });
                        }
                    }
                }

                allBranches.OrderBy(r => r.Distance);

                response.BranchesList = allBranches.DistinctBy(b => b.BranchId).ToList();
                response.MachinesList = allRequestedMachines.DistinctBy(p => p.MachineId).ToList();
                response.DiscountedPrice = totalPriceAfterOffer;
                response.Price = totalPrice;

                return Ok(new { data = response });
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

                    return Ok(new { data = response });
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
        public async Task<IActionResult> RequestBulkRequest(BulkClothRequestDto requestDto)
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
