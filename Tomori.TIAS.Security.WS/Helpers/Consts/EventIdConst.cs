using Tomori.TIAS.Security.WS.Models.DTO;

namespace Tomori.TIAS.Security.WS.Helpers.Consts
{
    public static class EventIdConst
    {
        public static Status OK = new Status { CodeStatus = 200, MessageStatus = "OK" };
        public static Status NOT_MODIFED = new Status { CodeStatus = 304, MessageStatus = "Not Modifed" };
        public static Status BAD_REQUEST = new Status { CodeStatus = 400, MessageStatus = "Bad Request" };
        public static Status UNAUTHORIZE = new Status { CodeStatus = 401, MessageStatus = "Unauthorize" };
        public static Status NOT_FOUND = new Status { CodeStatus = 404, MessageStatus = "Not Found" };
        public static Status REQUEST_TIMEOUT = new Status { CodeStatus = 408, MessageStatus = "Request Timeout" };
        public static Status REQUEST_TO_LARGE = new Status { CodeStatus = 413, MessageStatus = "Request To Large" };
        public static Status UNSUPPORT_MEDIA_TYPE = new Status { CodeStatus = 415, MessageStatus = "Unsupport Media Type" };
        public static Status INTERNAL_SERVER_ERROR = new Status { CodeStatus = 500, MessageStatus = "Internal Server Error" };
        public static Status SERVICE_UNAVAILABLE = new Status { CodeStatus = 503, MessageStatus = "Service Unavailable" };
    }
}