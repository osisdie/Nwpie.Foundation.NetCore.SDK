using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Logging.Enums
{
    public enum LoggingTypeEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "request")]
        Request,

        [Display(Name = "request.exception")]
        RequestException,

        [Display(Name = "response")]
        Response,

        [Display(Name = "response.exception")]
        ResponseException,

        [Display(Name = "foundation.api.request")]
        ApiRequest,

        [Display(Name = "foundation.api.response")]
        ApiResponse,

        [Display(Name = "foundation.api.exception")]
        ApiException,

        [Display(Name = "foundation.dal.init")]
        DALInitialization,

        [Display(Name = "foundation.dal.request")]
        DALRequest,

        [Display(Name = "foundation.dal.exception")]
        DALException,

        [Display(Name = "foundation.db.query.fail")]
        DbQueryFail,

        [Display(Name = "foundation.db.insert.fail")]
        DbInsertFail,

        [Display(Name = "foundation.db.update.fail")]
        DbUpdateFail,

        [Display(Name = "foundation.db.delete.fail")]
        DbDeleteFail,

        [Display(Name = "foundation.cache.add")]
        CacheAdd,

        [Display(Name = "foundation.cache.remove")]
        CacheRemove,

        [Display(Name = "foundation.cache.hit")]
        CacheHit,

        [Display(Name = "foundation.cache.miss")]
        CacheMiss,

        [Display(Name = "foundation.cache.exception")]
        CacheException,

        [Display(Name = "event.login.ok")]
        LoginSuccess,

        [Display(Name = "event.logout.ok")]
        LogoutSuccess,

        [Display(Name = "event.login.fail")]
        LoginFail,

        [Display(Name = "event.status.change")]
        StatusChange,

        [Display(Name = "event.meta.duplicate")]
        MetaDuplicated,

        [Display(Name = "ms")]
        MillisecondsDuration,
    }
}
