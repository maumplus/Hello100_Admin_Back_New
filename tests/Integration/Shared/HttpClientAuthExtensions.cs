namespace Hello100Admin.Integration.Shared
{
    public static class HttpClientAuthExtensions
    {
        // "S" => "SuperAdmin",
        // "C" => "HospitalAdmin",
        // "A" => "GeneralAdmin",
        // _ => "GeneralAdmin"

        public static HttpClient AsHospitalAdmin(this HttpClient client, string userId = "TEST1234", string name = "병원테스트")
        {
            client.DefaultRequestHeaders.Remove(TestAuthHandler.HeaderName);
            client.DefaultRequestHeaders.Add(TestAuthHandler.HeaderName, $"sub={userId};name={name};role=HospitalAdmin");
            return client;
        }

        public static HttpClient AsSuperAdmin(this HttpClient client, string userId = "TEST5678", string name = "슈퍼테스트")
        {
            client.DefaultRequestHeaders.Remove(TestAuthHandler.HeaderName);
            client.DefaultRequestHeaders.Add(TestAuthHandler.HeaderName, $"sub={userId};name={name};role=SuperAdmin");
            return client;
        }

        public static HttpClient AsAnonymous(this HttpClient client)
        {
            client.DefaultRequestHeaders.Remove(TestAuthHandler.HeaderName);
            return client;
        }
    }
}
