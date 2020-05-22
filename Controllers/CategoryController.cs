using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        // [ResponseCache(Location = ResponseCacheLocation.None, Duration = 0, NoStore = true)]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar a categoria no banco de dados." });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model)
        {
            if (model.Id != id)
                return BadRequest(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry<Category>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado." });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível alterar a categoria no banco de dados." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover a categoria no banco de dados." });
            }
        }
    }
}