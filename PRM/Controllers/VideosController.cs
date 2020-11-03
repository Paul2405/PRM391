﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRM.Models;
using SQLitePCL;

namespace PRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly PRMContext _context;

        public VideosController(PRMContext context)
        {
            _context = context;
        }

        // GET: api/Videos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideo()
        {
            return await _context.Video.ToListAsync();
        }

        // GET: api/Videos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = _context.Video.Where(v => v.Id == id).Include(v => v.Likes).Include(v => v.Comments).ThenInclude(c => c.User).SingleOrDefault();

            if (video == null)
            {
                return NotFound();
            }

            return video;
        }

        // PUT: api/Videos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, Video video)
        {
            if (id != video.Id)
            {
                return BadRequest();
            }

            _context.Entry(video).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Videos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo(Video video)
        {
            _context.Video.Add(video);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VideoExists(video.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVideo", new { id = video.Id }, video);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Video>> DeleteVideo(int id)
        {
            var video = await _context.Video.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            _context.Video.Remove(video);
            await _context.SaveChangesAsync();

            return video;
        }

        private bool VideoExists(int id)
        {
            return _context.Video.Any(e => e.Id == id);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<Video>> GetVideoByID(int id)
        {
            var video = _context.Video.Where(v => v.Id == id).Include(v => v.Likes).ThenInclude(c => c.User).Include(v => v.Comments).ThenInclude(c => c.User).SingleOrDefault();

            if (video == null)
            {
                return NotFound();
            }

            return video;
        }

        //Like

        [HttpPost("Like")]
        public async Task<ActionResult<Like>> LikeVideo(Like like)
        {
            Like chkLike = _context.Like.Where(l => l.UserId == like.UserId && l.VideoId == like.VideoId).FirstOrDefault();
            if (chkLike != null && chkLike.Status == false)
            {
                chkLike.Status = true;
                _context.SaveChanges();
            }
            else
            {
                var likeModel = _context.Like.Add(like);
                await _context.SaveChangesAsync();
            }

            var totalLike = _context.Video.Where(v => v.Id == like.VideoId).Select(v => v.LikeCount).FirstOrDefault();

            var video = new Video()
            {
                Id = like.VideoId,
                LikeCount = ++totalLike
            };

            _context.Video.Attach(video).Property(v => v.LikeCount).IsModified = true;
            var chk = _context.SaveChanges();

            return like;
        }

        [HttpDelete("Like")]
        public async Task<ActionResult<Like>> UnlikeVideo(int userID, int videoID)
        {

            var like = _context.Like.Where(l => l.VideoId == videoID && l.UserId == userID).FirstOrDefault();

            like.Status = false;
            _context.SaveChanges();

            var totalLike = _context.Video.Where(v => v.Id == like.VideoId).Select(v => v.LikeCount).FirstOrDefault();

            var video = new Video()
            {
                Id = like.VideoId,
                LikeCount = --totalLike
            };

            _context.Video.Attach(video).Property(v => v.LikeCount).IsModified = true;
            _context.SaveChanges();
            return like;
        }

        //Comment

        [HttpPost("Comment")]
        public async Task<ActionResult<Comment>> CommentVideo(Comment comment)
        {
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            var totalComment = _context.Video.Where(v => v.Id == comment.VideoId).Select(v => v.CommentCount).FirstOrDefault();

            var video = new Video()
            {
                Id = comment.VideoId,
                CommentCount = ++totalComment
            };

            _context.Video.Attach(video).Property(v => v.CommentCount).IsModified = true;
            _context.SaveChanges();
            return comment;
        }

        [HttpPut("Comment")]
        public async Task<ActionResult<Comment>> UpdateCommentVideo(Comment comment)
        {
            Comment existedComment = _context.Comment.Where(c => c.Id == comment.Id).FirstOrDefault();
            existedComment.Conttent = comment.Conttent;
            _context.SaveChanges();
            return comment;
        }

        [HttpDelete("Comment")]
        public async Task<ActionResult<Comment>> DeleteCommentVideo(int Id)
        {
            Comment existedComment = _context.Comment.Where(c => c.Id == Id).FirstOrDefault();
            existedComment.Status = false;
            _context.SaveChanges();
            var totalComment = _context.Video.Where(v => v.Id == existedComment.VideoId).Select(v => v.CommentCount).FirstOrDefault();

            var video = new Video()
            {
                Id = existedComment.VideoId,
                CommentCount = --totalComment
            };

            _context.Video.Attach(video).Property(v => v.CommentCount).IsModified = true;
            _context.SaveChanges();
            return existedComment;
        }

    }
}
