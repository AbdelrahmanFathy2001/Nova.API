namespace Talabat.API.Errors
{
    public class ApiEceptionReponse:ApiResponse
    {
        public string Details { get; set; }
        public ApiEceptionReponse(int statusCode,string message = null , string details = null):base(statusCode,message)
        {
            Details = details;
        }

    }
}
