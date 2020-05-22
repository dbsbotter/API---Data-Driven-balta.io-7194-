using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var products = await _context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();

            return products;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(product);
        }

        [HttpGet("categories/{id:int}")]
        public async Task<ActionResult<Product>> GetByCategory(int id)
        {
            var products = await _context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar o produto no banco de dados." });
            }
        }
    }
}