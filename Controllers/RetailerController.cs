/*using FYP.API.Data;
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
    [Authorize(Policy = "RetailerDto")]

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

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email && x.UserType == "RetailerDto");

                if (user == null)
                {
                    return NotFound("RetailerDto not found.");
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == user.BranchId);

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
                    Capacity = machine.LoadCapacity,
                    Status = machine.Status,
                }).ToList();

                return Ok(machinesDtoList);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("machines")]
        public async Task<IActionResult> AddMachine([FromBody] MachineDto request)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("User email not found.");
                }

                // Active , Busy , In-Maintenance

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email && x.Role == "RetailerDto");

                if (user == null)
                {
                    return NotFound("RetailerDto not found.");
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == user.BranchId);

                if (branch == null)
                {
                    return NotFound("Branch not found for the operator.");
                }

                var machine = new LaundryMachine
                {
                    Name = request.Name,
                    Capacity = request.Capacity,
                    Status = request.Status,
                    UserId = user.Id,
                    BranchId = branch.Id,
                };

                await _dbContext.Machines.AddAsync(machine);
                await _dbContext.SaveChangesAsync();

                return Ok($"MachineIds added successfully with Name: {machine.Name}");
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
                    return NotFound(new { Error = "MachineIds not found." });
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

                machine.Name = request.Name;
                machine.Capacity = request.Capacity;
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
                    Name = machine.Name,
                    Capacity = machine.Capacity,
                    Status = machine.Status
                };

                return Ok(machineDto);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("User email not found.");
                }

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email && x.Role == "RetailerDto");

                if (user == null)
                {
                    return NotFound("RetailerDto not found.");
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == user.BranchId);

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
                    Qauntity = product.Quantity,
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
        [HttpGet("products/{id}")]
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
                    Description = product.Description,
                    Qauntity = product.Quantity,
                    Price = product.Price,
                    ProductImageUrl = product.ImageUrl
                };

                return Ok(pr);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("products")]
        public async Task<IActionResult> AddProduct()
        {
            try
            {
                var file = Request.Form.Files["image"];
                var name = Request.Form["name"];
                var description = Request.Form["description"];
                var price = Request.Form["price"];
                var quantity = Request.Form["quantity"];

                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("User email not found.");
                }

                var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email && x.Role == "RetailerDto");

                if (user == null)
                {
                    return NotFound("RetailerDto not found.");
                }

                var branch = await _dbContext.Branches.SingleOrDefaultAsync(b => b.Id == user.BranchId);

                var products = await _dbContext.Products.ToListAsync();

                if (branch == null)
                {
                    return NotFound("Branch not found.");
                }


                var random = new Random();
                IFormFile image = file!;
                string fileName = name + random.Next(1, 9999) + "_" + products.Count + 1 + "." + image.FileName.Split('.')[1];
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Products");
                string imagePath = Path.Combine(directoryPath, fileName);

                Directory.CreateDirectory(directoryPath);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                var product = new LaundryItem()
                {
                    Name = name!,
                    Description = description!,
                    Quantity = int.Parse(quantity.ToString()),
                    Price = int.Parse(price.ToString()),
                    ImageUrl = imagePath,
                    BranchId = branch.Id,
                    UserId = user.Id,
                };
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = $"ProductsData Uploaded Successfully with Name : {product.Name}" });

            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto request)
        {
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound("ProductsData not found.");
                }
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.Quantity = request.Qauntity;

                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = $"ProductsData Updated Successfully with Name : {product.Name}" });
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("products/{id}")]
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

                return Ok($"ProductsData Delete with Id : {product.Id}");
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
        }
    }
}
*/