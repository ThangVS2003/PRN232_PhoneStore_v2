using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DbPhoneStoreContext _context;

        public CommentRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .Include(c => c.Reply) // comment được trả lời
                .Include(c => c.InverseReply) // các comment con trả lời comment này
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .Include(c => c.Reply)
                .Include(c => c.InverseReply)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            var existing = await _context.Comments.FirstOrDefaultAsync(c => c.Id == comment.Id);
            if (existing != null)
            {
                existing.Content = comment.Content;
                existing.UserId = comment.UserId;
                existing.ProductId = comment.ProductId;
                existing.ReplyId = comment.ReplyId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}