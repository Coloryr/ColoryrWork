using System;
using System.Collections.Generic;
using System.Text;

namespace ColoryrWork.Lib.Debug.Object;

public record DatabaseObj
{
    public string database;
    public int id;
    public int type;
    public int read;

    public long qid;

    public string sql;
    public object arg;
}

public record RedisObj
{ 
    
}

public record DatabaseResObj
{
    public bool ok;
    public string message;
    public IEnumerable<dynamic> res;
    public int res1;

    public long qid;
}

public record RedisResObj
{

}