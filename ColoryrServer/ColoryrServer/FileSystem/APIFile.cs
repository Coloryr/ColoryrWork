using Lib.Build.Object;

namespace ColoryrServer.FileSystem
{
    internal class APIFile
    {
        public static APIFileObj list;
        public static void Start()
        {
            list = new APIFileObj();
            list.list.Add("api", ColoryrServerResource.api);
            list.list.Add("api1", ColoryrServerResource.api1);
            list.list.Add("api2", ColoryrServerResource.api2);
            list.list.Add("api3", ColoryrServerResource.api3);
            list.list.Add("api4", ColoryrServerResource.api4);
            list.list.Add("api5", ColoryrServerResource.api5);
            list.list.Add("api6", ColoryrServerResource.api6);
            list.list.Add("api7", ColoryrServerResource.api7);
            list.list.Add("api8", ColoryrServerResource.api8);
        }
    }
}
