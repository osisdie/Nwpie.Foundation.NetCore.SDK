using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Measurement;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Services
{
    public abstract class ApiServiceAnyInOutEntry<
        T_Request,
        T_Response,
        T_DomainService> : HttpRequestServiceBase
        //where T_Response : IServiceResponse, new() // ContractResponseBase<T_Response_DataModel>, new()
        where T_DomainService : class, IDomainService
    {
        protected void Initialize(T_Request request)
        {
            ContractRequestDto = request;
            DomainServiceParamModel = request;
            try
            {
                DomainService = ResolveService();

                var responseDataType = typeof(T_Response);
                if (responseDataType.IsValueTyped() &&
                    false == responseDataType.IsNullable())
                {
                    ContractResponseDto = default(T_Response);
                }
                else
                {
                    ContractResponseDto = (T_Response)Activator.CreateInstance(typeof(T_Response));
                }
            }
            catch { }

            // Default: success
            DefaultFoundationResponseStatus();
            OnInit();
        }

        public virtual T_DomainService ResolveService()
        {
            return ComponentMgr.Instance.TryResolve<T_DomainService>();
            //  return (T_DomainService)ComponentMgr.Instance.DIContainer
            //      .Resolve(
            //          typeof(T_DomainService),
            //          new TypedParameter(
            //              typeof(IRequestService),
            //              this
            //          )
            //      );
        }

        /// <summary>
        /// ServiceStack contract and its service entry point
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<T_Response> Any(T_Request request)
        {
            ApiServiceEntryExtension
                .WriteApiHit(Request?.AbsoluteUri?.InferBaseUrl(Request?.RawUrl), Request?.RawUrl);

            try
            {
                Initialize(request);
                OnBegin();
                Validate(request);

                await ProcessLogic();
                ProcessResponse();

                _ = LogResult(Request, ContractResponseDto);
            }
            catch (Exception ex)
            {
                OnServiceException(ex);
                _ = LogErrorResult(Request, ContractResponseDto, ex);
                ApiServiceEntryExtension
                    .WriteApiException(Request?.AbsoluteUri?.InferBaseUrl(Request?.RawUrl), Request?.RawUrl, _ts, ex);
            }

            OnResponseProcessEnd(ContractResponseDto);
            OnEnd(HttpStatusCode.OK == MetaStatusCode);
            return ContractResponseDto;
        }

        #region Flow function
        // step 1
        protected void Validate(T_Request request)
        {
            IsValidRequest = false;
            OnValidationProcessBegin(request);
            try
            {
                IsValidRequest = ValidateUtils.ValidateAndThrow(request)?.IsSuccess ?? false;
                OnValidationProcessEnd(IsValidRequest);
            }
            catch (Exception ex)
            {
                //ContractResponseDto?.Error(StatusCodeEnum.InvalidContractRequest,
                //    Utility.ErrMsgDependOnEnv(ex));

                OnValidationProcessEnd(IsValidRequest, ex.Message);
                throw;
            }
        }

        // step 2
        protected async Task ProcessLogic()
        {
            if (false == IsValidRequest)
            {
                return;
            }

            OnLogicProcessBegin(DomainServiceParamModel);

            await OnDomainValidation(DomainServiceParamModel);
            await OnDomainExecuting(DomainServiceParamModel);

            OnLogicProcessEnd(ContractResponseDto);
        }

        // step 3
        protected void ProcessResponse()
        {
            OnResponseProcessBegin(ContractResponseDto);
            DefaultFoundationResponseBody();
            ProcessFoundationResponse();
        }

        protected void ProcessFoundationResponse()
        {
            if (null != ContractResponseDto)
            {
                DefaultResponseBodyWithData();
                return;
            }

            DefaultResponseBodyWithoutData();
        }

        protected void DefaultFoundationResponseStatus()
        {
            MetaStatusCode = HttpStatusCode.OK;
        }

        protected void DefaultFoundationResponseBody()
        {
            OnResponseBodyProcessBegin(ContractResponseDto);

            OnResponseBodyProcessEnd(ContractResponseDto);
        }

        protected void DefaultResponseBodyWithData()
        {
            OnResponseBodyWithDataProcessBegin(ContractResponseDto);

            OnResponseBodyWithDataProcessEnd(ContractResponseDto);
        }

        protected void DefaultResponseBodyWithoutData()
        {
            OnResponseBodyWithoutDataProcessBegin(ContractResponseDto);

            OnResponseBodyWithoutDataProcessEnd(ContractResponseDto);
        }

        protected void DefaultExceptionResponseBody(Exception ex)
        {
            if (HttpStatusCode.OK == MetaStatusCode)
            {
                MetaStatusCode = HttpStatusCode.InternalServerError;
            }

            base.Response.StatusCode = (int)MetaStatusCode;
        }
        #endregion

        #region Event function
        public event OnInitHandler InitEvent;
        public virtual void OnInit() => InitEvent?.Invoke();

        public event OnBeginHandler BeginEvent;
        public virtual void OnBegin() => BeginEvent?.Invoke();

        public event OnEndHandler EndEvent;
        public virtual void OnEnd(bool isSuccess) => EndEvent?.Invoke(isSuccess);

        public event OnLogicProcessBeginHandler LogicProcessBeginEvent;
        public virtual void OnLogicProcessBegin(T_Request param) =>
            LogicProcessBeginEvent?.Invoke(param);

        public virtual async Task OnDomainValidation(T_Request param)
        {
            IsValidRequest = false;
            try
            {
                if (null == param)
                {
                    throw new ArgumentNullException(typeof(T_Request).Name);
                }

                if (null == DomainService)
                {
                    throw new ArgumentNullException(typeof(T_DomainService).Name);
                }

                MethodInfo method = null;
                var funcName = DefaultAsyncValidationFuncName;
                method = DomainService.GetType().GetMethod(funcName,
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new Type[] { param.GetType() },
                    null
                );

                if (null == method)
                {
                    return;
                }

                //method = method.MakeGenericMethod(param.GetType());
                var isAwaitable = AsyncHelper.IsAwaitableMethod(method);
                if (isAwaitable)
                {
                    IsValidRequest = (bool)(await method.InvokeAsync(DomainService, param));
                }
                else
                {
                    IsValidRequest = (bool)(method.Invoke(DomainService, new[] { param as object }));
                }

                if (false == IsValidRequest)
                {
                    MetaStatusCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                if (string.IsNullOrWhiteSpace(DomainService.SubCode))
                {
                    DomainService.SubCode = StatusCodeEnum.InvalidContractRequest.ToString();
                }

                if (string.IsNullOrWhiteSpace(DomainService.SubMsg))
                {
                    DomainService.SubMsg = Utility.ErrMsgDependOnEnv(ex);
                }

                MetaStatusCode = HttpStatusCode.BadRequest;
                throw;
            }
        }

        public virtual async Task OnDomainExecuting(T_Request param)
        {
            if (false == IsValidRequest)
            {
                return;
            }

            try
            {
                if (null == param)
                {
                    throw new ArgumentNullException(typeof(T_Request).Name);
                }

                if (null == DomainService)
                {
                    throw new ArgumentNullException(typeof(T_DomainService).Name);
                }

                MethodInfo method = null;
                var funcName = DefaultAsyncExecuteFuncName;
                method = DomainService.GetType().GetMethod(funcName,
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new Type[] { param.GetType() },
                    null
                );

                if (null == method)
                {
                    return;
                }

                object executedResult;
                var isAwaitable = AsyncHelper.IsAwaitableMethod(method);
                if (isAwaitable)
                {
                    executedResult = await method.InvokeAsync(DomainService, param);
                }
                else
                {
                    executedResult = method.Invoke(DomainService, new[] { param as object });
                }

                if (executedResult is T_Response resultDto)
                {
                    //ContractResponseDto = (T_Response)Convert.ChangeType(executedResult, typeof(T_Response));
                    ContractResponseDto = resultDto;
                }
            }
            catch
            {
                if (true != DomainService?.IsValidParam)
                {
                    MetaStatusCode = HttpStatusCode.BadRequest;
                }
                else
                {
                    MetaStatusCode = HttpStatusCode.InternalServerError;
                }

                throw;
            }
        }

        public event OnLogicProcessEndHandler LogicProcessEndEvent;
        public virtual void OnLogicProcessEnd(T_Response result) =>
            LogicProcessEndEvent?.Invoke(result);

        public event OnResponseProcessBeginHandler ResponseProcessBeginEvent;
        public virtual void OnResponseProcessBegin(T_Response response) =>
            ResponseProcessBeginEvent?.Invoke(response);

        public event OnResponseProcessEndHandler ResponseProcessEndEvent;
        public virtual void OnResponseProcessEnd(T_Response response) =>
            ResponseProcessEndEvent?.Invoke(response);

        public event OnResponseBodyProcessEndHandler ResponseBodyProcessEndEvent;
        public virtual void OnResponseBodyProcessEnd(T_Response response) =>
            ResponseBodyProcessEndEvent?.Invoke(response);

        public event OnResponseBodyProcessBeginHandler ResponseBodyProcessBeginEvent;
        public virtual void OnResponseBodyProcessBegin(T_Response response) =>
            ResponseBodyProcessBeginEvent?.Invoke(response);

        public event OnResponseBodyWithDataProcessEndHandler ResponseBodyWithDataProcessEndEvent;
        public virtual void OnResponseBodyWithDataProcessEnd(T_Response response) =>
            ResponseBodyWithDataProcessEndEvent?.Invoke(response);

        public event OnResponseBodyWithDataProcessBeginHandler ResponseBodyWithDataProcessBeginEvent;
        public virtual void OnResponseBodyWithDataProcessBegin(T_Response response) =>
            ResponseBodyWithDataProcessBeginEvent?.Invoke(response);

        public event OnResponseBodyWithoutDataProcessEndHandler ResponseBodyWithoutDataProcessEndEvent;
        public virtual void OnResponseBodyWithoutDataProcessEnd(T_Response response) =>
            ResponseBodyWithoutDataProcessEndEvent?.Invoke(response);

        public event OnResponseBodyWithoutDataProcessBeginHandler ResponseBodyWithoutDataProcessBeginEvent;
        public virtual void OnResponseBodyWithoutDataProcessBegin(T_Response response) =>
            ResponseBodyWithoutDataProcessBeginEvent?.Invoke(response);

        public event OnValidationProcessBeginHandler ValidationProcessBeginEvent;
        public virtual void OnValidationProcessBegin(T_Request request) =>
            ValidationProcessBeginEvent?.Invoke(request);

        public event OnValidationProcessEndHandler ValidationProcessEndEvent;
        public virtual void OnValidationProcessEnd(bool isValid, string errMsg = null)
        {
            if (false == isValid)
            {
                MetaStatusCode = HttpStatusCode.BadRequest;
                ApiServiceEntryExtension
                    .WriteApiBadRequest(Request?.AbsoluteUri?.InferBaseUrl(Request?.RawUrl), Request?.RawUrl, _ts);
            }

            ValidationProcessEndEvent?.Invoke(isValid);
        }

        public event OnServiceExceptionHandler ServiceExceptionEvent;
        public virtual void OnServiceException(Exception ex)
        {
            DefaultExceptionResponseBody(ex);
            ServiceExceptionEvent?.Invoke(ex);
        }
        #endregion

        public T_Request ContractRequestDto { get; private set; }
        public T_Response ContractResponseDto { get; set; }
        public T_DomainService DomainService { get; set; }
        public T_Request DomainServiceParamModel { get; private set; }
        public bool IsValidRequest { get; set; } = false;
        public bool ThrowIfInvalid { get; set; } = true;
        public HttpStatusCode MetaStatusCode { get; set; } = HttpStatusCode.OK;
        public string DefaultAsyncValidationFuncName { get; set; } = ServiceNodeConst.DomainSvcValidationFunName;
        public string DefaultAsyncExecuteFuncName { get; set; } = ServiceNodeConst.DomainSvcExecuteFuncName;

        public delegate void OnInitHandler();
        public delegate void OnBeginHandler();
        public delegate void OnEndHandler(bool isSuccess);
        public delegate void OnLogicProcessBeginHandler(T_Request param);
        public delegate void OnLogicProcessEndHandler(T_Response result);
        public delegate void OnResponseProcessBeginHandler(T_Response response);
        public delegate void OnResponseProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyProcessBeginHandler(T_Response response);
        public delegate void OnResponseBodyWithDataProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyWithDataProcessBeginHandler(T_Response response);
        public delegate void OnResponseBodyWithoutDataProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyWithoutDataProcessBeginHandler(T_Response response);
        public delegate void OnValidationProcessBeginHandler(T_Request request);
        public delegate void OnValidationProcessEndHandler(bool isValid);
        public delegate void OnServiceExceptionHandler(Exception ex);
    }
}
