﻿using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CompanyEmployees.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegistrationDto, User>();

        }
    }
}
