using System;

namespace MusicStore.Spa.Infrastructure
{
    public class Mapper
    {
        public static TDest Map<TSource, TDest>(TSource source, TDest dest)
        {
#if ASPNET50
            return AutoMapper.Mapper.Map(source, dest);
#else
            return SimpleMapper.Map(source,dest);
#endif
        }
    }
}