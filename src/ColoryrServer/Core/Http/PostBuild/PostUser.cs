using ColoryrServer.Core.FileSystem;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.Core.Http.PostBuild;

internal static class PostUser
{
    public static UserList GetAll()
    {
        UserList list = new()
        {
            List = new()
        };
        foreach (var item in LoginSave.GetAllUser())
        {
            list.List.Add(new()
            {
                User = item.user,
                Time = item.time.ToString()
            });
        }
        return list;
    }

    public static ReMessage Add(BuildOBJ obj)
    {
        string user = obj.Code;
        string password = obj.Text.ToLower();
        var res = LoginSave.AddUser(user.ToLower(), password);
        if (!res)
        {
            return new()
            {
                Build = false,
                Message = "添加用户失败"
            };
        }
        return new()
        {
            Build = true,
            Message = "添加用户完成"
        };
    }

    public static ReMessage Remove(BuildOBJ obj)
    {
        string user = obj.Code;
        var res = LoginSave.Remove(user.ToLower());
        if (!res)
        {
            return new()
            {
                Build = false,
                Message = "删除用户失败"
            };
        }
        return new()
        {
            Build = true,
            Message = "删除用户完成"
        };
    }
}
