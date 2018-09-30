using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
 using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
// using System.Collections.Generic;
// using System.Linq;

// using DatingApp.API.Data;
// using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Controllers
{
   
    [Route("api/[controller]")]
    //[ApiController]
    /* ApiController simplified a lot of stuff such as ->
        if we remove API controller we no longer get:
            -  string values such username and password filled-in as string empty but as null,
             and operation on string crashes, we could solve this PB by adding:
                Register ([FromBody]UserForRegisterDto userForRegisterDto) 
            - but in doing so we need also to call ModelState.valid() to be able to check the annotation 
            (eg: required, stringlength) and send a bad request our self: e.g if(!ModelState.IsValid) return BadRequest(ModelState);
    */
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository, IConfiguration config, IMapper mapper)
        {
            _repo = authRepository;
            _config = config;
            _mapper = mapper;
        }

        //we don't need to use [FromBody], because dotnet inferer auto from the post request body 
        //used by userForRegisterDto
        // public async Task<IActionResult> Register ([FromBody]UserForRegisterDto userForRegisterDto){
        [HttpPost("register")]        
        public async Task<IActionResult> Register ([FromBody]UserForRegisterDto userForRegisterDto){
            
            //validate request if [ApiController] not specified 
            if(!ModelState.IsValid) return BadRequest(ModelState);

            userForRegisterDto.username = userForRegisterDto.username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.username)) return BadRequest("Username already exists");
            
            var userToCreate = new User {Username = userForRegisterDto.username};

            userToCreate=_mapper.Map<User>(userForRegisterDto);
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.password);
            
            var userToReturn = _mapper.Map<UserForDetailedDto >(createdUser);
            
            //we need to a send location header (url to the newly created user) as well
            // as the resource we created (newly created user)

            //since the 'GetUser' method is in UsersController (another controller) we need to provide that value 
            return CreatedAtRoute("GetUser", new { Controller="Users", id = userToCreate.Id }, userToReturn);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userForLoginDto)
        {

            //throw new Exception("computer says NO!");
            var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);

            //we don't want to give the user a hint if he exist or not!
            if (userFromRepo == null) return Unauthorized();


            //Start Token building
            //-1 : create a claims so that the server doesn't go to the DB to check for credentials: Id & username      
            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username)
            };


            /*
                Token server signing!: to make sure that when the token is comming back is valid!
            */
            //2-Key for the token which will be hashed!, encoded to a bytearray
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //3- signing the key AppSetting:Token with hashing algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //4-create a token descriptor, based on the claims, expiration day and hashed key
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds
            };

            //5- JwtSecurityTokenHandler allows us to create a token based on the token passed above 
            var tokenHandler = new JwtSecurityTokenHandler();
            //6- store the token in var token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            //return the token to the client who successfully logged in!
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}