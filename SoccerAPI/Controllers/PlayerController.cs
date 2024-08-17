using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoccerAPI.Models;

namespace SoccerAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        Context context;
        public PlayerController()
        {
            context = new Context();
        }

        [HttpGet]
        public IActionResult GetPlayers()
        {
            if (context.Players == null)
            {
                return NotFound("There are no players !!!!");
            }

            var player = context.Players
                .Include(x => x.Club)
                .Select(x => new
                {
                    x.Id,
                    x.name,
                    x.description,
                    x.position,
                    x.nationality,
                    x.salary,
                    x.speed,
                    x.passes,
                    x.strength,
                    x.agility,
                    x.gender,
                    Clubname = x.Club.name,
                })
                .ToList();

            return Ok(player);
        }

        [HttpGet("{id}")]
        public IActionResult GetPlayer(int id)
        {
            if (context.Players is null)
            {
                return NotFound("There are no players !!!!");
            }

            var player = context.Players
               .Include(x => x.Club)
               .Where(x=>x.Id == id)
               .Select(x => new
               {
                   x.Id,
                   x.name,
                   x.description,
                   x.position,
                   x.nationality,
                   x.salary,
                   x.speed,
                   x.passes,
                   x.strength,
                   x.agility,
                   x.gender,
                   Clubname = x.Club.name,
               })
               .FirstOrDefault();

            if (player == null)
            {
                return NotFound("No players detected !!!!!");
            }
            return Ok(player);
        }


        [HttpPost]
        public IActionResult PostPlayer(Player player)
        {
            if (context.Players is null)
            {
                return NotFound("There are no players !!!!");
            }

            context.Players.Add(player);

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!PlayerExist(player.Id))
                {
                    NotFound();
                }
                else
                {
                    throw ;
                }
            }
            
            return CreatedAtAction("GetPlayer", new {id = player.Id }, player);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePlayer(int id)
        {
            if (id == 0)
            {
                return BadRequest("No Id detected !!!!");
            }

            if (context.Players is null)
            {
                return NotFound("There are no players !!!!");
            }

            var player = context.Players.Find(id);
            context.Players.Remove(player);
            context.SaveChanges();
            return Ok();
        }


        [HttpPut("{id}")]
        public IActionResult PutPlayer(int id, Player player) 
        {
            if(id == 0)
            {
                return BadRequest("No Id detected !!!!");
            }
            if(id != player.Id)
            {
                return BadRequest("Not the same ID !!!!");
            }

            context.Entry(player).State = EntityState.Modified;
            
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(player);
        }


        [HttpPatch("{id}")]
        public IActionResult PatchPlayer(int id, [FromBody] JsonPatchDocument<Player> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            var player = context.Players.Find(id);

            if (player == null)
            {
                return NotFound("Player not found.");
            }

            patchDoc.ApplyTo(player,ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Update(player);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(player);
        }


        private bool PlayerExist(int id)
        {
            return context.Players.Any(p => p.Id == id);
        }
    }
}
