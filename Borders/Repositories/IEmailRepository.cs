namespace Borders.Repositories
{
    public interface IEmailRepository
    {
        public void Post(string subject, string html);
    }
}
