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

namespace WatersTicketingAPI.Controllers
{
    [Route("user")]
    public class UserController : ControllerBase
    {
        

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody]User model, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var user = await context.Users.AsNoTracking().Where( x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound(new { message = "Invalid User or Password..."});
                var token = TokenService.GenerateToken(user);
                //TODO - I should create a DTO in order to not set the password to empty
                user.Password = "";
                return new {
                    user = user,
                    token = token
                }; 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not authenticate. Error: {ex.Message}"});
            }
        }

        

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None , Duration = 0, NoStore = true)]
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context)
        {
            try
            {
                var users = await context.Users.AsNoTracking().ToListAsync();
                if(users == null || users.Count == 0 )
                    return NotFound(new { message = "users not found."});
                foreach(var user in users)
                {
                    //TODO - I should create a DTO in order to not set the password to empty
                    user.Password = "";
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get users. Error: {ex.Message}"});
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None , Duration = 0, NoStore = true)]
        public async Task<ActionResult<User>> GetById([FromRoute] int id, [FromServices] DataContext context)
        {
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if(user == null)
                    return NotFound(new { message = "user not found."});
                user.Password = "";
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get user. Error: {ex.Message}"});
            }   
        }

        [HttpGet]
        [Route("{username}")]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None , Duration = 0, NoStore = true)]
        public async Task<ActionResult<User>> GetByUsername([FromRoute] string username, [FromServices] DataContext context)
        {
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
                if(user == null)
                    return NotFound(new { message = "user not found."});
                user.Password = "";
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get user. Error: {ex.Message}"});
            }   
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None , Duration = 0, NoStore = true)]
        public async Task<ActionResult<User>> Post([FromBody]User model, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if(model.Role == null){
                model.Role = "visitor";
            }
            
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == model.Username);
                if(user != null)
                    return BadRequest(new { message = $"Could not create User with that username"});
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return Ok(new { message = $"User {model.Username} created"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not create User. Error: {ex.Message}"});
            }
        }
        [HttpPut]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None , Duration = 0, NoStore = true)]
        public async Task<ActionResult<User>> Put([FromBody]User model, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
                if(user == null)
                    return NotFound(new { message = "user not found"});
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                model.Password = "";
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = $"Could not Update this User (Concurrency exception). Error: {ex.Message}"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not Update this User. Error: {ex.Message}"});
            }
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None , Duration = 0, NoStore = true)]
        public async Task<ActionResult<User>> Delete([FromRoute] int id, [FromServices]DataContext context)
        {
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if(user == null)
                    return NotFound(new { message = "user not found"});
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new { message = "user removed"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not remove this User. Error: {ex.Message}"});
            }
        }
    }
}