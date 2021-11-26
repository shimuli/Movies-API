using AuthenticationPlugin;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using moviesApi.Data;
using moviesApi.Dto;
using moviesApi.Models;
using moviesApi.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace moviesApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private MoviesDbContext _moviesDbContext;
        string _baseUrl;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        System.Random random = new System.Random();

        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(MoviesDbContext moviesDbContext, IHttpContextAccessor context, IConfiguration configuration, IMapper mapper, ILogger<UsersController> logger)
        {
            _moviesDbContext = moviesDbContext;

            // for url
            var request = context.HttpContext.Request;
           _baseUrl = $"{request.Scheme}://{request.Host}";

            // for jwt
            _configuration = configuration;
            _auth = new AuthService(_configuration);

            // mapper 
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegitserDto registerDto)
        {
            // image path
            string userimageUrl = null;

            //string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(registerDto.Name + " registered an account");

            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".png");
           
            if (registerDto.UserImage != null)
            {
                var filestream = new FileStream(filePath, FileMode.Create);
                registerDto.UserImage.CopyTo(filestream);
                userimageUrl = _baseUrl+ filePath.Remove(0, 7);
            }
            else if(registerDto.UserImage == null)
            {
                 userimageUrl = null;
            }

            var userWithSameEmail =_moviesDbContext.Users.Where(u => u.Email == registerDto.Email).SingleOrDefault();
            var userWIthSamePhone = _moviesDbContext.Users.Where(u => u.Phone == registerDto.Phone).SingleOrDefault();
            if (userWithSameEmail != null)
            {
                return BadRequest(new { message = "User with the same email already exists" });

            }
            if (userWIthSamePhone != null)
            {
               // return BadRequest("User with the same phone number already exists");
                return BadRequest(new { message = "User with the same phone number already exists" });
            }
            else
            {
                var emailToken = Guid.NewGuid().ToString();
                string randomNum = Convert.ToString(random.Next(1000, 9999));
               if(registerDto.Role == null)
                {
                    registerDto.Role = "User";
                }
               _mapper.Map<User>(registerDto);

                var userObj = new User
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Password = SecurePasswordHasherHelper.Hash(registerDto.Password),
                    Phone = registerDto.Phone,
                    Role = registerDto.Role,
                    ImageUrl = userimageUrl,
                    Verificationtoken = emailToken,
                    confirmCode = randomNum

                };

                await _moviesDbContext.Users.AddAsync(userObj);
                await _moviesDbContext.SaveChangesAsync();

                // send sms
                string message = "Hello " + registerDto.Name +", use this code to verify your number: "+ randomNum;

                //Communication.sendSMs(registerDto.Phone, message);


                return new ObjectResult(new
                {
                    message = "Account  was created successful",
                    Id = userObj.Id,
                    user_name = userObj.Name,
                    user_email = userObj.Email,
                    verifuPhoneCode = randomNum
                });

            }

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            _logger.LogInformation(loginDto.Phone + " logged in");
            var userPhone = await _moviesDbContext.Users.FirstOrDefaultAsync(u => u.Phone == loginDto.Phone);
            if(userPhone == null)
            {
                return NotFound();
            }

            if(!SecurePasswordHasherHelper.Verify(loginDto.Password, userPhone.Password))
            {
                return Unauthorized();
            }

            _mapper.Map<User>(loginDto);
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.NameId,userPhone.Email),
               new Claim(JwtRegisteredClaimNames.Email,userPhone.Email ),
               new Claim(ClaimTypes.MobilePhone, loginDto.Phone),
               new Claim(ClaimTypes.Role, userPhone.Role),
               new Claim(ClaimTypes.Name, userPhone.Name),
             };
            var token = _auth.GenerateAccessToken(claims);

            return new ObjectResult(new
            {
                message= "Login was successful",
                user_id = userPhone.Id,
                user_name = userPhone.Name,
                user_phone = userPhone.Phone,
                user_email = userPhone.Email,
                isPhoneVerified = userPhone.PhoneVerified,
                isEmailVerified = userPhone.EmailVerified,
                access_token = token.AccessToken,
                token_type = token.TokenType,
                expires_in = token.ExpiresIn,
               

                /* expires_in = token.ExpiresIn,
                 token_type = token.TokenType,
                 creation_Time = token.ValidFrom,
                 expiration_Time = token.ValidTo,*/
            });
        }



        [HttpPost("{id}")]
        public async Task<IActionResult> VerfiyPhone(int id, [FromForm] VerifyPhoneDto verifyDto)
        {
            _logger.LogInformation(verifyDto.Phone + " verified account");
            var getUser = await _moviesDbContext.Users.FindAsync(id);

            if(getUser == null)
            {
                return NotFound();
            }

            var userPhone = await _moviesDbContext.Users.FirstOrDefaultAsync(u => u.Phone == verifyDto.Phone);
            if (userPhone == null)
            {
                return NotFound();
            }
            if (userPhone.confirmCode == null)
            {
                //return StatusCode(StatusCodes.Status406NotAcceptable);
                return new ObjectResult( new 
                {
                    message = "This number was verified"
                });

            }

            if (verifyDto.confirmCode != userPhone.confirmCode)
            {
                return Unauthorized();
            }

            

            _mapper.Map<User>(verifyDto);

            getUser.PhoneVerified = true;
            getUser.confirmCode = null;
            await _moviesDbContext.SaveChangesAsync();

            return new ObjectResult(new
            {
                message = "Phone number verified successfully",
                verifiedPhone = userPhone.PhoneVerified,
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(int? pageNumber, int? pageSize)
        {

            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Get all users ");

            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 7;

            var users = await (from user in _moviesDbContext.Users
                                 select new
                                 {
                                     Id = user.Id,
                                     UserName = user.Name,                                    
                                     Image = user.ImageUrl
                                 }).ToListAsync();

            //var userDto = _mapper.Map<UserDto>(users);
            return Ok(users.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));


        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> userDetails(int userId)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Get user details with Id "+ userId);
            //var user = await _moviesDbContext.Users.Where(a => a.Id == userId).Include(a => a.Songs).ToListAsync();
            var user = await _moviesDbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userDto =  _mapper.Map<UserDto>(user);
            return Ok(userDto);
           
        }


        [HttpGet]
        public async Task<IActionResult> resendCode(string phonenumber)
        {

            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Code resent to " + phonenumber);
            //var user = await _moviesDbContext.Users.Where(a => a.Id == userId).Include(a => a.Songs).ToListAsync();

            var userPhone = await _moviesDbContext.Users.FirstOrDefaultAsync(u => u.Phone == phonenumber);
            if(userPhone == null)
            {
                return NotFound();
            }
            if(userPhone.PhoneVerified == true)
            {
                var notVerified = "This user was verified";
                //return NotFound(new JsonResult(notVerified) { StatusCode = 403 });
                return BadRequest(notVerified);

            }
            string code = userPhone.confirmCode; 
            string message = "Hello " + userPhone.Name + ", use this code to verify your account: " + code;

            //Communication.sendSMs(userPhone.Phone, message);
            return new ObjectResult(new
            {
                message = "Code was sent succesfully",
                userId = userPhone.Id,
                code = userPhone.confirmCode
            });

        }

    }
}
