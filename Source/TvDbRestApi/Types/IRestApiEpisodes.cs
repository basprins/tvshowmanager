using System.Collections.Generic;

namespace PerfectCode.TvDbRestApi
{
    public interface IRestApiEpisodes
    {
        int Id { get; set; }
        List<IRestApiEpisode> Episodes { get; set; }
    }
}