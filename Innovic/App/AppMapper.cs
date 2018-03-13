using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.App
{
    public class AppMapper
    {
        private MapperConfiguration _config;

        public AppMapper()
        {

        }

        public AppMapper(MapperConfiguration config)
        {
            _config = config;
        }

        public Destination Convert<Source, Destination>(Source source)
        {
            if (_config == null)
            {
                _config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<Source, Destination>();
                });
            }

            var mapper = _config.CreateMapper();

            return mapper.Map<Source, Destination>(source);
        }
    }
}