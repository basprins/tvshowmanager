using System.Collections.Generic;

namespace PerfectCode.TvDbRestApi
{
    public interface IRestApiImagesConfiguration
    {
        string BaseUrl { get; set; }
        List<string> BackdropSizes { get; set; }
        List<string> LogoSizes { get; set; }
        List<string> PosterSizes { get; set; }
    }
}