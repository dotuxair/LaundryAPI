using FYP.API.Data;
using FYP.API.Models.Domain;
using FYP.API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Security.Claims;

namespace FYP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "BranchManager")]

    public class RetailerController : ControllerBase
    {
        private readonly LaundaryDbContext _dbContext;
        public RetailerController(LaundaryDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("machines")]
        public async Task<IActionResult> GetAllMachines()
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
                    return NotFound("User does not exist.");
                }

                var retailer = await _dbContext.BranchManagers.SingleOrDefaultAsync(r => r.UserId == user.Id);
                if (retailer == null)
                {
                    return NotFound("Retailer not found.");
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == retailer.BranchId);

                if (branch == null)
                {
                    return NotFound("Branch not found.");
                }

                var machines = await _dbContext.Machines
                    .Where(m => m.BranchId == branch.Id)
                    .ToListAsync();

                var machinesDtoList = machines.Select(machine => new GetMachinesDto
                {
                    Id = machine.Id,
                    MachineCode = machine.MachineCode,
                    Status = machine.Status,
                    MachineType =machine.Type,
                    Price = getPrice(machine.LoadCapacityId),
                }).ToList();

                return Ok(machinesDtoList);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        double getPrice(int id)
        {
            var load = _dbContext.LoadCapacity.SingleOrDefault(l => l.Id == id);
            return load!.Price;
        }


        [HttpPost("machines")]
        public async Task<IActionResult> AddMachine([FromBody] MachineDto request)
        {
            try
            {
                
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { ErrorMsg = "Token Expired, Login Again." });
                }
                var machineCodeExist = await _dbContext.Machines.SingleOrDefaultAsync(m => m.MachineCode == request.MachineCode);
                if (machineCodeExist != null)
                {
                    return BadRequest(new { ErrorMsg = "MachineCode Already Exist." });
                }

                // Active , Busy , In-Maintenance

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return NotFound(new { ErrorMsg = "User does not exist." });
                }
                var retailer = await _dbContext.BranchManagers.SingleOrDefaultAsync(r => r.UserId == user.Id);
                if (retailer == null)
                {
                    return NotFound(new { ErrorMsg = "Retailer does not exist." });
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == retailer.BranchId);

                if (branch == null)
                {
                    return NotFound(new { ErrorMsg = "Branch not found for the Retailer." });
                }


                var machine = new Machine
                {
                    MachineCode = request.MachineCode,
                    Status = "Ready",
                    Type = request.MachineType,
                    BranchId = branch.Id,
                    LoadCapacityId = request.loadCapacity,
                };

                await _dbContext.Machines.AddAsync(machine);
                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = $"Machine Added in {branch.Name}  Successfully." });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("machines/{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            try
            {
                var machine = await _dbContext.Machines.SingleOrDefaultAsync(m => m.Id == id);

                if (machine == null)
                {
                    return NotFound(new { Error = "Machine not found." });
                }

                _dbContext.Remove(machine);
                await _dbContext.SaveChangesAsync();

                return Ok($"MachineIds deleted successfully with Id: {machine.Id}");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut("machines/{id}")]
        public async Task<IActionResult> UpdateMachine(int id, [FromBody] MachineDto request)
        {
            try
            {
                var machine = await _dbContext.Machines.SingleOrDefaultAsync(m => m.Id == id);

                if (machine == null)
                {
                    return NotFound(new { Error = "MachineIds not found." });
                }

                machine.Status = request.Status;

                _dbContext.Update(machine);
                await _dbContext.SaveChangesAsync();

                return Ok($"MachineIds updated successfully with Id: {machine.Id}");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("machines/{id}")]
        public async Task<IActionResult> GetMachineById(int id)
        {
            try
            {
                var machine = await _dbContext.Machines.SingleOrDefaultAsync(m => m.Id == id);

                if (machine == null)
                {
                    return NotFound(new { Error = "MachineIds not found." });
                }

                var machineDto = new MachineDto
                {
                    Id = machine.Id,
                    Status = machine.Status
                };

                return Ok(machineDto);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetAllProducts()
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
                var retailer = await _dbContext.BranchManagers.SingleOrDefaultAsync(r => r.UserId == user.Id);
                if (retailer == null)
                {
                    return NotFound("Retailer not found.");
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == retailer.BranchId);

                if (branch == null)
                {
                    return NotFound("Branch not found.");
                }

                var products = await _dbContext.Products
                    .Where(m => m.BranchId == branch.Id)
                    .ToListAsync();

                var allproducts = products.Select(product => new GetProductsDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    ProductImageUrl = product.ImageUrl,
                }).ToList();


                return Ok(allproducts);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {

                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return NotFound(new { Error = "ProductsData does not exist" });
                }
                var pr = new GetProductsDto()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    Description = product.Description,
                    ProductImageUrl = product.ImageUrl
                };
                return Ok(pr);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddProduct()
        {
            try
            {
                var file = Request.Form.Files["image"];
                var name = Request.Form["name"];
                var description = Request.Form["description"];
                var price = Request.Form["price"];
                var quantity = Request.Form["quantity"];
                //var type = Request.Form["type"];


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
                var retailer = await _dbContext.BranchManagers.SingleOrDefaultAsync(r => r.UserId == user.Id);
                if (retailer == null)
                {
                    return NotFound("Retailer Not Found");
                }

                var imagePath = await UploadImageAsync(file!);

                var item = new Product()
                {
                    Name = name!,
                    Description = description!,
                    Quantity = int.Parse(quantity.ToString()),
                    Price = int.Parse(price.ToString()),
                    ImageUrl = imagePath,
                    ProductType ="Washer",
                    BranchId = retailer.BranchId,
                };

                await _dbContext.Products.AddAsync(item);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = $"ProductsData Uploaded Successfully with Name : {item.Name}" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<string> UploadImageAsync(IFormFile file)
        {
            var guid = Guid.NewGuid().ToString(); // Generate GUID
            var extension = Path.GetExtension(file.FileName);
            string fileName = guid + extension; // Append extension
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Products");
            string imagePath = Path.Combine(directoryPath, fileName);

            Directory.CreateDirectory(directoryPath);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return guid + extension; // Return only the GUID without extension
        }



        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto request)
        {
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound("Item not found.");
                }
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Quantity = request.Quantity;

                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = $"Item Data Updated Successfully with Name : {product.Name}" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {

                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return NotFound(new { Error = "ProductsData does not exist" });
                }
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();

                return Ok($"ProductsData Delete with Id : {product.Id}");
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /* [HttpGet("bookings")]
         public async Task<IActionResult> GetBookings()
         {
             try
             {
                 var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                 if (string.IsNullOrEmpty(email))
                 {
                     return BadRequest("User email not found.");
                 }

                 var user = await _dbContext.Users
                     .Include(u => u.Branch)
                     .SingleOrDefaultAsync(x => x.Email == email && x.Role == "RetailerDto");

                 if (user == null)
                 {
                     return NotFound("RetailerDto not found.");
                 }
                 var booking = await _dbContext.Bookings.Where(b => b.BranchId == user.BranchId).Select(b => new BookingDto
                 {
                     StartTime = b.StartTime,
                     EndTime = b.EndTime,
                     Date = b.Date,
                     Price = b.Price,
                     Status = b.Status
                 }).ToListAsync();

                 return Ok(booking);
             }
             catch
             {
                 return StatusCode(500, "Internal Server Error");
             }
         }*/

        [HttpGet("getBulkRequests")]
        public async Task<IActionResult> GetBulkRequests()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { ErrorMsg = "Token Expired or Invalid. Please login again." });

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);
                if (user == null)
                    return NotFound(new { ErrorMsg = "User not found." });

                var retailer = await _dbContext.BranchManagers.FirstOrDefaultAsync(r => r.UserId == user.Id);
                if (retailer == null)
                    return NotFound(new { ErrorMsg = "Retailer not found." });

                var requests = await _dbContext.BulkClothes.Where(b => b.BranchId == retailer.BranchId).ToListAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error: " + ex.Message });
            }
        }


        [HttpGet("getBulkRequests/{id}")]
        public async Task<IActionResult> GetBulkRequest(int id)
        {
            try
            {
                var requests = await _dbContext.BulkClothes.Where(b => b.Id == id).ToListAsync();
                return Ok(requests);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error: " + ex.Message });
            }
        }


        [HttpPost("getAllBulkRequests")]
        public async Task<IActionResult> UpdatePrice()
        {
            try
            {

                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { ErrorMsg = "Token Expired or Invalid. Please login again." });

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);
                if (user == null)
                    return NotFound(new { ErrorMsg = "User not found." });



                var bulk = await _dbContext.BulkClothes.Where(b => b.UserId == user.Id).ToListAsync();
                if (bulk == null)
                {
                    return BadRequest(new { SuccessMsg = "Bulk Cloth not found." });

                }

                return Ok(bulk);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error: " + ex.Message });
            }
        }


        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { ErrorMsg = "Invalid ID." });

                var bulk = await _dbContext.BulkClothes.FirstOrDefaultAsync(b => b.Id == id);
                if (bulk == null)
                    return NotFound(new { ErrorMsg = "Bulk Cloth request not found." });

                bulk.AcceptedPrice = bulk.PriceOffered;
                bulk.DateAccepted = DateTime.Now;
                bulk.Status = "Accepted";
                bulk.PaymentStatus = "Not Paid";
                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = "Bulk Cloth Accepted Successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error: " + ex.Message });
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
                    return Ok(new { SuccessMsg = "Bulk Cloth Reqyested Accepted Successfully." });
                }
            }

            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error," });

            }
        }
        [HttpPut("bulkCloth/{id}")]
        public async Task<IActionResult> UpdateBulkClothStatus(int id, string status)
        {
            try
            {
                if (id <= 0 || string.IsNullOrEmpty(status))
                    return BadRequest(new { ErrorMsg = "Invalid ID or Status." });

                var bulk = await _dbContext.BulkClothes.FirstOrDefaultAsync(b => b.Id == id);
                if (bulk == null)
                    return NotFound(new { ErrorMsg = "Bulk Cloth request not found." });

                bulk.Status = status;
                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = $"Bulk Cloth Status updated to '{status}' successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error: " + ex.Message });
            }
        }
        [HttpPut("completed/{id}")]
        public async Task<IActionResult> CompleteBulkCloth(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { ErrorMsg = "Invalid ID." });

                var bulk = await _dbContext.BulkClothes.FirstOrDefaultAsync(b => b.Id == id);
                if (bulk == null)
                    return NotFound(new { ErrorMsg = "Bulk Cloth request not found." });

                bulk.Status = "Completed";
                bulk.DateCompleted = DateTime.Now.Date;
                await _dbContext.SaveChangesAsync();

                return Ok(new { SuccessMsg = "Bulk Cloth Status updated to 'Completed' successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error: " + ex.Message });
            }
        }

        [HttpGet("load-capacity/{type}")]
        public async Task<IActionResult> GetAllLoadCapacities(string type)
        {
            try
            {
                var loadCapacities = await _dbContext.LoadCapacity.Where(l => l.Type == type).ToListAsync();
                return Ok(loadCapacities);
            }
            catch
            {
                return StatusCode(500, new { ErrorMsg = "Internal Server Error" });
            }
        }


    }
}
