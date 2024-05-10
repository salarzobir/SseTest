namespace SseTest.Services
{
    public class UserService
    {
        private static readonly User _user = new();

        public UserService(ISseService sseService)
        {
            _user.PropertyChanged += async (sender, e) =>
            {
                await sseService.SendEventAsync(["testClientId"], _user.Name);
            };
        }

        public string? Get()
        {
            return _user.Name;
        }

        public void Put(string newName)
        {
            _user.Name = newName;
        }
    }
}
