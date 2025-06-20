using GraduateProject_Core.DTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Interfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<MessageDTO>> GetConversationAsync(int user1Id, int user2Id);
        Task<MessageDTO> SendMessageAsync(CreateMessageDTO messageDto);
        Task<bool> DoctorHasUnreadMessagesAsync(int doctorId);
        Task<bool> MarkMessageAsReadAsync(int messageId);
        Task<bool> MarkAllMessagesFromSenderAsReadAsync(int senderId, int receiverId);
        Task<Dictionary<int, int>> GetUnreadCountsGroupedBySenderAsync(int doctorId);
    }

}
