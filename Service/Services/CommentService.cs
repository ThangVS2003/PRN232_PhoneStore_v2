using BusinessObject.Models;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _commentRepository.GetAllAsync();
        }

        public async Task<Comment> GetByIdAsync(int id)
        {
            return await _commentRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Comment comment)
        {
            await _commentRepository.AddAsync(comment);
        }

        public async Task UpdateAsync(Comment comment)
        {
            await _commentRepository.UpdateAsync(comment);
        }

        public async Task DeleteAsync(int id)
        {
            await _commentRepository.DeleteAsync(id);
        }
    }
}
