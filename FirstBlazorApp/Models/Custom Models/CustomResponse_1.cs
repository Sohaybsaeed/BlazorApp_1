namespace FirstBlazorApp.Models.Custom_Models
{
    public class CustomResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
