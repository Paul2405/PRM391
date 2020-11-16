using System;
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
        public async Task<ICollection<Video>> GetVideo([FromQuery] int pageSize, [FromQuery] int pageNum)
        {
            List<Video> videos = _context.Video.Include(v => v.Likes).Include(v => v.Comments).ThenInclude(c => c.User).Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

            return videos;
        }

        [HttpGet("title")]
        public async Task<ICollection<Video>> GetVideoByTitle([FromQuery] String title, [FromQuery] int pageSize, [FromQuery] int pageNum)
        {
            List<Video> videos = _context.Video.Where(v => v.Title.Contains(title)).Include(v => v.Likes).Include(v => v.Comments).ThenInclude(c => c.User).Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();

            return videos;
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
        [HttpPut]
        public async Task<IActionResult> PutVideo(Video video)
        {
            Video existedVideo = _context.Video.Where(v => v.Id == video.Id).FirstOrDefault();
            existedVideo.Title = video.Title;
            existedVideo.Decription = video.Decription;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(video.Id))
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
            Video video = _context.Video.Where(v => v.Id == id).FirstOrDefault();
            if (video == null)
            {
                return NotFound();
            }
            video.Status = false;
            await _context.SaveChangesAsync();

            List<Like> likes = _context.Like.Where(l => l.VideoId == id).ToList();
            foreach (Like like in likes)
            {
                Like existedLike = _context.Like.Where(l => l.Id == like.Id).FirstOrDefault();
                existedLike.Status = false;
            }

            await _context.SaveChangesAsync();


            List<Comment> comments = _context.Comment.Where(c => c.VideoId == id).ToList();
            foreach (Comment c in comments)
            {
                Comment existedComment = _context.Comment.Where(l => l.Id == c.Id).FirstOrDefault();
                existedComment.Status = false;
            }
            await _context.SaveChangesAsync();

            return video;
        }

        private bool VideoExists(int id)
        {
            return _context.Video.Any(e => e.Id == id);
        }

        //Like
        [HttpGet("{Id}/Like")]
        public async Task<ICollection<Like>> GetUserLikefromVideo(int Id)
        {
            return await _context.Like.Where(c => c.VideoId == Id).Include(c => c.User).ToListAsync();
        }

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

        [HttpDelete("Like/{id}")]
        public async Task<ActionResult<Like>> UnlikeVideo(int id)
        {

            var like = _context.Like.Where(l => l.Id == id).FirstOrDefault();

            like.Status = false;
            await _context.SaveChangesAsync();


            var totalLike = _context.Video.Where(v => v.Id == like.VideoId).Select(v => v.LikeCount).FirstOrDefault();

            var video = new Video()
            {
                Id = like.VideoId,
                LikeCount = --totalLike
            };

            _context.Video.Attach(video).Property(v => v.LikeCount).IsModified = true;
            await _context.SaveChangesAsync();

            return like;
        }

        //Comment
        [HttpGet("{Id}/Comment")]
        public async Task<ICollection<Comment>> GetCommentfromVideo(int Id,[FromQuery] int pageSize, [FromQuery] int pageNum)
        {
            return await _context.Comment.Where(c => c.VideoId == Id).Include(c => c.User).Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
        }

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
            await _context.SaveChangesAsync();
            return comment;
        }

        [HttpDelete("Comment")]
        public async Task<ActionResult<Comment>> DeleteCommentVideo(int Id)
        {
            Comment existedComment = _context.Comment.Where(c => c.Id == Id).FirstOrDefault();
            existedComment.Status = false;
            await _context.SaveChangesAsync();

            var totalComment = _context.Video.Where(v => v.Id == existedComment.VideoId).Select(v => v.CommentCount).FirstOrDefault();

            var video = new Video()
            {
                Id = existedComment.VideoId,
                CommentCount = --totalComment
            };

            _context.Video.Attach(video).Property(v => v.CommentCount).IsModified = true;
            await _context.SaveChangesAsync();

            return existedComment;
        }


    }
}