using Game;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace RpsClient
{
    class Program
    {
        private static string loggedInUserId = "";

        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7069"); // Ensure this matches your server's address
            var client = new GameService.GameServiceClient(channel);

            await Login(); // Login step before showing options

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Play Game");
                Console.WriteLine("2. Create Match");
                Console.WriteLine("3. Get Balance");
                Console.WriteLine("4. Get Match History");
                Console.WriteLine("5. Exit");
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
                        Console.WriteLine("Exiting the application...");
                        return; // Exit the loop and the program
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(); // Wait for user input to continue
            }
        }

        private static async Task Login()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Rock-Paper-Scissors Game!");

            while (string.IsNullOrEmpty(loggedInUserId))
            {
                Console.WriteLine("Please enter your User ID to log in:");
                var userId = Console.ReadLine();

                if (!string.IsNullOrEmpty(userId))
                {
                    loggedInUserId = userId; // Save the logged-in user's ID
                    Console.WriteLine($"Successfully logged in as User ID: {loggedInUserId}");
                }
                else
                {
                    Console.WriteLine("User ID cannot be empty. Please try again.");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task PlayGame(GameService.GameServiceClient client)
        {
            Console.WriteLine("Enter match ID: ");
            var matchId = Console.ReadLine();

            Console.WriteLine("Enter your move: ('Камень', 'Ножницы', 'Бумага')");
            var playerMove = Console.ReadLine();

            var playGameRequest = new JoinMatchRequest()
            {
                MatchId = matchId,
                OpponentId = loggedInUserId, // Use logged-in user ID
                PlayerMove = playerMove
            };

           try
            {
                var response = client.JoinMatch(playGameRequest);
                Console.WriteLine($"Match ID: {response.MatchId}");
                Console.WriteLine($"Winner: {response.Winner}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }
        }

        private static async Task CreateMatch(GameService.GameServiceClient client)
        {
            Console.WriteLine("Enter bet amount: ");
            var betAmount = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter host's move ('Камень', 'Ножницы', 'Бумага'):");
            var hostMove = Console.ReadLine();

            var createMatchRequest = new CreateMatchRequest
            {
                HostId = loggedInUserId, // Use logged-in user ID
                Bet = (double)betAmount,
                HostMove = hostMove
            };

            try
            {
                var response = await client.CreateMatchAsync(createMatchRequest);
                Console.WriteLine($"Match created with ID: {response.MatchId}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }
        }

        private static async Task GetBalance(GameService.GameServiceClient client)
        {
            var getBalanceRequest = new GetBalanceRequest
            {
                UserId = loggedInUserId // Use logged-in user ID
            };

            try
            {
                var response = await client.GetBalanceAsync(getBalanceRequest);
                Console.WriteLine($"Current balance: {response.Balance}");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }
        }

        private static async Task GetMatchHistory(GameService.GameServiceClient client)
        {
            var getMatchHistoryRequest = new GetMatchHistoryRequest
            {
                UserId = loggedInUserId // Use logged-in user ID
            };

            try
            {
                var response = await client.GetMatchHistoryAsync(getMatchHistoryRequest);
                Console.WriteLine("Match History:");
                foreach (var match in response.MatchHistory)
                {
                    Console.WriteLine($"Match ID: {match.MatchId}");
                    Console.WriteLine($"Bet: {match.Bet}");
                    Console.WriteLine($"Winner: {match.Winner}");
                    Console.WriteLine($"Start Time: {match.StartTime}");
                    Console.WriteLine();
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Error: {ex.Status.Detail}");
            }
        }
    }
}
