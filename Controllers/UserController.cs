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
        public async Task<IActionResult> UpdateProfile(string phoneNumber, string name)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }
                user.PhoneNumber = phoneNumber;
                user.Name = name;
                await _dbContext.SaveChangesAsync();

                return Ok($"Profile Updated Successfully");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> Password(string password)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }
                user.Password = password;
                await _dbContext.SaveChangesAsync();

                return Ok($"Password Updated Successfully");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("update-profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value.ToString();

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }


                return Ok(user);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("update-offers-status")]
        public async Task<IActionResult> UpdateOffersStatus()
        {
            try
            {
                var currentDate = DateTime.Now.Date;

                var offersToUpdate = await _dbContext.Offers.ToListAsync();

                foreach (var offer in offersToUpdate)
                {
                    if (currentDate > offer.EndDate.Date)
                    {
                        offer.Status = "Expired";
                    }
                    else if (currentDate >= offer.StartDate.Date && currentDate <= offer.EndDate.Date)
                    {
                        offer.Status = "Active";
                    }
                }

                if (offersToUpdate.Any())
                {
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("Offers status updated successfully");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("offers")]
        public async Task<IActionResult> GetActiveOffers(string showBy = "all")
        {
            try
            {
                if (showBy == "all")
                {

                    var activeOffers = await _dbContext.Offers
                        .Join(_dbContext.LaundryPrograms,
                              offer => offer.LaundryProgramId,
                              program => program.Id,
                              (offer, program) => new
                              {
                                  Offer = offer,
                                  ProgramName = program.Name
                              })
                        .AsNoTracking()
                        .ToListAsync();

                    return Ok(activeOffers);
                }
                else
                {
                    var currentDate = DateTime.Now;

                    var activeOffers = await _dbContext.Offers
                        .Where(o => o.StartDate <= currentDate && o.EndDate >= currentDate && o.Status != "Expired")
                        .Join(_dbContext.LaundryPrograms,
                              offer => offer.LaundryProgramId,
                              program => program.Id,
                              (offer, program) => new
                              {
                                  Offer = offer,
                                  ProgramName = program.Name
                              })
                        .AsNoTracking()
                        .ToListAsync();

                    return Ok(activeOffers);
                }
               
            }
            catch
            {
                return StatusCode(500, "Internal Server Error:");

            }
        }



        [HttpGet("get-programs")]
        public async Task<IActionResult> GetPrograms()
        {
            try
            {
                return Ok(await _dbContext.LaundryPrograms.ToListAsync());
            }
            catch
            {
                return StatusCode(500, "Internal Server Error:");
            }
        }

        [HttpGet("get-load")]
        public async Task<IActionResult> GetLoadCapacities()
        {
            try
            {
                return Ok(await _dbContext.LoadCapacity.ToListAsync());
            }
            catch
            {
                return StatusCode(500, "Internal Server Error:");
            }
        }
        [HttpPut("completed/{id}")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            return await UpdateBookingStatus(id, "Completed", "Ready");
        }

        [HttpPut("in-progress/{id}")]
        public async Task<IActionResult> InProgressBooking(int id)
        {
            return await UpdateBookingStatus(id, "In-Progress", "Booked");
        }

        [HttpPut("canceled/{id}")]
        public async Task<IActionResult> CanceledBooking(int id)
        {
            return await UpdateBookingStatus(id, "Cancelled", "Ready");
        }

        private async Task<IActionResult> UpdateBookingStatus(int id, string bookingDetailStatus, string machineStatus)
        {
            var bookingDetail = await _dbContext.BookingDetails.FirstOrDefaultAsync(bd => bd.Id == id);
            if (bookingDetail == null)
            {
                return NotFound("Booking detail not found");
            }

            bookingDetail.Status = bookingDetailStatus;

            var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingDetail.BookingId);
            if (booking == null)
            {
                return NotFound("Booking not found");
            }

            var relatedBookingDetails = await _dbContext.BookingDetails
                .Where(bd => bd.BookingId == booking.Id)
                .ToListAsync();

            if (relatedBookingDetails.All(bd => bd.Status == bookingDetailStatus))
            {
                booking.Status = bookingDetailStatus;
            }

            if (machineStatus != null)
            {
                var machine = await _dbContext.Machines.FirstOrDefaultAsync(m => m.Id == bookingDetail.MachineId);
                if (machine != null)
                {
                    machine.Status = machineStatus;
                }
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
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
                    BookingDate = request.Machines![0].BookingDate.Date,
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

                foreach (var m in request.Machines!.Where(mbox => mbox.BranchId == request.BranchId))
                {
                    var bookingDetail = new BookingDetail
                    {
                        StartTime = TimeOnly.FromDateTime(m.BookingDate),
                        EndTime = TimeOnly.FromDateTime(m.EndDate),
                        Cycles = request.LaundryIntervals,
                        BookingId = bookingId,
                        MachineId = m.MachineId,
                        LaundryProgramId = m.ProgramId,
                        Status = "Scheduled"
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

        [HttpGet("booked-machines")]
        public async Task<IActionResult> GetBookedMachines()
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

                var bookedMachines = await _dbContext.BookingDetails
                    .Where(bd => bd.Booking!.UserId == user.Id)
                    .Select(bd => new BookedMachineDto
                    {
                        BookingDetailId = bd.Id,
                        BookingDate = bd.Booking!.BookingDate,
                        StartTime = bd.StartTime,
                        EndTime = bd.EndTime,
                        Status = bd.Status,
                        MachineCode = bd.Machine!.MachineCode,
                        Price = bd.Booking.Price,
                        MachineType = bd.Machine.Type,
                        ProgramName = bd.LaundryProgram!.Name,  // Assuming BookingDetail has a navigation property to Program
                        BranchName = bd.Booking.Branch!.Name  // Assuming Booking has a navigation property to Branch
                    })
                    .OrderBy(bd => bd.BookingDate)
                    .ToListAsync();

                var filteredBookedMachines = bookedMachines
                    .Where(b => b.Status == "In-Progress" || b.Status == "Scheduled")
                    .ToList();

                return Ok(filteredBookedMachines);
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


        [HttpGet("get-offers")]
        public async Task<IActionResult> GetActiveOffers()
        {
            try
            {
                var offers = await _dbContext.Offers.Where(p => p.Status == "Active").ToListAsync();
                return Ok(offers);
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

                return Ok(loads);
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

        [HttpGet("items")]
        public async Task<IActionResult> GetLaundryItems(string type, int branchId)
        {
            try
            {
                var items = await _dbContext.Products.Where(i => i.ProductType == type.ToUpper() && i.Quantity > 0 && i.BranchId == branchId).Select(item => new
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

        [HttpPost("getmachines")]
        public async Task<IActionResult> GetAvailableMachineInfo([FromBody] ReceivedRequestDto request)
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
                var getAlreadyBookedBranch = _dbContext.Bookings.Where(bd => bd.Status == "Scheduled" || bd.Status == "In-Progress").FirstOrDefault();


                bool isPriceSet = false;

                var response = new AvailableMachinesDto();
                var neededMachinesList = new List<NeededMachineDto>();

                // Populate neededMachinesList based on request.MachinesNeeded
                if (request.MachinesNeeded == 1)
                {
                    neededMachinesList.Add(new NeededMachineDto { ProgramId = request.ProgramIdOne, LoadCapacityId = request.CapacityIdOne });
                }
                else if (request.MachinesNeeded == 2)
                {
                    neededMachinesList.Add(new NeededMachineDto { ProgramId = request.ProgramIdOne, LoadCapacityId = request.CapacityIdOne });
                    neededMachinesList.Add(new NeededMachineDto { ProgramId = request.ProgramIdTwo, LoadCapacityId = request.CapacityIdTwo });
                }
                else
                {
                    neededMachinesList.Add(new NeededMachineDto { ProgramId = request.ProgramIdOne, LoadCapacityId = request.CapacityIdOne });
                    neededMachinesList.Add(new NeededMachineDto { ProgramId = request.ProgramIdTwo, LoadCapacityId = request.CapacityIdTwo });
                    neededMachinesList.Add(new NeededMachineDto { ProgramId = request.ProgramIdThree, LoadCapacityId = request.CapacityIdThree });
                }

                var assignedBranches = await _dbContext.Branches
                    .Where(branch => branch.BranchManager != null)
                    .ToListAsync();

                var allMachines = await _dbContext.Machines.ToListAsync();
                var allLoadCapacities = await _dbContext.LoadCapacity.ToListAsync();
                var allPrograms = await _dbContext.LaundryPrograms.ToListAsync();


                var currentDate = DateTime.Now;

                if (currentDate >= request.BookingDate)
                    return BadRequest(new { ErrorMsg = "Please choose a future date and time." });

                var allBranches = new List<BranchesData>();
                var allRequestedMachines = new List<AvailableMachines>();

                foreach (var branch in assignedBranches)
                {
                   

                    // Initialize branch specific price
                    double branchTotalPrice = 0;
                    double dPrice = 0;

                    var selectedMachineType = request.SelectedOption == "washer" ? "Washer" : "Dryer";
                    var branchMachines = allMachines.Where(m => m.BranchId == branch.Id && m.Type == selectedMachineType && m.Status == "Ready").ToList();

                    int count = 0;
                    var machinesData = new List<AvailableMachines>();

                    foreach (var neededMachine in neededMachinesList)
                    {
                        foreach (var machine in branchMachines)
                        {
                            var machineExist = branchMachines.FirstOrDefault(m => m.LoadCapacityId == neededMachine.LoadCapacityId && m.Id == machine.Id);
                            if (machineExist != null)
                            {
                                var loadCapacity = allLoadCapacities.SingleOrDefault(lc => lc.Id == neededMachine.LoadCapacityId);
                                var program = allPrograms.SingleOrDefault(p => p.Id == neededMachine.ProgramId);

                                if (loadCapacity != null && program != null)
                                {

                                    var timeNeededForOneMachine = request.BookingDate.AddMinutes(program.Duration * request.LaundryIntervals);
                                    var bookingTime = TimeOnly.FromDateTime(request.BookingDate);
                                    var date = new DateTime(request.BookingDate.Year, request.BookingDate.Month, request.BookingDate.Day,
                                                            bookingTime.Hour, bookingTime.Minute, bookingTime.Second);
                                    var endTime = TimeOnly.FromDateTime(timeNeededForOneMachine);
                                    DateTime endingDate = new DateTime(date.Year, date.Month, date.Day,
                                                                       endTime.Hour, endTime.Minute, endTime.Second);

                                    var isMachineAvailable = await IsMachineAvailableAsync(machineExist.Id, bookingTime, endTime);

                                    if (isMachineAvailable)
                                    {
                                        var machineData = new AvailableMachines
                                        {
                                            MachineId = machineExist.Id,
                                            ProgramId = program.Id,
                                            BookingDate = date,
                                            EndDate = endingDate,
                                            BranchId = branch.Id
                                        };

                                        machinesData.Add(machineData);
                                        count++;
                                        branchTotalPrice += loadCapacity.Price * request.LaundryIntervals + program.Price * request.LaundryIntervals;

                                        if (count == neededMachinesList.Count)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (count == neededMachinesList.Count)
                        {
                            allRequestedMachines.AddRange(machinesData);
                            var branchDistance = _methods.GetDistance(request.Latitude, request.Longitude, branch.Latitude, branch.Longitude);
                            if (branchDistance <= request.Distance)
                            {
                                allBranches.Add(new BranchesData
                                {
                                    BranchName = branch.Name,
                                    BranchId = branch.Id,
                                    Distance = branchDistance,
                                    isRecomended = getAlreadyBookedBranch != null && branch.Id == getAlreadyBookedBranch.BranchId,
                                    Stars = GetAverageRatingByBranchIdAsync(branch.Id),
                                });

                                // Set total price only once
                                if (!isPriceSet)
                                {
                                    response.Price = branchTotalPrice;
                                    response.DiscountedPrice = response.Price - dPrice;
                                    isPriceSet = true;
                                }
                            }
                        }
                    }
                }

                response.BranchesList = allBranches.OrderBy(r => r.Distance).DistinctBy(b => b.BranchId).ToList();
                response.MachinesList = allRequestedMachines.DistinctBy(p => p.MachineId).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
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
                        var distance = await _methods.GetDrivingDistanceAsync(latitude, longitude, b.Latitude, b.Longitude);
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
                    return Ok(response.OrderBy(o => o.Distance));
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
                            Latitude = requestDto.Latitude,
                            Longitude = requestDto.Longitude,
                        };
                        await _dbContext.BulkClothes.AddAsync(request);
                        await _dbContext.SaveChangesAsync();
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
                    return BadRequest(new { ErrorMsg = "Id Can't be null and empty." });

                };
                var bulk = await _dbContext.BulkClothes.SingleOrDefaultAsync(b => b.Id == id);
                if (bulk == null)
                {
                    return NotFound(new { ErrorMsg = "Bulk Cloth request does not exist." });
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


        [HttpGet("bulkCloth")]
        public async Task<IActionResult> GetBulkRequest()
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
                    var bulkRequests = _dbContext.BulkClothes.Where(b => b.UserId == user.Id).ToList();

                    return Ok(bulkRequests);
                }

            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }
        [HttpGet("get-bookings")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { Error = "Email claim not found" });
                }

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }

                var bookings = await _dbContext.Bookings
                    .Where(b => b.UserId == user.Id)
                    .Select(b => new
                    {
                        b.Id,
                        b.BookingDate,
                        b.Price,
                        b.Status,
                        b.BranchId,
                        BranchName = b.Branch!.Name,
                        MachineId = b.BookingDetails!.Select(d => d.MachineId).FirstOrDefault(),

                    })
                    .OrderBy(b => b.BookingDate)
                    .ToListAsync();

                var machineIds = bookings
                    .Where(b => b.MachineId.HasValue)
                    .Select(b => b.MachineId!.Value)
                    .Distinct()
                    .ToList();

                var machines = await _dbContext.Machines
                                               .Where(m => machineIds.Contains(m.Id))
                                               .ToDictionaryAsync(m => m.Id, m => m.Type);

                var userBookingDtos = bookings.Where(b=>b.Status != "In-Progress").Select(b => new
                {
                    id = b.Id,
                    bookingDate = b.BookingDate,
                    price = b.Price,
                    status = b.Status,
                    branchId = b.BranchId,
                    branchName = b.BranchName,
                    machineType = b.MachineId.HasValue && machines.ContainsKey(b.MachineId.Value)
                                  ? machines[b.MachineId.Value]
                                  : "Machine Removed",
                    ratingDone = isRatingDone(b.Id),
                }).ToList();

                return Ok(userBookingDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal Server Error", Details = ex.Message });
            }
        }

        bool isRatingDone(int bookingId)
        {
            return _dbContext.Reviews.FirstOrDefault(r => r.BookingId == bookingId) != null ? true : false;
        }

        [HttpDelete("remove-booking")]
        public async Task<IActionResult> RemoveBooking(int bookingId)
        {
            try
            {
                var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking != null)
                {
                    var purchProducts = _dbContext.PurchasedProducts.Where(b => b.BookingId == booking.Id);
                    var bookdetails = _dbContext.BookingDetails.Where(b => b.BookingId == booking.Id);
                    _dbContext.RemoveRange(purchProducts);
                    _dbContext.RemoveRange(bookdetails);
                    _dbContext.Remove(booking);
                    await _dbContext.SaveChangesAsync();
                    return NoContent();
                }
                else
                {
                    return BadRequest(new { ErrorMsg = "Booking not found" });
                }
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = $"Internal Server Error" });
            }
        }

         int GetAverageRatingByBranchIdAsync(int branchId)
        {
            var reviewCount =  _dbContext.Reviews
                .Where(r => r.BranchId == branchId)
                .Count();

            if (reviewCount == 0)
            {
                return 0;
            }

            var totalStars =  _dbContext.Reviews
                .Where(r => r.BranchId == branchId)
                .Sum(r => r.Stars);

            double averageStars = (double)totalStars / reviewCount;
            int flooredAverageStars = (int)Math.Floor(averageStars);
            return flooredAverageStars;
        }


        [HttpGet("booking-status-count")]
        public async Task<IActionResult> GetBookingStatusCount()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { Error = "Email claim not found" });
                }

                var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Email == email);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found" });
                }

                var bookingStatusCounts = await _dbContext.Bookings
                    .Where(b => b.UserId == user.Id)
                    .GroupBy(b => b.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var completedCount = bookingStatusCounts.FirstOrDefault(x => x.Status == "Completed")?.Count ?? 0;
                var scheduledCount = bookingStatusCounts.FirstOrDefault(x => x.Status == "Scheduled")?.Count ?? 0;
                var inProgressCount = bookingStatusCounts.FirstOrDefault(x => x.Status == "In-Progress")?.Count ?? 0;
                var cancelledCount = bookingStatusCounts.FirstOrDefault(x => x.Status == "Cancelled")?.Count ?? 0;

                return Ok(new
                {
                    Completed = completedCount,
                    Scheduled = scheduledCount,
                    InProgress = inProgressCount,
                    Cancelled = cancelledCount
                });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview(ReviewDto request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { Error = "Email claim not found" });
                }

                var userWithBooking = await _dbContext.Users
                    .Where(u => u.Email == email)
                    .Select(u => new
                    {
                        User = u,
                        Booking = _dbContext.Bookings.FirstOrDefault(b => b.Id == request.BookingId)
                    })
                    .FirstOrDefaultAsync();

                if (userWithBooking == null || userWithBooking.User == null)
                {
                    return NotFound(new { Error = "User not found" });
                }

                if (userWithBooking.Booking == null)
                {
                    return NotFound(new { Error = "Booking does not exist" });
                }

                var review = new Review
                {
                    UserId = userWithBooking.User.Id,
                    BookingId = request.BookingId,
                    UserThoughts = request.UserThoughts,
                    Stars = request.Stars,
                    BranchId = userWithBooking.Booking.BranchId
                };

                await _dbContext.Reviews.AddAsync(review);
                await _dbContext.SaveChangesAsync();

                return StatusCode(201, new { SuccessMsg = "Review done successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error", DetailedErrorMsg = ex.ToString() });
            }
        }


    }


}
