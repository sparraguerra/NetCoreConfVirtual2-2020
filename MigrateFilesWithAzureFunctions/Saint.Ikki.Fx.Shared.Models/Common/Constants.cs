namespace Saint.Ikki.Fx.Shared.Models.Common
{
    public static class Constants
    {
        public static class AppContext
        {
            public static string UserId => "userId";
        }

        public static class ClaimTypes
        {
            public static string Aud => "aud";
            public static string AppId => "appid";
            public static string UniqueName => "unique_name";
            public static string Upn => "upn";
            public static string PreferredUserName => "preferred_username";
        }

        public static class OrchestatorHttpCalls
        {
            public static string Migrate => "Suborchestrator_Test";
            public static string ClearDocs => "Suborchestrator_UpdateFile";
        }
    }
}
