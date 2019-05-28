namespace PerfectCode.TvDbRestApi
{
    public interface IRestApiTVShow
    {
        string BackdropPath { get; set; }
        int Id { get; set; }
        string OriginalName { get; set; }
        string FirstAirDate { get; set; }
        string PosterPath { get; set; }
        double Popularity { get; set; }
        string Name { get; set; }
        double VoteAverage { get; set; }
        int VoteCount { get; set; }
        string Overview { get; set; }
    }
}