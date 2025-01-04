using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Measurement;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Services
{
    public abstract class ApiServiceEntry<
        T_Request,
        T_Response,
        T_Request_DataModel,
        T_Response_DataModel,
        T_DomainService_ParamModel,
        T_DomainService> : HttpRequestServiceBase
        where T_Request : class, IRequestMessage<T_Request_DataModel> // ContractRequestBase<T_Request_DataModel>
        where T_Response : IServiceResponse<T_Response_DataModel>, new() // ContractResponseBase<T_Response_DataModel>, new()
        where T_DomainService : class, IDomainService
        where T_Request_DataModel : class, IRequestDto // RequestDtoBase
        //where T_Response_DataModel : class, IResultDto, new() // ResultDtoBase
        where T_DomainService_ParamModel : class, IRequestDto // RequestDtoBase
    {
        protected void Initialize(T_Request request)
        {
            ContractRequestDto = request;

            try
            {
                DomainService = ResolveService();

                var responseDataType = typeof(T_Response_DataModel);
                if (responseDataType.IsValueTyped() &&
                    false == responseDataType.IsNullable())
                {
                    ContractResponseDto = new T_Response()
                    {
                        Data = default(T_Response_DataModel)
                    };
                }
                else
                {
                    ContractResponseDto = new T_Response()
                    {
                        // default: NULL or default
                        Data = default(T_Response_DataModel)
                        //Data = (T_Response_DataModel)Activator.CreateInstance(typeof(T_Response_DataModel))
                    };
                }

                var requestDataType = ContractRequestDto?.Data?.GetType();
                if (null != requestDataType &&
                    requestDataType.IsValueTyped() &&
                    false == requestDataType.IsNullable())
                {
                    DomainServiceParamModel = (T_DomainService_ParamModel)Convert
                        .ChangeType(ContractRequestDto.Data, typeof(T_DomainService_ParamModel));
                }
                else if (null != requestDataType)
                {
                    DomainServiceParamModel = ContractRequestDto.Data
                        .ConvertTo<T_DomainService_ParamModel>();
                }
            }
            catch { }

            // Default: success
            DefaultFoundationResponseStatus();
            OnInit();
        }

        public virtual T_DomainService ResolveService() =>
            ComponentMgr.Instance.TryResolve<T_DomainService>();

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
            OnEnd(ContractResponseDto?.IsSuccess ?? false);
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
                IsValidRequest = request.ValidateAndThrow();
                OnValidationProcessEnd(IsValidRequest);
            }
            catch (Exception ex)
            {
                ContractResponseDto?.Error(StatusCodeEnum.InvalidContractRequest,
                    Utility.ErrMsgDependOnEnv(ex));

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

            OnLogicProcessEnd(DomainData);
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
            if (null != DomainData)
            {
                DefaultResponseBodyWithData();
                return;
            }

            DefaultResponseBodyWithoutData();
        }

        protected void DefaultFoundationResponseStatus()
        {
            ContractResponseDto.Success();
        }

        protected void DefaultFoundationResponseBody()
        {
            OnResponseBodyProcessBegin(ContractResponseDto);
            if (null != ContractResponseDto)
            {
                ContractResponseDto.SubCode = DomainService?.SubCode;
                ContractResponseDto.SubMsg = DomainService?.SubMsg;
            }
            OnResponseBodyProcessEnd(ContractResponseDto);
        }

        protected void DefaultResponseBodyWithData()
        {
            OnResponseBodyWithDataProcessBegin(ContractResponseDto, DomainData);
            if (null != ContractResponseDto)
            {
                ContractResponseDto.Data = DomainData;
            }
            OnResponseBodyWithDataProcessEnd(ContractResponseDto, DomainData);
        }

        protected void DefaultResponseBodyWithoutData()
        {
            OnResponseBodyWithoutDataProcessBegin(ContractResponseDto);
            if (true == ContractResponseDto?.IsSuccess)
            {
                ContractResponseDto.Success(StatusCodeEnum.EmptyData);
            }

            OnResponseBodyWithoutDataProcessEnd(ContractResponseDto);
        }

        protected void DefaultExceptionResponseBody(Exception ex)
        {
            if (null != ContractResponseDto)
            {
                ContractResponseDto.SubCode = DomainService?.SubCode;
                ContractResponseDto.SubMsg = DomainService?.SubMsg;
                ContractResponseDto.Error(StatusCodeEnum.Exception,
                    Utility.ErrMsgDependOnEnv(ex));
            }
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
        public virtual void OnLogicProcessBegin(T_DomainService_ParamModel param) =>
            LogicProcessBeginEvent?.Invoke(param);

        public virtual async Task OnDomainValidation(T_DomainService_ParamModel param)
        {
            IsValidRequest = false;
            try
            {
                if (null == param)
                {
                    throw new ArgumentNullException(typeof(T_DomainService_ParamModel).Name);
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
                    ContractResponseDto.Error(StatusCodeEnum.InvalidContractRequest,
                        DomainService.SubMsg);
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
                    DomainService.SubMsg = ex.Message;
                }

                ContractResponseDto.Error(StatusCodeEnum.InvalidContractRequest,
                    Utility.ErrMsgDependOnEnv(ex));

                throw;
            }

            //CanContinue(lastEx);
        }

        public virtual async Task OnDomainExecuting(T_DomainService_ParamModel param)
        {
            if (false == IsValidRequest)
            {
                return;
            }

            try
            {
                if (null == param)
                {
                    throw new ArgumentNullException(typeof(T_DomainService_ParamModel).Name);
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

                if (executedResult is T_Response_DataModel resultDto)
                {
                    //DomainData = (T_Response_DataModel)Convert.ChangeType(executedResult, typeof(T_Response_DataModel));
                    DomainData = resultDto;
                }
            }
            catch (Exception ex)
            {
                if (true != DomainService?.IsValidParam)
                {
                    ContractResponseDto.Error(StatusCodeEnum.InvalidContractRequest, Utility.ErrMsgDependOnEnv(ex));
                }
                else
                {
                    ContractResponseDto.Error(StatusCodeEnum.Exception, Utility.ErrMsgDependOnEnv(ex));
                }

                throw;
            }

            //CanContinue(lastEx);
        }

        public event OnLogicProcessEndHandler LogicProcessEndEvent;
        public virtual void OnLogicProcessEnd(T_Response_DataModel result) =>
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
        public virtual void OnResponseBodyWithDataProcessEnd(T_Response response, T_Response_DataModel result) =>
            ResponseBodyWithDataProcessEndEvent?.Invoke(response, result);

        public event OnResponseBodyWithDataProcessBeginHandler ResponseBodyWithDataProcessBeginEvent;
        public virtual void OnResponseBodyWithDataProcessBegin(T_Response response, T_Response_DataModel result) =>
            ResponseBodyWithDataProcessBeginEvent?.Invoke(response, result);

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
        public T_Response_DataModel DomainData { get; set; }
        public T_DomainService_ParamModel DomainServiceParamModel { get; private set; }
        public bool IsValidRequest { get; set; } = false;
        public bool ThrowIfInvalid { get; set; } = true;
        public HttpStatusCode MetaStatusCode { get; set; } = HttpStatusCode.OK;
        public string DefaultAsyncValidationFuncName { get; set; } = ServiceNodeConst.DomainSvcValidationFunName;
        public string DefaultAsyncExecuteFuncName { get; set; } = ServiceNodeConst.DomainSvcExecuteFuncName;

        public delegate void OnInitHandler();
        public delegate void OnBeginHandler();
        public delegate void OnEndHandler(bool isSuccess);
        public delegate void OnLogicProcessBeginHandler(T_DomainService_ParamModel param);
        public delegate void OnLogicProcessEndHandler(T_Response_DataModel result);
        public delegate void OnResponseProcessBeginHandler(T_Response response);
        public delegate void OnResponseProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyProcessBeginHandler(T_Response response);
        public delegate void OnResponseBodyWithDataProcessEndHandler(T_Response response, T_Response_DataModel result);
        public delegate void OnResponseBodyWithDataProcessBeginHandler(T_Response response, T_Response_DataModel result);
        public delegate void OnResponseBodyWithoutDataProcessEndHandler(T_Response response);
        public delegate void OnResponseBodyWithoutDataProcessBeginHandler(T_Response response);
        public delegate void OnValidationProcessBeginHandler(T_Request request);
        public delegate void OnValidationProcessEndHandler(bool isValid);
        public delegate void OnServiceExceptionHandler(Exception ex);
    }
}
