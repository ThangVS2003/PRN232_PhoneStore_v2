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
    public class SerialRepository : ISerialRepository
    {
        private readonly DbPhoneStoreContext _context;

        public SerialRepository(DbPhoneStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Serial>> GetAllAsync()
        {
            return await _context.Serials
                .Include(s => s.ProductVariant) // Load navigation property if needed
                .ToListAsync();
        }

        public async Task<Serial?> GetByIdAsync(int id)
        {
            return await _context.Serials
                .Include(s => s.ProductVariant)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Serial>> SearchAsync(string keyword)
        {
            var query = _context.Serials
                .Include(s => s.ProductVariant)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(s => s.SerialNumber.Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Serial>> SearchByProductVariantIdAsync(string keyword, int? productVariantId)
        {
            var query = _context.Serials
                .Include(s => s.ProductVariant)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(s => s.SerialNumber.Contains(keyword));
            }

            if (productVariantId.HasValue)
            {
                query = query.Where(s => s.ProductVariantId == productVariantId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Serial serial)
        {
            await _context.Serials.AddAsync(serial);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Serial serial)
        {
            var existingSerial = await _context.Serials.FindAsync(serial.Id);
            if (existingSerial != null)
            {
                existingSerial.ProductVariantId = serial.ProductVariantId;
                existingSerial.SerialNumber = serial.SerialNumber;
                existingSerial.Status = serial.Status;

                _context.Serials.Update(existingSerial);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var serial = await _context.Serials.FindAsync(id);
            if (serial != null)
            {
                _context.Serials.Remove(serial);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Serial>> GetByProductVariantIdAsync(int productVariantId)
        {
            return await _context.Serials
                .Include(s => s.ProductVariant)
                .Where(s => s.ProductVariantId == productVariantId)
                .ToListAsync();
        }
    }
}
