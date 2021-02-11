using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DreamSoccerApi.E2E
{
    public partial class FlowBuyPlayerTeamOwnerTest : IClassFixture<WebApiTesterFactory>
    {
        const int TOTAL_PLAYER = 20;
        private readonly WebApiTesterFactory _factory;

        public FlowBuyPlayerTeamOwnerTest(WebApiTesterFactory factory)
        {
            _factory = factory;
        }
        /// <summary>
        /// -> Register User 
        /// -> User Login 
        /// -> Get My Players 
        /// -> Update Team
        /// -> Update Team
        /// -> Update Player
        /// -> Update Player
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUser_Login_UpdateTeam_UpdatePlayer_UpdatePlayer()
        {
            var client = _factory.CreateClient();

            var user = await Post_Register_User(client);
            var userLogin = await Post_Login_User(client, user.Key);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userLogin.Value["data"]);

            var teamPlayer = await Get_My_Team_Players(client);
            await Post_Update_Team(client, teamPlayer.Value["data"], false);
            await Post_Update_Team(client, teamPlayer.Value["data"], false);
            var playerWillBeUpdate = (teamPlayer.Value["data"]["players"] as JArray)[0];
            await Post_Update_Player(client, playerWillBeUpdate, Convert.ToInt32(teamPlayer.Value["data"]["id"]));
            await Post_Update_Player(client, playerWillBeUpdate, Convert.ToInt32(teamPlayer.Value["data"]["id"]));

        }
        /// <summary>
        /// -> Register Admin 
        /// -> User Login 
        /// -> Get My Players 
        /// -> Update Team
        /// -> Update Team
        /// -> Update Player
        /// -> Update Player
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterAdmin_Login_UpdateTeam_UpdatePlayer_UpdatePlayer()
        {
            var client = _factory.CreateClient();

            var user = await Post_Register_User(client, RoleEnum.Admin);
            var userLogin = await Post_Login_User(client, user.Key);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userLogin.Value["data"]);

            var searchAllTeam = await Post_Search_All_Team(client);
            await Post_Update_Team(client, searchAllTeam.Value["data"][0], false);
            await Post_Update_Team(client, searchAllTeam.Value["data"][0], false);
            var playerWillBeUpdate = (searchAllTeam.Value["data"][0]["players"] as JArray)[0];
            await Post_Update_Player(client, playerWillBeUpdate, Convert.ToInt32(searchAllTeam.Value["data"][0]["id"]));
            await Post_Update_Player(client, playerWillBeUpdate, Convert.ToInt32(searchAllTeam.Value["data"][0]["id"]));

        }


        /// <summary>
        /// -> Register User 
        /// -> User Login 
        /// -> Get My Players 
        /// -> Add one Player to Market List
        /// -> Create new User for Buyer 
        /// -> Login for Buyer 
        /// -> Take My Players 
        /// -> Search Player in Market 
        /// -> Buy Player -> Check total team
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUser_Until_OtherUserBuyPlayer()
        {
            var client = _factory.CreateClient();
            
            var user = await Post_Register_User(client);
            var userLogin = await Post_Login_User(client, user.Key);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + userLogin.Value["data"]);

            var teamPlayer = await Get_My_Team_Players(client);
            var updateTeam = await Post_Update_Team(client, teamPlayer.Value["data"], false);
            var updateTeamMakeSureWeCanUpdateAgain = await Post_Update_Team(client, teamPlayer.Value["data"], false);
            var playerWillBeUpdate = (teamPlayer.Value["data"]["players"] as JArray)[0];
            var updatePlayer = await Post_Update_Player(client, playerWillBeUpdate, Convert.ToInt32(teamPlayer.Value["data"]["id"]));
            var updatePlayerMakeSureWeCanUpdateAgain = await Post_Update_Player(client, playerWillBeUpdate, Convert.ToInt32(teamPlayer.Value["data"]["id"]));
            var playerId = Convert.ToInt32((teamPlayer.Value["data"]["players"] as JArray)[0]["id"]);
            var playerForSale = await Post_Add_Player_To_Market(client, playerId, 1500000);


            var buyerUser = await Post_Register_User(client);
            var buyerUserLogin = await Post_Login_User(client, buyerUser.Key);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + buyerUserLogin.Value["data"]);
            var buyerTeamPlayer = await Get_My_Team_Players(client);
            var countPlayerBeforeBuy = (buyerTeamPlayer.Value["data"]["players"] as JArray).Count();
            var buyerSearchPlayer = await Get_Search_Player_For_SaleAsync(client);
            var trasnferId = Convert.ToInt32((buyerSearchPlayer.Value["data"] as JArray)[0]["id"].Value<string>());
            var buyerBuyPlayer = await Post_Buy_Player_Async(client, trasnferId);
            var buyerGetLatestTeamPlayer = await Get_My_Team_Players(client, TOTAL_PLAYER + 1);
            var countPlayerAfterBuy = (buyerGetLatestTeamPlayer.Value["data"]["players"] as JArray).Count();
            playerId = Convert.ToInt32(buyerSearchPlayer.Value["data"][0]["player"]["id"].ToString());
            var playerForSaleFromBuyer = await Post_Add_Player_To_Market(client, playerId, 2500000);
            Assert.Equal(playerForSaleFromBuyer.Key.PlayerId, playerId);
            Assert.Equal<int>(countPlayerAfterBuy, countPlayerBeforeBuy + 1);

        }

        private async Task<KeyValuePair<string, JObject>> Post_Buy_Player_Async(HttpClient client, int trasnferId, int targetTeam = -1)
        {
            // Arrange
            var createFakeUser = new BuyPlayerRequest()
            {
                TransferId = trasnferId,
                TeamId = targetTeam

            };
            var content = new StringContent(JsonConvert.SerializeObject(createFakeUser), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_POST_BUY_PLAYER_IN_MARKET, content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(!string.IsNullOrEmpty(result["data"].ToString()));
            return new KeyValuePair<string, JObject>("", result);
        }

        private async Task<KeyValuePair<string, JObject>> Get_Search_Player_For_SaleAsync(HttpClient client)
        {
            // Arrange
            var createFakeUser = new SearchPlayerRequest()
            {
                MinValue = 1000000
            };
            var content = new StringContent(JsonConvert.SerializeObject(createFakeUser), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_POST_SEARCH_PLAYER_IN_MARKET, content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True((result["data"] as JArray).Count > 0);
            return new KeyValuePair<string, JObject>("", result);
        }

        private async Task<KeyValuePair<AddTransferListRequest, JObject>> Post_Add_Player_To_Market(HttpClient client, int playerId, long price)
        {
            // Arrange
            var player = new AddTransferListRequest()
            {
                PlayerId = playerId,
                Price = price
            };
            var content = new StringContent(JsonConvert.SerializeObject(player), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_POST_ADD_PLAYER_TO_MARKET, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(Convert.ToInt32(result["data"].ToString()) > 0);
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<AddTransferListRequest, JObject>(player, result);
        }

        private async Task<KeyValuePair<string, JObject>> Get_My_Team_Players(HttpClient client, int expectedPalyers = 20)
        {
            // Arrange
            // Act
            var response = await client.GetAsync(Constants.URL_GET_MY_PLAYER);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True((result["data"]["players"] as JArray).Count == expectedPalyers);
            return new KeyValuePair<string, JObject>("", result);
        }

        private async Task<KeyValuePair<UserLoginDto, JObject>> Post_Login_User(HttpClient client, UserRegisterDto key)
        {
            // Arrange
            var createFakeUser = new UserLoginDto()
            {
                Email = key.Email,
                Password = key.Password
            };
            var content = new StringContent(JsonConvert.SerializeObject(createFakeUser), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_LOGIN, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(!String.IsNullOrEmpty(result["data"].ToString()));
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<UserLoginDto, JObject>(createFakeUser, result);
        }

        private static async Task<KeyValuePair<UserRegisterDto, JObject>> Post_Register_User(HttpClient client, RoleEnum role = RoleEnum.Team_Owner)
        {
            // Arrange
            var createFakeUser = FactoryCreator.CreateUser(role);
            var content = new StringContent(JsonConvert.SerializeObject(createFakeUser), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_REGISTRATION, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(Convert.ToInt32(result["data"]) > 0);
            return new KeyValuePair<UserRegisterDto, JObject>(createFakeUser, result);
        }
    }



}
