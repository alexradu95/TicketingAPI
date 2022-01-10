using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatersTicketingAPI.Data;
using WatersTicketingAPI.DTO.Ticket;
using WatersTicketingAPI.Models;
using WatersTicketingAPI.Utils;

namespace WatersTicketingAPI.Controllers;

[Route("tickets")]
public class TicketController : ControllerBase
{
    private readonly IMapper mapper;

    public TicketController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public async Task<ActionResult<List<TicketDto>>> Get([FromServices] DataContext dbContext)
    {
        try
        {
            var tickets = await dbContext.Tickets.ToListAsync();
            return tickets.Count == 0
                ? NotFound(new { message = "Tickets not found." })
                : Ok(mapper.Map<List<TicketDto>>(tickets));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<TicketDto>> GetById([FromRoute] int id, [FromServices] DataContext dbContext)
    {
        try
        {
            Ticket tickets = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
            return tickets == null
                ? NotFound(new { message = "Tickets not found." })
                : Ok(mapper.Map<TicketDto>(tickets));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}" });
        }
    }


    [HttpGet]
    [Route("myTickets")]
    [Authorize(Roles = "admin, seller")]
    public async Task<ActionResult<Ticket>> GetMyTickets([FromServices] DataContext dbContext)
    {
        try
        {
            string username = this.ExtractUsernameFromClaim();

            if (string.IsNullOrEmpty(username))
                return BadRequest(new { message = "Invalid Session. Please authenticate" });

            var tickets = await dbContext.Tickets.Where(x => x.CreatedBy.Username == username).ToListAsync();

            return !tickets.Any() ? NotFound(new { message = "Tickets not found." }) : Ok(tickets);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}" });
        }
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "admin, seller")]
    public async Task<ActionResult<TicketDto>> Post([FromBody] Ticket model, [FromServices] DataContext dbContext)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            string username = this.ExtractUsernameFromClaim();

            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null) return BadRequest("You are not authenticated");

            model.UserId = user.Id;
            dbContext.Tickets.Add(model);
            await dbContext.SaveChangesAsync();
            return Ok(model);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not create ticket. Error: {ex.Message}" });
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "admin, seller")]
    public async Task<ActionResult<TicketDto>> Put([FromBody] TicketEditDto model, [FromRoute] int id,
        [FromServices] DataContext dbContext)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string username = this.ExtractUsernameFromClaim();
        User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
        Ticket ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);

        if (user == null) return BadRequest("You are not authenticated or registered.");
        if (ticket == null) return BadRequest("The ticket with id: {{id}} does not exist.");
        if (ticket.UserId != user.Id) return BadRequest("You don't own this ticket. You can't modify it");

        ticket.Description = model.Description;
        ticket.Price = model.Price;

        try
        {
            dbContext.Entry(ticket).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return BadRequest(new
                { message = $"Could not Update this Ticket (Concurrency exception). Error: {ex.Message}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not Update this Ticket. Error: {ex.Message}" });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "admin, seller")]
    public async Task<ActionResult<TicketDto>> Delete([FromRoute] int id, [FromServices] DataContext dbContext)
    {
        try
        {
            Ticket product = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null) return NotFound(new { message = "Ticket Not Found" });

            dbContext.Tickets.Remove(product);
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Ticket Removed" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Could not Delete this ticket. Error: {ex.Message}" });
        }
    }
}