namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts
{
    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResponseDtoBase : IResponseDto
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public string ErrMsg { get; set; }
    }

    public class ResponseDto<T> : ResponseDtoBase, IResponseDto<T>
    {
        public T Data { get; set; }
    }

    public interface IResponseDto<T> : IResponseDto
    {
        T Data { get; set; }
    }

    public interface IResponseDto
    {
        int Code { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        string Msg { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        string ErrMsg { get; set; }

        /// <summary>
        /// The most important Successful flag after function, procedure, service call
        /// </summary>
        bool IsSuccess { get; set; }
    }
}
