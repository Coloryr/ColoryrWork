using ColoryrServer.Core.Database;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.Core.BuilderPost;

internal static class PostUser
{
    public static UserList GetAll()
    {
        UserList list = new()
        {
            List = new()
        };
        foreach (var item in LoginDatabase.GetAllUser())
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
        var res = LoginDatabase.AddUser(user.ToLower(), password);
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
        var res = LoginDatabase.Remove(user.ToLower());
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
