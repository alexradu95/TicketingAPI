using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatersTicketingAPI.Data;
using WatersTicketingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using WatersTicketingAPI.Services;
using AutoMapper;
using WatersTicketingAPI.DTO;

namespace WatersTicketingAPI.Controllers
{
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;

        public UserController(IMapper mapper)
        {
            _mapper = mapper;
        }


        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] UserLoginDTO model, [FromServices] DataContext dbContext)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var user = await dbContext.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound(new { message = "Invalid User or Password..." });

                var token = TokenService.GenerateToken(user);

                return new
                {
                    user = _mapper.Map<UserDTO>(user),
                    token //Inferred name
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not authenticate. Error: {ex.Message}" });
            }
        }



        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromServices] DataContext dbContext)
        {
            try
            {
                var users = await dbContext.Users.Select(user => _mapper.Map<UserDTO>(user)).ToListAsync();
                return (users == null || users.Count == 0) ? NotFound(new { message = "users not found." }) : Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get users. Error: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> GetById([FromRoute] int id, [FromServices] DataContext dbContext)
        {
            try
            {
                var user = _mapper.Map<UserDTO>(await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id));
                return (user == null) ? NotFound(new { message = "user not found." }) : Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get user. Error: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("{username}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> GetByUsername([FromRoute] string username, [FromServices] DataContext dbContext)
        {
            try
            {
                var user = _mapper.Map<UserDTO>(await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username));
                return (user == null) ? NotFound(new { message = "user not found." }) : Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get user. Error: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> Post([FromBody] UserRegisterDTO model, [FromServices] DataContext dbContext)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Role == null)
            {
                model.Role = "buyer";
            }

            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == model.Username);
                if (user != null)
                {
                    return BadRequest(new { message = $"Could not create User with that username" });
                }
                dbContext.Users.Add(_mapper.Map<User>(model));
                await dbContext.SaveChangesAsync();
                return Ok(new { message = $"User {model.Username} created" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not create User. Error: {ex.Message}" });
            }
        }
        [HttpPut]
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> Put([FromBody] UserEditDTO model, [FromServices] DataContext dbContext)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == model.Username);

                if (user == null)
                    return NotFound(new { message = "user not found" });

                user.Password = model.Password;
                dbContext.Entry<User>(user).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = $"Could not Update this User (Concurrency exception). Error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not Update this User. Error: {ex.Message}" });
            }
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDTO>> Delete([FromRoute] int id, [FromServices] DataContext dbContext)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                    return NotFound(new { message = "user not found" });
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "user removed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not remove this User. Error: {ex.Message}" });
            }
        }
    }
}