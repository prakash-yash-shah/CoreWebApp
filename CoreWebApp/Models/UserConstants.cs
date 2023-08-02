namespace JsonWebTokens.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new()
            {
                    new UserModel(){ Username="Yash",Password="Yash123",Role="Admin",Email="yash@gmail.com",OfficeName="Psspl",IsActive=true}
            };
    }
}
