using System.Collections.Generic;

namespace PerfectCode.TvDbRestApi
{
    public interface IRestApiSeason
    {
        int Id { get; set; }
        int Number { get; set; }
        string AirDate { get; set; }
        List<IRestApiEpisode> Episodes { get; }
    }
}