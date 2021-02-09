using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Entities.Enums;
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
        /// <summary>
        /// -> Register new User for Buyer 
        /// -> Register User Admin
        /// -> User Login         
        /// -> Search All Team
        /// -> Update Team        
        /// -> Search All Player                
        /// -> Add one Player to Market List
        /// -> Delete Player
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterAdmin_Until_DeletePlayer()
        {
            var client = _factory.CreateClient();
            var buyerUser = await Post_Register_User(client);
            var addAnotherTeam = await Post_Register_User(client);
            var adminUser = await Post_Register_User(client, RoleEnum.Admin);
            var adminLogin = await Post_Login_User(client, adminUser.Key);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + adminLogin.Value["data"]);

            var searchAllTeam = await Post_Search_All_Team(client);
            var team = (searchAllTeam.Value["data"] as JArray)[0];
            var teamWillBuyPlayer = (searchAllTeam.Value["data"] as JArray)[1];
            var updateTeam = await Post_Update_Team(client, team);
            var updateBudget = await Post_Update_Team(client, teamWillBuyPlayer);
            var searchAllPlayer = await Post_Search_All_Player(client);

            var playerWillBeDelete = (searchAllPlayer.Value["data"] as JArray)[0];
            var playerWillBeUpdate = (searchAllPlayer.Value["data"] as JArray)[1];
            var playerWillAddToMarket = (searchAllPlayer.Value["data"] as JArray).FirstOrDefault(n => n["teamId"].ToString() != teamWillBuyPlayer["id"].ToString());
            var playerWillAddToMarketAgain = (searchAllPlayer.Value["data"] as JArray)[3];
            var updatePlayer = await Post_Update_Player(client, playerWillBeUpdate);


            var playerForSale = await Post_Add_Player_To_Market(client, Convert.ToInt32(playerWillAddToMarket["id"].ToString()), 1500000);
            var playerForSaleAgain = await Post_Add_Player_To_Market(client, Convert.ToInt32(playerWillAddToMarketAgain["id"].ToString()), 1500000);
            var transferId = Convert.ToInt32(playerForSale.Value["data"].ToString());
            var adminBuyPlayerForAnotherTeam = await Post_Buy_Player_Async(client, transferId, Convert.ToInt32(teamWillBuyPlayer["id"]));
            var deletedPlayer = await Post_Delete_Player(client, playerWillBeDelete);

        }

        private async Task<KeyValuePair<PlayerReqeust, JObject>> Post_Update_Player(HttpClient client, JToken playerWillBeUpdate)
        {
            // Arrange
            var newAge = 67;
            var filter = new PlayerReqeust()//Get all Team
            {
                Id = Convert.ToInt32(playerWillBeUpdate["id"].ToString()),
                Country = playerWillBeUpdate["country"].ToString(),
                Age = newAge,
                FirstName = playerWillBeUpdate["firstName"].ToString(),
                LastName = playerWillBeUpdate["lastName"].ToString(),
                Position = (PositionEnum)Enum.Parse(typeof(PositionEnum), playerWillBeUpdate["position"].ToString()),
                TeamId = Convert.ToInt32(playerWillBeUpdate["teamId"].ToString()),
                Value = Convert.ToInt32(playerWillBeUpdate["value"].ToString())

            };
            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PutAsync(Constants.URL_POST_UPDATE_PLAYER, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(Convert.ToInt32(result["data"]["age"].ToString()) == newAge);
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<PlayerReqeust, JObject>(filter, result);
        }

        private async Task<KeyValuePair<PlayerReqeust, JObject>> Post_Delete_Player(HttpClient client, JToken playerWillBeUpdate)
        {
            // Arrange
            var newAge = 67;
            var filter = new PlayerReqeust()//Get all Team
            {
                Id = Convert.ToInt32(playerWillBeUpdate["id"].ToString()),
                Country = playerWillBeUpdate["country"].ToString(),
                Age = newAge,
                FirstName = playerWillBeUpdate["firstName"].ToString(),
                LastName = playerWillBeUpdate["lastName"].ToString(),
                Position = (PositionEnum)Enum.Parse(typeof(PositionEnum), playerWillBeUpdate["position"].ToString()),
                TeamId = Convert.ToInt32(playerWillBeUpdate["teamId"].ToString()),
                Value = Convert.ToInt32(playerWillBeUpdate["value"].ToString())

            };
            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            // Act
            var response = await client.DeleteAsync($"{Constants.URL_POST_DELETE_PLAYER}?id={filter.Id}");
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(Convert.ToInt32(result["data"]["age"].ToString()) == 0);
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<PlayerReqeust, JObject>(filter, result);
        }

        private async Task<KeyValuePair<SearchPlayerRequest, JObject>> Post_Search_All_Player(HttpClient client)
        {
            // Arrange

            var filter = new SearchPlayerRequest()//Get all Team
            {

            };
            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_POST_GET_ALL_PLAYER, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True((result["data"] as JArray).Count() > 0);
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<SearchPlayerRequest, JObject>(filter, result);
        }

        private async Task<KeyValuePair<TeamReqeust, JObject>> Post_Update_Team(HttpClient client, JToken team)
        {
            // Arrange
            var newName = "Dream Team";
            var filter = new TeamReqeust()//Get all Team
            {
                TeamName = newName,
                Budget = 500000000,
                Id = Convert.ToInt32(team["id"].ToString()),
                Country = team["country"].ToString(),
            };
            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PutAsync(Constants.URL_POST_UPDATE_TEAM, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True(result["data"]["teamName"].ToString() == newName);
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<TeamReqeust, JObject>(filter, result);
        }

        private async Task<KeyValuePair<SearchTeamRequest, JObject>> Post_Search_All_Team(HttpClient client)
        {
            // Arrange

            var filter = new SearchTeamRequest()//Get all Team
            {

            };
            var content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(Constants.URL_POST_GET_ALL_TEAM, content);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
            Assert.True(Convert.ToBoolean(result["success"].ToString()));
            Assert.True((result["data"] as JArray).Count() > 0);
            Assert.Null(result["message"].Value<string>());
            return new KeyValuePair<SearchTeamRequest, JObject>(filter, result);
        }
    }

}
