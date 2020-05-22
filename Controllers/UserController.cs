using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    [Authorize(Roles = "manager")]
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;

        public UserController(DataContext context,
                              TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await _context
                .Users
                .AsNoTracking()
                .ToListAsync();

            return users;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                model.Password = null;

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = await _context
                .Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username &&
                            x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = _tokenService.GenerateToken(user);

            return new
            {
                token = token
            };
        }
    }
}