using System.Net;

namespace FinbourneCache.Domain.DataTransferObjects.Response;
public class BaseResponse
{
	public string Message { get; set; }
	public HttpStatusCode HttpStatusCode { get; set; }

	public bool IsHttpStatusCodeOk => HttpStatusCode == HttpStatusCode.OK;
}