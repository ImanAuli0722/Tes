namespace Tomori.TIAS.Security.WS.Models.DTO
{
    public class Request<T>
    {
        public Paging Paging = new Paging();
        public T PropertyData;
    }
}