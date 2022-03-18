namespace Api.Rtt.Models.Seeds
{
    public class DataSeed
    {
        private ApiContext _context;
        
        public DataSeed(ApiContext apiContext)
        {
            _context = apiContext;
        }

        public void SeedData()
        {
            _context.Database.EnsureCreated();
        }
    }
}