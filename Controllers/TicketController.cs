using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WatersTicketingAPI.Data;
using WatersTicketingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using WatersTicketingAPI.Utils;
using AutoMapper;
using WatersTicketingAPI.DTO;

namespace WatersTicketingAPI.Controllers
{
    [Route("tickets")]
    public class TicketController : ControllerBase
    {
        private readonly IMapper _mapper;

        public TicketController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TicketDTO>>> Get([FromServices] DataContext dbContext)
        {
            try
            {
                var tickets = await dbContext.Tickets.ToListAsync();
                return (tickets == null || tickets.Count == 0) ? NotFound(new { message = "Tickets not found." }) : Ok(_mapper.Map<List<TicketDTO>>(tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}"});
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<TicketDTO>> GetById([FromRoute] int id, [FromServices] DataContext dbContext)
        {
            try
            {
                var tickets = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
                return (tickets == null) ? NotFound(new { message = "Tickets not found." }) : Ok(_mapper.Map<TicketDTO>(tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}"});
            }   
        }


        [HttpGet]
        [Route("myTickets")]
        [Authorize(Roles = "admin, seller")]
        public async Task<ActionResult<Ticket>> GetMyTickets([FromServices] DataContext dbContext)
        {
            try
            {
                var username = this.ExtractUsernameFromClaim();

                if(String.IsNullOrEmpty(username))
                {
                    return BadRequest(new { message = $"Invalid Session. Please authenticate" });
                }

                List<Ticket> tickets = await dbContext.Tickets.Where(x => x.CreatedBy.Username == username).ToListAsync();

                return (tickets == null ? NotFound(new { message = "Tickets not found." }) : Ok(tickets));

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin, seller")]
        public async Task<ActionResult<TicketDTO>> Post([FromBody]Ticket model, [FromServices]DataContext dbContext)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {

                var username = this.ExtractUsernameFromClaim();

                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

                model.UserId = user.Id;

                dbContext.Tickets.Add(model);
                await dbContext.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not create ticket. Error: {ex.Message}"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "admin, seller")]
        public async Task<ActionResult<TicketDTO>> Put([FromBody] TicketEditDTO model, [FromRoute] int id, [FromServices]DataContext dbContext)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = this.ExtractUsernameFromClaim();
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
            var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);

            if (ticket.UserId != user.Id)
            {
                return BadRequest("You don't own this ticket. You can't modify it");
            }

            ticket.Description = model.Description;
            ticket.Price = model.Price;

            try
            {
                dbContext.Entry<Ticket>(ticket).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = $"Could not Update this Ticket (Concurrency exception). Error: {ex.Message}"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not Update this Ticket. Error: {ex.Message}"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "admin, seller")]
        public async Task<ActionResult<TicketDTO>> Delete([FromRoute]int id, [FromServices]DataContext dbContext)
        {
            
            try
            {
                var product = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);

                if(product == null) { 
                    return NotFound(new { message = "Ticket Not Found"});
                }

                dbContext.Tickets.Remove(product);
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Ticket Removed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not Delete this ticket. Error: {ex.Message}"});
            }
            
        }
    }
}