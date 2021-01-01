using Lib.Build.Object;

namespace ColoryrServer.FileSystem
{
    internal class APIFile
    {
        public static APIFileObj list;
        public APIFile()
        {
            list = new APIFileObj();
            list.list.Add("api", ColorResource.api);
            list.list.Add("api1", ColorResource.api1);
            list.list.Add("api2", ColorResource.api2);
            list.list.Add("api3", ColorResource.api3);
            list.list.Add("api4", ColorResource.api4);
            list.list.Add("api5", ColorResource.api5);
            list.list.Add("api6", ColorResource.api6);
            list.list.Add("api7", ColorResource.api7);
            list.list.Add("api8", ColorResource.api8);
        }
    }
}
