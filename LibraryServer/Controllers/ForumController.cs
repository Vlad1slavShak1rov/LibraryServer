using LibraryServer.DbContext;
using LibraryServer.DTO.Forum;
using LibraryServer.Model;
using LibraryServer.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibraryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {
        private readonly ForumService _forumService;
        public ForumController(ForumService forumService)
        {
           _forumService = forumService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetForums()
        {
            var forums = await _forumService.GetAll();
            return Ok(forums);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetForum(int id)
        {
            var forum = await _forumService.GetById(id);

            if (forum == null)
                return NotFound();

            return Ok(forum);
        }

      
        [HttpPost]
        public async Task<ActionResult<object>> CreateForum([FromBody] CreateForumDto dto)
        {
            try
            {
                var forum = await _forumService.CreateForum(dto);

                return Ok(new
                {
                    acces = true,
                    msg = "Forum created!"
                });
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

          
        }

        [HttpGet("messages/{forumId}")]
        public async Task<IActionResult> GetForumMessages(int forumId)
        {
            try
            {
                var messages = _forumService.GetForumMessage(forumId);
                return Ok(messages);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("byBookId/{id}")]
        public async Task<IActionResult> GetByBookId(int bookId)
        {
            try
            {
                var forum = await _forumService.GetByBookId(bookId);
                return Ok(forum);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
