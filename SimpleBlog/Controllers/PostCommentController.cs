using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SimpleBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleBlog.Controllers
{
    public class PostCommentController : Controller
    {

        private readonly ApplicationDbContext _context;

        public PostCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = new PostCommentViewModel();
            model.Post = await _context.Post
                .Include(Post => Post.Comments)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (model.Post == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
