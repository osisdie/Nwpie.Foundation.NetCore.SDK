using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Kind
{
    public enum AuthLogEventEnum
    {
        [Display(Name = "event.unknown")]
        UnSet = 0,

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

        [Display(Name = "event.db.query.fail")]
        DbQueryFail,

        [Display(Name = "event.db.insert.fail")]
        DbInsertFail,

        [Display(Name = "event.db.update.fail")]
        DbUpdateFail,

        [Display(Name = "event.db.delete.fail")]
        DbDeleteFail,

        [Display(Name = "event.cache.add")]
        Add,

        [Display(Name = "event.cache.remove")]
        Remove,

        [Display(Name = "event.cache.miss")]
        Miss,

        [Display(Name = "event.cache.hit")]
        Hit,
    }
}
