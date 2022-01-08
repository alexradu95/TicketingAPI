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

namespace WatersTicketingAPI.Controllers
{
    [Route("tickets")]
    public class TicketController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any,Duration = 30)]
        public async Task<ActionResult<List<Ticket>>> Get([FromServices] DataContext context)
        {
            try
            {
                var products = await context.Tickets.ToListAsync();
                if(products == null || products.Count == 0 )
                    return NotFound(new { message = "Tickets not found."});
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}"});
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any,Duration = 30)]
        public async Task<ActionResult<Ticket>> GetById([FromRoute] int id, [FromServices] DataContext context)
        {
            try
            {
                var product = await context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
                if(product == null)
                    return NotFound(new { message = "Tickets not found." });
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not get tickets. Error: {ex.Message}"});
            }   
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Ticket>> Post([FromBody]Ticket model, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {

                var username = this.ExtractUsernameFromClaim();

                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

                model.UserId = user.Id;

                context.Tickets.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not create ticket. Error: {ex.Message}"});
            }
        }

        [HttpPut]
        [Route("")]

        public async Task<ActionResult<Ticket>> Put([FromBody] Ticket model, [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);


            var username = this.ExtractUsernameFromClaim();

            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
            var ticket = await context.Tickets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);

            model.UserId = ticket.UserId;

            if (ticket.UserId != user.Id)
            {
                return BadRequest("You don't own this ticket. You can't modify it");
            }

            try
            {
                context.Entry<Ticket>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Ticket>> Delete([FromRoute]int id, [FromServices]DataContext context)
        {
            
            try
            {
                var product = await context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
                if(product == null)
                    return NotFound(new { message = "Ticket Not Found"});
                context.Tickets.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Ticket Removed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not Delete this ticket. Error: {ex.Message}"});
            }
            
        }
    }
}