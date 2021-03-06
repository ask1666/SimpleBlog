﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleBlog.Data;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Home()
        {

            var posts = from p in _context.Post.Include(Post => Post.Creator) select p;

            return View(await posts.ToListAsync());
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchString)
        {
            
            var posts = from p in _context.Post.Include(Post => Post.Creator) select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString));
            }
            return View(await posts.ToListAsync());
        }

        //GET: MyPosts
        public async Task<IActionResult> MyIndex()
        {
            User currentUser = await _userManager.GetUserAsync(User);
            List<Post> posts = await _context.Post.Include(Post => Post.Creator).Include(Post => Post.Comments).ToListAsync();
            List<Post> myPosts = new List<Post>();

            foreach (var post in from post in posts where post.Creator == currentUser select post)
            {
                myPosts.Add(post);
            }

            return View(myPosts);
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
                .Include(Post => Post.Comments).ThenInclude(Comment => Comment.Creator)
                .Include(Post => Post.Creator)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (model.Post == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,Title,Text")] Post post)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            post.Creator = currentUser;
            post.CreatedTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                await _context.AddAsync(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([Bind("ID,Text,Post")] Comment comment, [Bind("ID")] Post post)
        {

            User currentUser = await _userManager.GetUserAsync(User);
            Post currentPost = await _context.Post.FirstOrDefaultAsync(m => m.ID == post.ID);
            comment.Post = currentPost;
            comment.Creator = currentUser;
            comment.CreatedTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details",new { post.ID });
            }
            return View();
        }

        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Text")] Post post)
        {
            post.CreatedTime = DateTime.Now;
            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            //var creator = await _context.Users.FindAsync(post.Creator.Id);
            //creator.Posts.Remove(post);
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);
        }
    }
}
