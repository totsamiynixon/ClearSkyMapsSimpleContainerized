namespace Web.Infrastructure
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }

        public FirebaseCloudMessagingAppSettings FirebaseCloudMessaging { get; set; }

        public string Verison { get; set; }
        
        //TODO: Check how to manage service ip 
        public string ServerUrl { get; set; }
    }

    public class FirebaseCloudMessagingAppSettings
    {
        public string ServerKey { get; set; }
        
        public string MessagingSenderId { get; set; }
    }
}