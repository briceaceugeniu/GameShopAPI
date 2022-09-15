using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameShopAPI.Data;
using GameShopAPI.Models;
using GameShopAPI.DTO;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text.Json;
using Newtonsoft.Json;
using GameShopAPI.Helper;
using JsonResult = GameShopAPI.Helper.JsonResult;

namespace GameShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        private const int CUSTOMER_ADMIN_ROLE_ID = 1;
        private const int CUSTOMER_REGULAR_USER_ROLE_ID = 2;
        private const int CUSTOMER_MODERATOR_ROLE_ID = 3;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            if (_context.Customer == null)
            {
              return NotFound();
            }
            return await _context.Customer.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
          if (_context.Customer == null)
          {
              return NotFound();
          }
            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        
        private async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (_context.Customer == null)
            {
                return Problem("Entity set 'AppDbContext.Customer' is null.");
            }
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            return Created("", "success");
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<Customer>> RegisterCustomer(CustomerRegistrationDto customer)
        {
            if (String.IsNullOrEmpty(customer.Name.Trim()) || String.IsNullOrEmpty(customer.Email.Trim())
                || !IsValidEmail(customer.Email) || String.IsNullOrEmpty(customer.Password.Trim()))
            {
                return Problem("Parameter fail", "", 401);
            }

            var checkEmail = await _context.Customer.FirstOrDefaultAsync(c => c.Email == customer.Email);

            if (checkEmail != null)
            {
                return Problem("Email already exist", "", 401);
            }

            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: customer.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));


            Customer newCustomer = new()
            {
                Name = customer.Name,
                Email = customer.Email,
                Passkey = hashed,
                RoleId = CUSTOMER_REGULAR_USER_ROLE_ID,
                Salt = Convert.ToBase64String(salt),
                CreationDate = DateTime.Now
            };

            return await PostCustomer(newCustomer);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(CustomerRegistrationDto customer)
        {
            if (String.IsNullOrEmpty(customer.Email.Trim()) || String.IsNullOrEmpty(customer.Password.Trim()))
            {
                return Problem("Parameter fail", "", 401);
            }

            var customerExist = await _context.Customer.FirstOrDefaultAsync(c => c.Email == customer.Email);
            if (customerExist == null)
            {
                return Problem("Name or password wrong", "", 401);
            }

            var salt = Convert.FromBase64String(customerExist.Salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: customer.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

            if (hashed != customerExist.Passkey)
            {
                return StatusCode(401, new JsonResult { status = "error", message = "Name or password wrong" });
            }

            byte[] token_byte = new byte[256];
            using (var rngCsp = RandomNumberGenerator.Create())
            {
                rngCsp.GetNonZeroBytes(token_byte);
            }

            var token = Convert.ToBase64String(token_byte);

            // save session token
            customerExist.SessionToken = token;
            customerExist.SessionExpire = DateTime.Today.AddMonths(+3);

            _context.Customer.Update(customerExist);
            var entry = await _context.SaveChangesAsync();

            if (entry != 1)
            {
                return Problem("Could not update Customer session", "", 500);
            }

            // return / set sookie session token
            string json_result = JsonConvert.SerializeObject(new JsonResult { status = "success", auth_token = token, is_admin = customerExist.RoleId == CUSTOMER_ADMIN_ROLE_ID });

            return Ok(json_result);
        }


        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customer == null)
            {
                return NotFound();
            }
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customer?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
