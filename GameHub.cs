using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Task6.Models;

namespace Task6.Hubs
{
    public class GameHub : Hub
    {
        public static ConcurrentDictionary<string, GameModel> availableGames = new ConcurrentDictionary<string, GameModel>();
        public static ConcurrentDictionary<string, PlayerModel> players = new ConcurrentDictionary<string, PlayerModel>();
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> l)
        {
            _logger = l;
        }

        public string GetConnectionId() => Context.ConnectionId;

        public async Task SendMove(string s_cell)
        {
            int cell = Convert.ToInt16(s_cell);

            PlayerModel player;
            if ( players.TryGetValue(Context.ConnectionId, out player) )
            {
                GameModel game;
                if( availableGames.TryGetValue(player.GameId, out game) )
                {
                    if(game.Board[cell] != String.Empty)
                        return;

                    bool updateGame = false;
                    if(game.player1turn == true)
                    {
                        if(game.player1 == player)
                        {
                            updateGame = true;
                            game.player1turn = false;
                            game.Board[cell] = "X";
                        }
                    }
                    else
                    {
                        if(game.player2 == player)
                        {
                            updateGame = true;
                            game.player1turn = true;
                            game.Board[cell] = "O";
                        }
                    }

                    if (updateGame)
                    {
                        await UpdateGame(game.Id);
                        var w = game.GameWon();
                        if(w.Key == true)
                        {
                            await this.Clients.Client(game.player1.Id).SendAsync("GameWon", "Player " + w.Value.Id + " won the game!");
                            await this.Clients.Client(game.player2.Id).SendAsync("GameWon", "Player " + w.Value.Id + " won the game!");
                            game.watchers.ForEach(async (watcher) =>
                            {
                                await this.Clients.Client(watcher.Id).SendAsync("GameWon", "Player " + w.Value.Id + " won the game!");
                            });
                        }
                    }
                }
            }
        }

        private async Task<bool> UpdateGame(string gameId)
        {
            GameModel game;
            if (availableGames.TryGetValue(gameId, out game) == false)
                return false;

            await this.Clients.Client(game.player1.Id).SendAsync("GameUpdate", game.Board);
            if(game.player2 != null)
                await this.Clients.Client(game.player2.Id).SendAsync("GameUpdate", game.Board);

            game.watchers.ForEach(async (watcher) =>
            {
                await this.Clients.Client(watcher.Id).SendAsync("GameUpdate", game.Board);
            });

            return true;
        }
        public void NewGame(string gameName)
        {
            string id = Context.ConnectionId;
            players.TryAdd(id, new PlayerModel(id));


            GameModel newGame = new GameModel(players[id]);
            newGame.Name = gameName;
            if(availableGames.TryAdd(newGame.Id, newGame))
            {
                players[id].GameId = id;
            }
        }

        public void ConnectToGame(string gameId)
        {
            GameModel targetGame;
            if( availableGames.TryGetValue(gameId, out targetGame) )
            {
                string id = Context.ConnectionId;
                var x = players.TryAdd(id, new PlayerModel(id));

                players[id].GameId = gameId;

                if(targetGame.player2 == null)
                {
                    targetGame.player2 = players[id];
                }
                else
                {
                    targetGame.watchers.Add(players[id]);
                }

                this.Clients.Caller.SendAsync("GameUpdate", targetGame.Board);
            }
            else
            {
                this.Clients.Caller.SendAsync("Redirect", "Home", "Index", "Game was not found");
            }
        }

        public string[] GetAllGames()
        {
            var arr = Array.ConvertAll(availableGames.ToArray(),
                new Converter<KeyValuePair<string,GameModel>, string>((item) => 
                    item.Value.Name + "," + item.Value.Id + "," + Convert.ToString(
                                                                item.Value.watchers.Count
                                                                + Convert.ToInt16(item.Value.player1 != null)
                                                                + Convert.ToInt16(item.Value.player2 != null))));

            return arr;
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            PlayerModel disconnectedPlayer;
            if( players.TryGetValue(Context.ConnectionId, out disconnectedPlayer) )
            {
                GameModel game;
                if( availableGames.TryGetValue(disconnectedPlayer.GameId, out game) )
                {
                    if(game.player1 == disconnectedPlayer || game.player2 == disconnectedPlayer)
                    {
                        await this.Clients.Client(game.player1.Id).SendAsync("Redirect", "Home", "Index", "One of players has disconnected");
                        PlayerModel p;
                        players.TryRemove(game.player1.Id, out p);

                        if(game.player2 != null)
                        {
                            await this.Clients.Client(game.player2.Id).SendAsync("Redirect", "Home", "Index", "One of players has disconnected");
                            players.TryRemove(game.player2.Id, out p);
                        }

                        game.watchers.ForEach(async (watcher) =>
                        {
                            await this.Clients.Client(watcher.Id).SendAsync("Redirect", "Home", "Index", "One of players has disconnected");
                            PlayerModel t;
                            players.TryRemove(watcher.Id, out t);
                        });
                        GameModel g;
                        availableGames.TryRemove(game.Id, out g);
                    }
                    else
                    {
                        game.watchers.Remove(disconnectedPlayer);
                    }
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
