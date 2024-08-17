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
    public class CoachController : ControllerBase
    {
        Context context;
        public CoachController()
        {
            context = new Context();
        }

        [HttpGet]
        public IActionResult GetCoaches()
        {
            if (context.Coaches == null)
            {
                return NotFound("There are no coashes !!!!");
            }

            var coach = context.Coaches.Include(c => c.Clubs).Select(c => new
            {
                c.CoachId,
                c.name,
                c.Salary,
                c.nationality,
                c.age,
                c.description,
                c.gender,
                Teams = c.Clubs.Select(c => c.name)
            })
            .ToList();

            return Ok(coach);
        }

        [HttpGet("{id}")]
        public IActionResult GetCoach(int id)
        {
            if (context.Coaches is null)
            {
                return NotFound("There are no coaches !!!!");
            }

            var coach = context.Coaches
                .Include(c => c.Clubs)
                .Where(c => c.CoachId == id)
                .Select(c => new
                {
                    c.CoachId,
                    c.name,
                    c.Salary,
                    c.nationality,
                    c.age,
                    c.description,
                    c.gender,
                    Teams = c.Clubs.Select(c => c.name)
                })
                .FirstOrDefault();

            if (coach == null)
            {
                return NotFound("No coaches detected !!!!!");
            }
            return Ok(coach);
        }


        [HttpPost]
        public IActionResult PostCoach(Coach coach)
        {
            if (context.Coaches is null)
            {
                return NotFound("There are no coaches !!!!");
            }

            context.Coaches.Add(coach);

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoachExist(coach.CoachId))
                {
                    NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCoach", new { id = coach.CoachId }, coach);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCoach(int id)
        {
            if (id == 0)
            {
                return BadRequest("No Id detected !!!!");
            }

            if (context.Coaches is null)
            {
                return NotFound("There are no Coaches !!!!");
            }

            var coach = context.Coaches.Find(id);
            context.Coaches.Remove(coach);
            context.SaveChanges();
            return Ok();
        }


        [HttpPut("{id}")]
        public IActionResult PutCoach(int id, Coach coach)
        {
            if (id == 0)
            {
                return BadRequest("No Id detected !!!!");
            }
            if (id != coach.CoachId)
            {
                return BadRequest("Not the same ID !!!!");
            }

            context.Entry(coach).State = EntityState.Modified;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoachExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(coach);
        }

        [HttpPatch("{id}")]
        public IActionResult PatchCoach(int id, [FromBody] JsonPatchDocument<Coach> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest("Invalid patch document.");
            }

            var coach = context.Coaches.Find(id);

            if (coach == null)
            {
                return NotFound("Coach not found.");
            }

            patchDoc.ApplyTo(coach, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Update(coach);
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoachExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(coach);
        }


      

        private bool CoachExist(int id)
        {
            return (context.Coaches?.Any(p => p.CoachId == id)).GetValueOrDefault();
        }
    }
}
