using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var item = _context.CelestialObjects.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            return Ok(item);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var items = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (!items.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in items)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _context.CelestialObjects.ToList();
            foreach (var celestialObject in items)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(items);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var item = _context.CelestialObjects.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = celestialObject.Name;
            item.OrbitalPeriod = celestialObject.OrbitalPeriod;
            item.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(item);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var item = _context.CelestialObjects.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = name;
            _context.CelestialObjects.Update(item);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var items = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);
            if (!items.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(items);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
