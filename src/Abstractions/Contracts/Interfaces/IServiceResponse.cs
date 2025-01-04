namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IServiceResponse
    {
        int Code { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        string Msg { get; set; }

        /// <summary>
        /// Message Id
        /// </summary>
        string MsgId { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        string ErrMsg { get; set; }

        /// <summary>
        /// The most important Successful flag after function, procedure, service call
        /// </summary>
        bool IsSuccess { get; set; }

        /// <summary>
        /// Internal status code
        /// </summary>
        string SubCode { get; set; }

        /// <summary>
        /// Internal message or error message
        /// </summary>
        string SubMsg { get; set; }
    }

    public interface IServiceResponse<T> : IServiceResponse
    {
        T Data { get; set; }
    }
}
