namespace PerfectCode.TvDbRestApi
{
    public interface IRestApiEpisode
    {
        int Id { get; set; }
        int Number { get; set; }
        string Name { get; set; }
        string AirDate { get; set; }
        string Overview { get; set; }
    }
}