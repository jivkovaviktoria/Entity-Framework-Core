﻿using Trucks.Data.Models;
using Trucks.DataProcessor.ImportDto;

namespace Trucks
{
    using AutoMapper;

    public class TrucksProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
        public TrucksProfile()
        {
            this.CreateMap<ClientDto, Client>();
        }
    }
}
