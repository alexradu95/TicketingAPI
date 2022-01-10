using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatersTicketingAPI.Data;
using WatersTicketingAPI.DTO.User;
using WatersTicketingAPI.Models;
using WatersTicketingAPI.Services;

namespace WatersTicketingAPI.Controllers;

[Route("user")]
public class UserController : ControllerBase
{
    private readonly IMapper mapper;

    public UserController(IMapper mapper)
    {
        this.mapper = mapper;
    }


    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<dynamic>> Authenticate([FromBody] UserLoginDto model,
        [FromServices] DataContext dbContext)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            User user = await dbContext.Users.AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new { message = "Invalid User or Password..." });

            string token = TokenService.GenerateToken(user);

            return new
            {
                user = mapper.Map<UserDto>(user),
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
    public async Task<ActionResult<List<UserDto>>> Get([FromServices] DataContext dbContext)
    {
        try
        {
            var users = await dbContext.Users.Select(user => mapper.Map<UserDto>(user)).ToListAsync();
            return users.Count == 0 ? NotFound(new { message = "users not found." }) : Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not get users. Error: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> GetById([FromRoute] int id, [FromServices] DataContext dbContext)
    {
        try
        {
            UserDto user = mapper.Map<UserDto>(await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id));
            return user == null ? NotFound(new { message = "user not found." }) : Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not get user. Error: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("{username}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> GetByUsername([FromRoute] string username,
        [FromServices] DataContext dbContext)
    {
        try
        {
            UserDto user =
                mapper.Map<UserDto>(await dbContext.Users.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == username));
            return user == null ? NotFound(new { message = "user not found." }) : Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not get user. Error: {ex.Message}" });
        }
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> Post([FromBody] UserRegisterDto model,
        [FromServices] DataContext dbContext)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        model.Role ??= "buyer";

        try
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == model.Username);
            if (user != null) return BadRequest(new { message = "Could not create User with that username" });
            dbContext.Users.Add(mapper.Map<User>(model));
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
    public async Task<ActionResult<UserDto>> Put([FromBody] UserEditDto model, [FromServices] DataContext dbContext)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == model.Username);

            if (user == null)
                return NotFound(new { message = "user not found" });

            user.Password = model.Password;
            dbContext.Entry(user).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return BadRequest(new
                { message = $"Could not Update this User (Concurrency exception). Error: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not Update this User. Error: {ex.Message}" });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> Delete([FromRoute] int id, [FromServices] DataContext dbContext)
    {
        try
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
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