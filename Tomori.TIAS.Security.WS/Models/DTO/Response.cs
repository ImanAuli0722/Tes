using System;
using Tomori.TIAS.Security.WS.Helpers.Consts;

namespace Tomori.TIAS.Security.WS.Models.DTO
{
    public class Response<T>
    {
        public Status Status = EventIdConst.BAD_REQUEST;

        public Data<T> Data = new Data<T>();
        public DateTime DateTimeResult = DateTime.Now;
    }

    public class Status
    {
        public int CodeStatus;
        public string MessageStatus;
    }

    public class Data<T>
    {
        public Paging Paging = new Paging();
        public T ContentData;
    }
}