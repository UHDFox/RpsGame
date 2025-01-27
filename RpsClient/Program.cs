using Game;
using Grpc.Net.Client;
using Grpc.Core;

namespace RpsClient
{
    class Program
    {
        private static string _loggedInUserId = "";

        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7069"); // Ensure this matches your server's address
            var client = new GameService.GameServiceClient(channel);

            while (true)
            {
                await Login(); // Login step before showing options

                while (true)
                {
                    Console.WriteLine("=======================================");
                    Console.WriteLine("          ROCK-PAPER-SCISSORS GAME     ");
                    Console.WriteLine("=======================================");
                    Console.WriteLine("1. Play Game");
                    Console.WriteLine("2. Create Match");
                    Console.WriteLine("3. Get Balance");
                    Console.WriteLine("4. Get Match History");
                    Console.WriteLine("5. Get All Matches"); // New option
                    Console.WriteLine("6. Switch Account");
                    Console.WriteLine("7. Exit");
                    Console.WriteLine("=======================================");
                    Console.Write("Choose an option: ");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await PlayGame(client);
                            break;
                        case "2":
                            await CreateMatch(client);
                            break;
                        case "3":
                            await GetBalance(client);
                            break;
                        case "4":
                            await GetMatchHistory(client);
                            break;
                        case "5":
                            await GetMatchesWithBet(client); // Call the new method
                            break;
                        case "6":
                            Console.Clear();
                            await SwitchAccount();
                            break;
                        case "7":
                            Console.WriteLine("Exiting the application...");
                            return; // Exit the loop and the program
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
            }
        }

        private static Task Login()
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("   Welcome to the Rock-Paper-Scissors Game! ");
            Console.WriteLine("=======================================");

            // If user is not logged in, prompt for login
            if (string.IsNullOrEmpty(_loggedInUserId))
            {
                while (true)
                {
                    Console.Write("Please enter your User ID to log in: ");
                    var userId = Console.ReadLine();

                    if (!string.IsNullOrEmpty(userId))
                    {
                        _loggedInUserId = userId; // Save the logged-in user's ID
                        Console.WriteLine($"Successfully logged in as User ID: {_loggedInUserId}\n");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("User ID cannot be empty. Please try again.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Currently logged in as User ID: {_loggedInUserId}\n");
            }

            return Task.CompletedTask;
        }

        private static async Task SwitchAccount()
        {
            _loggedInUserId = "";
            await Login();
        }
        
        private static async Task PlayGame(GameService.GameServiceClient client)
        {
            Console.Write("Enter match ID: ");
            var matchId = Console.ReadLine();

            Console.Write("Enter your move ('К', 'Н', 'Б'): ");
            var playerMove = Console.ReadLine();

            var playGameRequest = new JoinMatchRequest()
            {
                MatchId = matchId,
                OpponentId = _loggedInUserId,
                PlayerMove = playerMove
            };

            try
            {
                var gameResult = await client.JoinMatchAsync(playGameRequest);

                Console.Clear();
                Console.WriteLine($"Match ID: {gameResult.MatchId}\nStatus: {gameResult.Status}");
                if (!string.IsNullOrEmpty(gameResult.Winner))
                    Console.WriteLine($"Winner: {gameResult.Winner}");
                else
                    Console.WriteLine("No winner yet.");
            }
            catch (RpcException ex)
            {
                Console.Clear();
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static async Task CreateMatch(GameService.GameServiceClient client)
        {
            Console.Write("Enter bet amount: ");
            var betAmount = decimal.Parse(Console.ReadLine());

            Console.Write("Enter host's move ('Rock', 'Scissors', 'Paper'): ");
            var hostMove = Console.ReadLine();

            var createMatchRequest = new CreateMatchRequest
            {
                HostId = _loggedInUserId, // Use logged-in user ID
                Bet = (double)betAmount,
                HostMove = hostMove
            };

            try
            {
                var response = await client.CreateMatchAsync(createMatchRequest);
                Console.Clear();
                Console.WriteLine($"Match created with ID: {response.MatchId}");
            }
            catch (RpcException ex)
            {
                Console.Clear();
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private static async Task GetMatchesWithBet(GameService.GameServiceClient client)
        {
            try
            {
                var response = await client.GetMatchesWithBetAsync(new GetMatchesWithBetRequest());

                Console.Clear();
                Console.WriteLine("Matches Information:");
                foreach (var match in response.MatchStatusInfo)
                {
                    Console.WriteLine($"Match ID: {match.MatchId}");
                    Console.WriteLine($"Bet: {match.Bet}");
                    Console.WriteLine($"Waiting for Player: {(match.IsWaitingForPlayer ? "Yes" : "No")}");
                    Console.WriteLine("------------------------------------");
                }
            }
            catch (RpcException ex)
            {
                Console.Clear();
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static async Task GetBalance(GameService.GameServiceClient client)
        {
            var getBalanceRequest = new GetBalanceRequest
            {
                UserId = _loggedInUserId // Use logged-in user ID
            };

            try
            {
                var response = await client.GetBalanceAsync(getBalanceRequest);
                Console.Clear();
                Console.WriteLine($"Current balance: {response.Balance}");
            }
            catch (RpcException ex)
            {
                Console.Clear();
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static async Task GetMatchHistory(GameService.GameServiceClient client)
        {
            var getMatchHistoryRequest = new GetMatchHistoryRequest
            {
                UserId = _loggedInUserId // Use logged-in user ID
            };

            try
            {
                var response = await client.GetMatchHistoryAsync(getMatchHistoryRequest);
                Console.Clear();
                Console.WriteLine("Match History:");
                foreach (var match in response.MatchHistory)
                {
                    Console.WriteLine($"Match ID: {match.MatchId}");
                    Console.WriteLine($"Bet: {match.Bet}");
                    Console.WriteLine($"Winner: {match.Winner}");
                    Console.WriteLine($"Start Time: {match.StartTime}");
                    Console.WriteLine("------------------------------------");
                }
            }
            catch (RpcException ex)
            {
                Console.Clear();
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
