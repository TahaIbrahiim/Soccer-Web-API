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
    public class ClubController : ControllerBase
    {
        Context context;
        public ClubController()
        {
            context = new Context();
        }

        [HttpGet]
        public IActionResult GetClubs()
        {
            if (context.Clubs == null)
            {
                return NotFound("There are no Clubs !!!!");
            }

            var club = context.Clubs
               .Include(c => c.Coach) // Include the Coach entity
               .Select(c => new
               {
                   c.ClubId,
                   c.name,
                   c.Owner,
                   c.description,
                   c.league,
                   c.stadium,
                   c.sponsor,
                   CoachName = c.Coach.name, // Coach name instead of Coach ID
                   Playersname = c.Players.Select(p => p.name),
               })
               .ToList();

            return Ok(club);
        }

        [HttpGet("{id}")]
        public IActionResult GetClub(int id)
        {
            if (context.Clubs is null)
            {
                return NotFound("There are no Clubs !!!!");
            }

          var club = context.Clubs
         .Include(c => c.Coach) 
         .Where(c => c.ClubId == id)
         .Select(c => new
         {
             c.ClubId,
             c.name,
             c.Owner,
             c.description,
             c.league,
             c.stadium,
             c.sponsor,
             CoachName = c.Coach.name // Coach Name
         })
         .FirstOrDefault();


            if (club == null)
            {
                return NotFound("No Clubs detected !!!!!");
            }
            return Ok(club);
        }


        [HttpPost]
        public IActionResult PostClub(Club Club)
        {
            if (context.Clubs is null)
            {
                return NotFound("There are no Clubs !!!!");
            }

            context.Clubs.Add(Club);

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExist(Club.ClubId))
                {
                    NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetClub", new { id = Club.ClubId }, Club);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClub(int id)
        {
            if (id == 0)
            {
                return BadRequest("No Id detected !!!!");
            }

            if (context.Clubs is null)
            {
                return NotFound("There are no Clubs !!!!");
            }

            var Club = context.Clubs.Find(id);
            context.Clubs.Remove(Club);
            context.SaveChanges();
            return Ok();
        }


        [HttpPut("{id}")]
        public IActionResult PutClub(int id, Club Club)
        {
            if (id == 0)
            {
                return BadRequest("No Id detected !!!!");
            }
            if (id != Club.ClubId)
            {
                return BadRequest("Not the same ID !!!!");
            }

            context.Entry(Club).State = EntityState.Modified;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(Club);
        }


        [HttpPatch("{id}")]
        public IActionResult PatchClub(int id, [FromBody] JsonPatchDocument<Club> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            var club = context.Clubs.Find(id);

            if (club == null)
            {
                return NotFound("Club not found.");
            }

            patchDoc.ApplyTo(club, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Update(club);
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(club);
        }


   

        private bool ClubExist(int id)
        {
            return (context.Clubs?.Any(p => p.ClubId == id)).GetValueOrDefault();
        }
    }
}
