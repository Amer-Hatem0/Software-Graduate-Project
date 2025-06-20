using AutoMapper;
using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateProject_Core.Mappings
{
 

    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<CreateMessageDTO, Message>();
            CreateMap<Message, MessageDTO>();
        }
    }

}
