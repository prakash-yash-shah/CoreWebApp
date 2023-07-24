using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CoreWebApp.ContextClass;
using CoreWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace CoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly BrandContext _brandContext;
        private readonly string _dataFilePath;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SecretClient _secretClient;
        private readonly IConfiguration _configuration;

        public BrandController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Redify-WP.postman_collection_19thJuly2023.json");
            var kvUrl = configuration["AzureKeyVaultUrl:url"];
            var keyVaultName = configuration["AzureKeyVaultUrl:KeyvaultName"];
            var keyVaultUri = new Uri("https://prakash-yash-shah.vault.azure.net/");
            _secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
            var secretsClient = new SecretClient(new Uri(kvUrl), new DefaultAzureCredential());
            var sqlConnectionString = secretsClient.GetSecret("ConnectionStrings--DefaultConnection");

            _configuration = configuration;
        }
        //public BrandController(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}

        [HttpGet]
        [Route("GetBrands")]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            if (_brandContext == null)
            {
                return NotFound();
            }
            return await _brandContext.brands.ToListAsync();
        }

        [HttpGet]
        [Route("GetBrand")]
        public async Task<ActionResult<Brand>> GetBrand(int Id)
        {
            if (_brandContext == null)
            {
                return NotFound();
            }
            var data = await _brandContext.brands.FindAsync(Id);
            if (data == null)
            {
                return NotFound();
            }
            return data;
        }


        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> PublicEndpointAsync()
        {
            // Read the JSON data from the file
            string jsonData = System.IO.File.ReadAllText(_dataFilePath);

            dynamic jsonObject =  JsonConvert.DeserializeObject<dynamic>(jsonData);
            string item =  jsonObject.item[0].item[0].name.ToString();

            if (item != null)
            {
                return Ok(item);
            }

            return NotFound("Single License record not found");
        }


        [HttpGet("authenticate")]
        public IActionResult Authenticate(string userId, string password)
        {
            if (userId == "admin" && password == "password123")
            {
                return Ok("Authentication successful");
            }
            else
            {
                return BadRequest("Invalid credentials");
            }
        }


        [HttpGet("secure")]
        public IActionResult SecureEndpoint()
        {
            return Ok("This is a secure endpoint. Requires authentication.");
        }

        [HttpGet("{secretName}")]
        public async Task<IActionResult> GetSecret(string companyname, string secretName)
        {
            //companyname = prakash-yash-shah
            //keyVaultName = ConnectionStrings--DefaultConnection
            try
            {
                Uri keyVaultUri = new Uri($"https://{companyname}.vault.azure.net/");
                var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());

                KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);
                return Ok(secret.Value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
