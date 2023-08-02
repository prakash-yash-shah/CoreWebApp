using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CoreWebApp.ContextClass;
using CoreWebApp.Models;
using JsonWebTokens.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CoreWebApp.Controllers
{
    //[Authorize]
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
            httpClient.BaseAddress = new Uri("http://localhost:5085/");

            _dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Redify-WP.postman_collection_19thJuly2023.json");
            var kvUrl = configuration["AzureKeyVaultUrl:url"];
            var keyVaultName = configuration["AzureKeyVaultUrl:KeyvaultName"];
            //var keyVaultUri = new Uri("https://prakash-yash-shah.vault.azure.net/");
            //_secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
            var secretsClient = new SecretClient(new Uri(kvUrl), new DefaultAzureCredential());
            var sqlConnectionString = secretsClient.GetSecret("ConnectionStrings--DefaultConnection");
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public  IActionResult PublicEndpointAsync()
        {
            var token = Request.Headers["Authorization"].ToString();

            // Read the JSON data from the file
            string jsonData = System.IO.File.ReadAllText(_dataFilePath);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
            string item = jsonObject.item[0].item[0].name.ToString();

            if (item != null)
            {
                return Ok(item);
            }

            return NotFound("Single License record not found");
        }

        [HttpGet("secure")]
        [Authorize]
        public IActionResult SecureEndpoint()
        {
            return Ok("This is a secure endpoint. Requires authentication.");
        }

        [HttpGet("{secretName}")]
        [Authorize]
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
