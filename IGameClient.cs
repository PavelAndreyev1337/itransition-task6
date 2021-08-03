using System.Threading.Tasks;

namespace Task6.Hubs
{
    public interface IGameClient
    {
        Task GameUpdate(string[] board);

        Task GameWon(string message);

        Task Redirect(string controller, string action, string message);
    }
}