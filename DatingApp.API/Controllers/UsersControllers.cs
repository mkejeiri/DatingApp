using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    //http://localhost:5000/api/[controller]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
                
        // [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users= await _repo.GetUsers();  
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }
       
        // [AllowAnonymous]
        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {            
            var user = await _repo.GetUser(id);  
            var userToReturn =_mapper.Map<UserForDetailedDto>(user);             
             return Ok(userToReturn);
        }

        [HttpPut("{id}")]
            public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
            {   

                /*
                  we need to match the user attempting to update his profile matching the id which part of the token in the server
                */         
                if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))   return Unauthorized();
                
                var userFroRepo = await _repo.GetUser(id);  
                _mapper.Map(userForUpdateDto, userFroRepo); 

                if (await _repo.SaveAll()) return NoContent();
                
                throw new Exception($"updating user with {id} failed");
            }

        
    }
}
