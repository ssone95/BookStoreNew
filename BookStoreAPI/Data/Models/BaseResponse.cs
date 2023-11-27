namespace BookStoreAPI.Data.Models
{
	public class BaseResponse<T>
	{
		public int StatusCode { get; set; }
		public string ErrorMessage { get; set; }

		public T Result { get; set; }

		public int Page { get; set; } = 1;
		public int TotalPages { get; set; } = 1;
	}
}
