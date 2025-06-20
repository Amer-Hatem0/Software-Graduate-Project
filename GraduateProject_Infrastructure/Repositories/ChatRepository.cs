using AutoMapper;
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChatRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> DoctorHasUnreadMessagesAsync(int doctorId)
        {
            // Get the corresponding UserID for the doctor
            var user = await _context.Doctors
                .Where(d => d.DoctorID == doctorId)
                .Select(d => d.UserId)
                .FirstOrDefaultAsync();

            if (user == 0)
                return false;

            // Check if there are any unread messages sent to this doctor (as user)
            return await _context.Messages
                .AnyAsync(m => m.ReceiverUserID == user && !m.IsRead);
        }


        public async Task<Dictionary<int, int>> GetUnreadCountsGroupedBySenderAsync(int doctorId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverUserID == doctorId && !m.IsRead)
                .GroupBy(m => m.SenderUserID)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<bool> MarkAllMessagesFromSenderAsReadAsync(int senderId, int receiverId)
        {
            var messages = await _context.Messages
                .Where(m => m.SenderUserID == senderId && m.ReceiverUserID == receiverId && !m.IsRead)
                .ToListAsync();

            if (!messages.Any())
                return false;

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                Console.WriteLine($"Message {messageId} not found.");
                return false;
            }

            message.IsRead = true;
            _context.Entry(message).Property(m => m.IsRead).IsModified = true;

            Console.WriteLine($"Before save: MessageID = {message.MessageID}, IsRead = {message.IsRead}");

            await _context.SaveChangesAsync();

            var reloaded = await _context.Messages.FindAsync(messageId);
            Console.WriteLine($"After save: MessageID = {reloaded.MessageID}, IsRead = {reloaded.IsRead}");

            return true;
        }



        public async Task<IEnumerable<MessageDTO>> GetConversationAsync(int user1Id, int user2Id)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderUserID == user1Id && m.ReceiverUserID == user2Id) ||
                            (m.SenderUserID == user2Id && m.ReceiverUserID == user1Id))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task<MessageDTO> SendMessageAsync(CreateMessageDTO messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);
            message.SentAt = DateTime.UtcNow;
            message.IsRead = false;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return _mapper.Map<MessageDTO>(message);
        }
    }

}
