using System.ComponentModel;

namespace Infrastructure.Definitions;

public class Messages
{
    [DisplayName("Middlewares")]
    public static class Middlewares
    {
        public const string IPAddressForbidden = "Mes.Middlewares.IPAddress.Forbidden";
    }


    [DisplayName("BMIs")]
    public static class BMIs
    {
        public const string RequestInValid = "Mes.BMIs.Request.InValid";
        public const string PhoneInValid = "Mes.BMIs.Phone.InValid"; 
        public const string AddressOverLength = "Mes.BMIs.Address.OverLength"; 
        /// <summary>
        /// không có dữ liệu cho thông tin gửi lên
        /// </summary>
        public const string ResponseNotSuport = "Mes.BMIs.Response.NotSuport";
    }

    
     [DisplayName("ProductSuitables")]
    [Description("ProductSuitables Messages")]
    public static class ProductSuitables
    {
        #region Check ProductSuitables message
        /// <summary>
        /// không tìm thấy id kết quả BMI
        /// </summary>
        public const string IdNotFound = "Mes.ProductSuitables.Id.NotFound";
        /// <summary>
        /// Id Mapping không hợp lệ
        /// </summary>
        public const string IdMappingInValid = "Mes.ProductSuitables.IdMapping.InValid";
        /// <summary>
        /// id kết quả BMI đã tồn tại
        /// </summary>
        public const string IdAlreadyExist = "Mes.ProductSuitables.Id.AlreadyExist";
        #endregion
        #region ProductSuitables validation message
        /// <summary>
        /// id kết quả BMI không hợp lệ
        /// </summary>
        public const string IdRequired = "Mes.ProductSuitables.Id.Required";
        #endregion
        #region ProductSuitables action message
        /// <summary>
        /// Lấy thồng tin chi tiết kết quả BMI thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.ProductSuitables.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin kết quả BMI thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.ProductSuitables.GetAll.Successfully";
        /// <summary>
        /// tải hình thành công
        /// </summary>
        public const string UploadSuccessfully = "Mes.ProductSuitables.Upload.Successfully";
        /// <summary>
        /// tải hình thất bại
        /// </summary>
        public const string UploadFailed = "Mes.ProductSuitables.Upload.Failed";
        /// <summary>
        /// Cập nhập thông tin kết quả BMI thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.ProductSuitables.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin kết quả BMI thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.ProductSuitables.Update.Failed";
        /// <summary>
        /// Xóa thông tin kết quả BMI thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.ProductSuitables.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin kết quả BMI thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.ProductSuitables.Delete.Failed";
        #endregion
    }
 
    
    
    [DisplayName("Customers")]
    public static class Customers
    {
        #region Check Customers message
        /// <summary>
        /// Không tìm thấy số điện thoại khách hàng
        /// </summary>
        public const string PhoneNotfound = "Mes.Customers.Phone.Notfound";

        /// <summary>
        /// Email khách hàng đã tồn tại trong cơ sở dữ liệu
        /// </summary>
        public const string EmailAddressAlreadyExist = "Mes.Customers.EmailAddress.AlreadyExist";
        /// <summary>
        /// Email khách hàng đã tồn tại trong cơ sở dữ liệu
        /// </summary>
        public const string EmailAddressNotExist = "Mes.Customers.EmailAddress.NotExist";
        /// <summary>
        /// không tìm thấy id khách hàng
        /// </summary>
        public const string IdNotFound = "Mes.Customers.Id.NotFound";
        /// <summary>
        /// chưa đăng nhập
        /// </summary>
        public const string UnAuthorized = "Mes.Customers.UnAuthorized";
        /// <summary>
        /// không tìm thấy khách hàng
        /// </summary>
        public const string NotFound = "Mes.Customers.NotFound";
        /// <summary>
        /// khách hàng đã bị khóa
        /// </summary>
        public const string IsLocked = "Mes.Customers.IsLocked";

        /// <summary>
        /// Số điện thoại khách hàng đã tồn tại
        /// </summary>
        public const string PhoneAlreadyExist = "Mes.Customers.Phone.AlreadyExist";
        #endregion

        #region Customers validation message
        /// <summary>
        /// Email khách hàng không hợp lệ
        /// </summary>
        public const string EmailAddressInvalid = "Mes.Customers.EmailAddress.Invalid";
        /// <summary>
        /// Mật khẩu không để trống
        /// </summary>
        public const string PasswordNotEmpty = "Mes.Users.Password.NotEmpty";
        /// <summary>
        /// Mật khẩu không hợp lệ: trên 6 kí tự, it nhất 1 kí tự số, regex ^.*(?=.{6,})(?=.*\d)(?=.*[a-z]).*$
        /// </summary>
        public const string PasswordInValid = "Mes.Users.Password.InValid";
        /// <summary>
        /// mật khẩu mới không đc giông mật khẩu cũ
        /// </summary>
        public const string NewPasswordNotEqualOldPassword = "Mes.Users.NewPassword.NotEqualOldPassword";
        /// <summary>
        /// Mật khẩu sai
        /// </summary>
        public const string PasswordIsWrong = "Mes.Users.Password.IsWrong";
        /// <summary>
        /// Mật khẩu không vượt quá giới hạn độ dài
        /// </summary>
        public const string PasswordOverLength = "Mes.Users.Password.OverLength";
        /// <summary>
        /// mật khẩu không hợp lệ
        /// </summary>
        public const string PasswordNotMatch = "Mes.Customers.Password.NotMatch";
        /// <summary>
        /// Yêu cầu bắt buộc reset token
        /// </summary>
        public const string RecoveryTokenRequired = "Mes.Customers.RecoveryToken.Required";
        /// <summary>
        /// Yêu cầu bắt buộc reset token
        /// </summary>
        public const string RecoveryTokenInvalid = "Mes.Customers.RecoveryToken.Invalid";
        /// <summary>
        /// Yêu cầu bắt buộc reset token
        /// </summary>
        public const string RecoveryTokenPleaseCheckEmail = "Mes.Customers.RecoveryToken.PleaseCheckEmail";
        /// <summary>
        /// Yêu cầu bắt buộc reset token
        /// </summary>
        public const string RecoveryTokenInvalidOrExpired = "Mes.Customers.RecoveryToken.InvalidOrExpired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string NameIsRequired = "Mes.Customers.Name.IsRequired";
        /// <summary>
        /// Độ dài vượt quá giới hạn cho phép
        /// </summary>
        public const string NameIsOverLength = "Mes.Customers.Name.OverLength";
        /// <summary>
        /// Yêu cầu bắt buộc nhập số điện thoại
        /// </summary>
        public const string PhoneIsRequired = "Mes.Customers.Phone.IsRequired";

        /// <summary>
        /// Số điện thoại khách hàng nhập không hợp lệ: 
        /// tiền tố: +84,84,03,05,07,08,09
        /// đuôi gồm 8 chữ số
        /// regex ^(((\+|)84)|0)(3|5|7|8|9){1}+([0-9]{8})$
        /// </summary>
        public const string PhoneInValid = "Mes.Customers.Phone.InValid";
        #endregion

        #region Customers action message
        /// <summary>
        /// Lấy thồng tin chi tiết khách hàng thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Customers.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin khách hàng thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Customers.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin khách hàng thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Customers.Update.Successfully";
        /// <summary>
        /// gửi email thành công
        /// </summary>
        public const string SendEmailSuccessfully = "Mes.Customers.SendEmail.Successfully";
         /// <summary>
        /// gửi email thành công
        /// </summary>
        public const string SendEmailFailed = "Mes.Customers.SendEmail.Failed";
        /// <summary>
        /// Cập nhập thông tin khách hàng thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Customers.Update.Failed";
        /// <summary>
        /// Xóa thông tin khách hàng thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Customers.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin khách hàng thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Customers.Delete.Failed";
        /// <summary>
        /// Lấy thông tin cá nhân người dùng hiện tại thành công
        /// </summary>
        public const string GetProfileSuccessfully = "Mes.Customers.GetProfile.Successfully";
        /// <summary>
        /// cập nhập thông tin cá nhân người dùng hiện tại thành công
        /// </summary>
        public const string UpdateProfileSuccessfully = "Mes.Customers.UpdateProfile.Successfully";
        /// <summary>
        /// Đăng ký thành công
        /// </summary>
        public const string SignUpSuccesfully = "Mes.Customers.SignUp.Succesfully";
        /// <summary>
        /// Đăng nhập thành công
        /// </summary>
        public const string SignInSuccesfully = "Mes.Customers.SignUp.Succesfully";
        /// <summary>
        /// thời gian hết hạn
        /// </summary>
        public const string ResetPasswordTimeExpire = "Mes.Customers.ResetPassword.TimeExpire";
        /// <summary>
        /// thời gian không hợp lệ
        /// </summary>
        public const string ResetPasswordSuccesfully = "Mes.Customers.ResetPassword.Succesfully";
        /// <summary>
        /// kiểm tra thành công
        /// </summary>
        public const string CheckRecoveryTokenSuccesfully = "Mes.Customers.CheckRecoveryToken.Succesfully";
        #endregion
    }
    [DisplayName("Medias")]
    [Description("Medias Messages")]
    public static class Medias
    {
        #region Check Medias message
        #endregion
        #region Medias validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string FileInValid = "Mes.Medias.File.InValid";
        /// <summary>
        /// file không để trống
        /// </summary>
        public const string FileNotEmpty = "Mes.Medias.File.NotEmpty";
        #endregion
        #region Medias action message
        /// <summary>
        /// Lấy thồng tin chi tiết media thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Medias.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin media thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Medias.GetAll.Successfully";
        /// <summary>
        /// tải hình thành công
        /// </summary>
        public const string UploadSuccessfully = "Mes.Medias.Upload.Successfully";
        /// <summary>
        /// tải hình thất bại
        /// </summary>
        public const string UploadFailed = "Mes.Medias.Upload.Failed";
        /// <summary>
        /// Cập nhập thông tin media thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Medias.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin media thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Medias.Update.Failed";
        /// <summary>
        /// Xóa thông tin media thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Medias.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin media thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Medias.Delete.Failed";
        #endregion
    }
    [DisplayName("Histories")]
    [Description("Histories Messages")]
    public static class Histories
    {
        #region Check Histories message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.Histories.Id.NotFound";
        #endregion
        #region Histories validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập kết quả
        /// </summary>
        public const string ResultIsRequired = "Mes.Histories.Result.IsRequired";
        /// <summary>
        /// kết quả không hợp lệ
        /// </summary>
        public const string RelationInvalid = "Mes.Histories.Result.Invalid";

        #endregion
        #region Histories action message
        /// <summary>
        /// Lấy thồng tin chi tiết lịch sử thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Histories.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin lịch sử thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Histories.GetAll.Successfully";
        /// <summary>
        /// gửi lịch sử thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.Histories.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin lịch sử thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Histories.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin lịch sử thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Histories.Update.Failed";
        /// <summary>
        /// Xóa thông tin lịch sử thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Histories.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin lịch sử thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Histories.Delete.Failed";
        #endregion
    }

    [DisplayName("Banners")]
    [Description("Banners Messages")]
    public static class Banners
    {
        #region Check Banners message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.Banners.Id.NotFound";
        /// <summary>
        /// code là bắt buộc
        /// </summary>
        public const string BannerCodeRequired = "Mes.BannerMedias.BannerCode.Required";
        /// <summary>
        /// code là quá dài
        /// </summary>
        public const string BannerCodeOverLength = "Mes.BannerMedias.BannerCode.OverLength";
        #endregion
        #region Banners validation message
        /// <summary>
        /// độ ưu tiên là bắt buộc
        /// </summary>
        public const string PriorityRequired = "Mes.Banners.Priority.Required";
        /// <summary>
        /// vị trí là bắt buộc
        /// </summary>
        public const string PositionRequired = "Mes.Banners.Position.Required";
        /// <summary>
        /// file là bắt buộc
        /// </summary>
        public const string NameRequired = "Mes.Banners.Name.Required";
        /// <summary>
        /// file là bắt buộc
        /// </summary>
        public const string BackgroundRequired = "Mes.Banners.Background.Required";
        /// <summary>
        /// file không hợp lệ
        /// </summary>
        public const string BackgroundInValid = "Mes.Banners.Background.InValid";
        #endregion
        #region Banners action message
        /// <summary>
        /// Lấy thồng tin chi tiết banner thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Banners.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin banner thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Banners.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin banner thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Banners.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin banner thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Banners.Update.Failed";
        /// <summary>
        /// Cập nhập thông tin file thất bại
        /// </summary>
        public const string FileUploadFailed = "Mes.Banners.FileUpload.Failed";
        /// <summary>
        /// xóa thông tin file thất bại
        /// </summary>
        public const string FileDeleteFailed = "Mes.Banners.FileDelete.Failed";
        /// <summary>
        /// tạo thông tin banner thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.Banners.Create.Successfully";
        /// <summary>
        /// Xóa thông tin banner thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Banners.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin banner thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Banners.Delete.Failed";
        #endregion
    }
    
    [DisplayName("BannerMedias")]
    [Description("BannerMedias Messages")]
    public static class BannerMedias
    {
        #region Check BannerMedias message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.BannerMedias.Id.NotFound";
        #endregion
        #region BannerMedias validation message
        /// <summary>
        /// vị trí là bắt buộc
        /// </summary>
        public const string IdRequired = "Mes.BannerMedias.Id.Required";
        public const string Required = "Mes.BannerMedias.Required";
        public const string IndexDuplicate = "Mes.BannerMedias.Index.Duplicate";
        /// <summary>
        /// file là bắt buộc
        /// </summary>
        public const string NameRequired = "Mes.BannerMedias.Name.Required";
        /// <summary>
        /// vị trí là bắt buộc
        /// </summary>
        public const string IndexRequired = "Mes.BannerMedias.Index.Required";
        /// <summary>
        /// vị trí đã tồn tại
        /// </summary>
        public const string IndexAlreadyExist = "Mes.BannerMedias.Index.AlreadyExist";
        /// <summary>
        /// file là bắt buộc
        /// </summary>
        public const string FileRequired = "Mes.BannerMedias.Files.Required";
        /// <summary>
        /// file không hợp lệ
        /// </summary>
        public const string FileInValid = "Mes.BannerMedias.Files.InValid"; 
        #endregion
        #region BannerMedias action message
        /// <summary>
        /// Lấy thồng tin chi tiết bannerMedia thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.BannerMedias.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin bannerMedia thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.BannerMedias.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin bannerMedia thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.BannerMedias.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin bannerMedia thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.BannerMedias.Update.Failed";
        /// <summary>
        /// Cập nhập thông tin file thất bại
        /// </summary>
        public const string FileUploadFailed = "Mes.BannerMedias.FileUpload.Failed";
        /// <summary>
        /// xóa thông tin file thất bại
        /// </summary>
        public const string FileDeleteFailed = "Mes.BannerMedias.FileDelete.Failed";
        /// <summary>
        /// tạo thông tin bannerMedia thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.BannerMedias.Create.Successfully";
        /// <summary>
        /// Xóa thông tin bannerMedia thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.BannerMedias.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin bannerMedia thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.BannerMedias.Delete.Failed";
        #endregion
    }

    
    [DisplayName("Questions")]
    [Description("Questions Messages")]
    public static class Questions
    {
        #region Check Questions message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.Questions.Id.NotFound";
        #endregion
        #region Questions validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>AnswerIsRequired
        public const string FullNameIsRequired = "Mes.Questions.FullName.IsRequired";
        /// <summary>
        /// Độ dài vượt quá giới hạn cho phép (50)
        /// </summary>
        public const string FullNameIsOverLength = "Mes.Questions.FullName.OverLength";
        /// <summary>
        /// Yêu cầu bắt buộc nhập số điện thoại
        /// </summary>
        public const string PhoneIsRequired = "Mes.Questions.Phone.IsRequired";

        /// <summary>
        /// Số điện thoại khách hàng nhập không hợp lệ: 
        /// tiền tố: +84,84,03,05,07,08,09
        /// đuôi gồm 8 chữ số
        /// regex (((\+|)84)|0)(3|5|7|8|9)+([0-9]{8})\b
        /// </summary>
        public const string PhoneInValid = "Mes.Questions.Phone.InValid";
        /// <summary>
        /// trạng thái trả lời không hợp lệ
        /// </summary>
        public const string AnswerStatusInValid = "Mes.Questions.AnswerStatus.InValid";
        /// <summary>
        /// trạng thái trả lời không hợp lệ
        /// </summary>
        public const string ToMustBeGreaterOrEqualToFrom = "Mes.Questions.ToMustBeGreaterToFrom";
        /// <summary>
        /// email không đúng định dạng
        /// </summary>
        public const string EmailInValid = "Mes.Questions.Email.InValid";
        /// <summary>
        /// question type phải là enum
        /// </summary>
        public const string QuestionTypeInValid = "Mes.Questions.QuestionType.InValid";
        /// <summary>
        /// question type phải là enum
        /// </summary>
        public const string QuestionTypeIsRequired = "Mes.Questions.QuestionType.IsRequired";

        /// <summary>
        /// Yêu cầu bắt buộc nhập nội dung
        /// </summary>
        public const string ContentIsRequired = "Mes.Questions.Content.IsRequired";
        #endregion
        #region Questions action message
        /// <summary>
        /// Lấy thồng tin chi tiết câu hỏi thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Questions.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin câu hỏi thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Questions.GetAll.Successfully";
        /// <summary>
        /// gửi câu hỏi thành công
        /// </summary>
        public const string SendSuccessfully = "Mes.Questions.Send.Successfully";
        /// <summary>
        /// gửi câu hỏi thành công
        /// </summary>
        public const string AnwserSuccessfully = "Mes.Questions.Anwser.Successfully";
        /// <summary>
        /// Cập nhập thông tin câu hỏi thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Questions.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin câu hỏi thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Questions.Update.Failed";
        /// <summary>
        /// Xóa thông tin câu hỏi thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Questions.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin câu hỏi thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Questions.Delete.Failed";
        #endregion
    }
    [DisplayName("OTPs")]
    [Description("OTPs Messages")]
    public static class OTPs
    {

        #region Check OTPs message
        /// <summary>
        /// code không tồn tại
        /// </summary>
        public const string NotExist = "Mes.OTP.Code.NotExit";
        /// <summary>
        /// code đã được sử dụng
        /// </summary>
        public const string IsUsed = "Mes.OTP.Code.IsUsed";
        #endregion
        #region OTPs validation message
        /// <summary>
        /// Code không được để trống
        /// </summary>
        public const string CodeNotEmpty = "Mes.OTP.CodeNotEmpty";
        /// <summary>
        /// otp chưa được xác nhận
        /// </summary>
        public const string NotVerified = "Mes.Users.OTP.NotVerified";
        /// <summary>
        /// otp không hợp lệ
        /// </summary>
        public const string InValidCode = "Mes.OTP.Code.Invalid";
        /// <summary>
        /// otp hết hạn
        /// </summary>
        public const string ExpiredCode = "Mes.OTP.Code.Expired";
        /// <summary>
        /// action phải là enum
        /// </summary>
        public const string ActionIsEnum = "Mes.OTP.Action.IsEnum";
        /// <summary>
        /// nhập sai authentication action
        /// </summary>
        public const string AuthenFlagInValid = "Mes.OTP.AuthenFlag.InValid";
        #endregion
        #region OTPs action message
        /// <summary>
        /// Tạo otp thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.OTP.Create.Successfully";
        /// <summary>
        /// Tạo otp thất bại
        /// </summary>
        public const string CreateFailed = "Mes.OTP.Create.Failed";
        #endregion
    }
    [DisplayName("Users")]
    public static class Users
    {
        #region Check Users region
        /// <summary>
        /// Email người dùng đã tồn tại
        /// </summary>
        public const string EmailAddressAlreadyExist = "Mes.Users.EmailAddress.AlreadyExist";
        /// <summary>
        /// Email người dùng không hợp lệ
        /// </summary>
        public const string EmailAddressInvalid = "Mes.Users.EmailAddress.Invalid";
        /// <summary>
        /// Username đã tồn tại
        /// </summary>
        public const string UserNameAlreadyExist = "Mes.Users.UserName.AlreadyExist";
        /// <summary>
        /// Username chưa đã tồn tại
        /// </summary>
        public const string UserNameNotExist = "Mes.Users.UserName.DoesNotExist";
        /// <summary>
        /// Id không tìm được
        /// </summary>
        public const string IdNotFound = "Mes.Users.Id.NotFound";
        /// <summary>
        /// không tìm được
        /// </summary>
        public const string NotFound = "Mes.Users.NotFound";
        /// <summary>
        /// Không tìm thấy username
        /// </summary>
        public const string UserNameNotFound = "Mes.Users.UserName.NotFound";
        /// <summary>
        /// người dùng đã bị khóa
        /// </summary>
        public const string IsLocked = "Mes.Users.IsLocked";
        #endregion
        #region Users validation message
        /// <summary>
        /// Username vượt quá độ dài cho phép
        /// </summary>
        public const string UserNameOverLength = "Mes.Users.UserName.OverLength";
        /// <summary>
        /// Username không hợp lệ: không cho phép gõ kí tự tiếng việt, regex ^[a-zA-Z0-9._~!@#$%^&*()_+<>?|]+$
        /// </summary>
        public const string UserNameInvalid = "Mes.Users.UserName.Invalid";
        /// <summary>
        /// Username không để trống
        /// </summary>
        public const string UserNameNotEmpty = "Mes.Users.UserName.NotEmpty";
        /// <summary>
        /// FullName vượt quá độ dài cho phép
        /// </summary>
        public const string FullNameOverLength = "Mes.Users.FullName.OverLength";
        /// <summary>
        /// FullName không để trống
        /// </summary>
        public const string FullNameNotEmpty = "Mes.Users.FullName.NotEmpty";
        /// <summary>
        /// Id người dùng là bắt buộc
        /// </summary>
        public const string IdIsRequired = "Mes.Users.Id.IsRequired";
        /// <summary>
        /// Id người dùng là không hợp lệ
        /// </summary>
        public const string UserIdInValid = "Mes.Users.Id.InValid";
        /// <summary>
        /// Mật khẩu không để trống
        /// </summary>
        public const string PasswordNotEmpty = "Mes.Users.Password.NotEmpty";
        /// <summary>
        /// Reset code người dùng là không hợp lệ
        /// </summary>
        public const string ResetCodeNotValid = "Mes.Users.ResetCode.NotValid";
        /// <summary>
        /// Mật khẩu không hợp lệ: trên 6 kí tự, it nhất 1 kí tự số, regex ^.*(?=.{6,})(?=.*\d)(?=.*[a-z]).*$
        /// </summary>
        public const string PasswordInValid = "Mes.Users.Password.InValid";
        /// <summary>
        /// Mật khẩu sai
        /// </summary>
        public const string PasswordIsWrong = "Mes.Users.Password.IsWrong";
        /// <summary>
        /// Mật khẩu không vượt quá giới hạn độ dài
        /// </summary>
        public const string PasswordOverLength = "Mes.Users.Password.OverLength";
        /// <summary>
        /// Mật khẩu không khớp
        /// </summary>
        public const string PasswordConfirmNotMatch = "Mes.Users.PasswordConfirm.NotMatch";
        /// <summary>
        /// Avatar không đúng định dạng
        /// </summary>
        public const string AvatarInValid = "Mes.Users.Avatar.InValid";
        #endregion

        #region Users action message
        /// <summary>
        /// Lấy thồng tin chi tiết người dùng thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Users.GetDetail.Successfully";
        /// <summary>
        /// Lấy thồng tin người dùng thành công
        /// </summary>
        public const string GetProfileSuccessfully = "Mes.Users.GetProfile.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin người dùng thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Users.GetAll.Successfully";
        /// <summary>
        /// Tạo thông tin người dùng thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.Users.Create.Successfully";
        /// <summary>
        /// Tạo thông tin người dùng thất bại
        /// </summary>
        public const string CreateFailed = "Mes.Users.Create.Failed";
        /// <summary>
        /// Cập nhập thông tin người dùng thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Users.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin người dùng thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Users.Update.Failed";
        /// <summary>
        /// Xóa thông tin người dùng thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Users.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin người dùng thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Users.Delete.Failed";
        /// <summary>
        /// Gửi email cho người dùng thành công
        /// </summary>
        public const string SendEmailSuccessfully = "Mes.Roles.SendEmail.Successfully";
        /// <summary>
        /// đăng nhập thành công
        /// </summary>
        public const string SignInSuccess = "Mes.Users.SignIn.Success";
        /// <summary>
        /// Đặt lại mật khẩu thành công
        /// </summary>
        public const string ResetPasswordSuccesfully = "Mes.Users.ResetPassword.Succesfully";
        #endregion
    }

    [DisplayName("Roles")]
    public static class Roles
    {
        #region Check Roles region
        /// <summary>
        /// Id quyền không tìm được
        /// </summary>
        public const string IdNotFound = "Mes.Roles.Id.NotFound";
        /// <summary>
        /// da su dung
        /// </summary>
        public const string BeingUsed = "Mes.Roles.BeingUsed";
        #endregion
        #region Roles validation message
        /// <summary>
        /// Tên quyền không được để trống
        /// </summary>
        public const string NameEmpty = "Mes.Roles.Name.NameIsEmpty";
        /// <summary>
        /// Tên quyền không hợp lệ
        /// </summary>
        public const string NameInValid = "Mes.Roles.Name.InValid";
        /// <summary>
        /// Id quyền là bắt buộc
        /// </summary>
        public const string IdIsRequired = "Mes.Roles.Id.IsRequired";
        #endregion
        #region Roles Action message
        /// <summary>
        /// Lấy thồng tin chi tiết nhóm quyền thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Roles.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin nhóm quyền thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Roles.GetAll.Successfully";
        /// <summary>
        /// Tạo thông tin nhóm quyền thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.Roles.Create.Successfully";
        /// <summary>
        /// Tạo thông tin nhóm quyền thất bại
        /// </summary>
        public const string CreateFailed = "Mes.Roles.Create.Failed";
        /// <summary>
        /// Cập nhập thông tin nhóm quyền thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Roles.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin nhóm quyền thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Roles.Update.Failed";
        /// <summary>
        /// Xóa thông tin nhóm quyền thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Roles.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin nhóm quyền thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Roles.Delete.Failed";
        #endregion

    }

    [DisplayName("RolePermissions")]
    public static class RolePermissions
    {
        #region Check RolePermissions region
        // <summary>
        /// Id quyền cho phép không tìm được
        /// </summary>
        public const string IdNotFound = "Mes.RolePermissions.Id.NotFound";
        #endregion
        #region RolePermissions validation message
        /// <summary>
        /// Tên quyền cho phép không được để trống
        /// </summary>
        public const string NameEmpty = "Mes.RolePermissions.Name.NameIsEmpty";
        /// <summary>
        /// Tên quyền cho phép không hợp lệ
        /// </summary>
        public const string NameInValid = "Mes.RolePermissions.Name.InValid";
        /// <summary>
        /// Id quyền cho phép là bắt buộc
        /// </summary>
        public const string IdIsRequired = "Mes.RolePermissions.Id.IsRequired";
        #endregion

        #region RolePermissions action message
        /// <summary>
        /// Lấy thồng tin chi tiết quyền cho phép thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.RolePermissions.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin quyền cho phép thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.RolePermissions.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin quyền cho phép thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.RolePermissions.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin quyền cho phép thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.RolePermissions.Update.Failed";
        /// <summary>
        /// Xóa thông tin quyền cho phép thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.RolePermissions.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin quyền cho phép thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.RolePermissions.Delete.Failed";
        #endregion

    }

    [DisplayName("UserPermissions")]
    public static class UserPermissions
    {
        #region Check UserPermissions message
        // <summary>
        /// Id người dùng cho phép không tìm được
        /// </summary>
        public const string NotExist = "Mes.UserPermissions.NotExist";
        // <summary>
        /// Id người dùng cho phép đã tồn tại
        /// </summary>
        public const string AlreadyExist = "Mes.UserPermissions.AlreadyExist";
        #endregion
        #region UserPermissions validation message
        /// <summary>
        /// Tên người dùng cho phép không được để trống
        /// </summary>
        public const string NameNotEmpty = "Mes.UserPermissions.Name.NameIsNotEmpty";
        /// <summary>
        /// Tên người dùng cho phép không hợp lệ
        /// </summary>
        public const string NameInValid = "Mes.UserPermissions.Name.InValid";
        /// <summary>
        /// Id người dùng cho phép là bắt buộc
        /// </summary>
        public const string IdIsRequired = "Mes.UserPermissions.Id.IsRequired";
        #endregion
        #region UserPermissions action message
        /// <summary>
        /// Lấy thồng tin chi tiết người dùng cho phép thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.UserPermissions.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin người dùng cho phép thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.UserPermissions.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin người dùng cho phép thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.UserPermissions.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin người dùng cho phép thất bại
        /// </summary>
        public const string CreateFailed = "Mes.UserPermissions.Create.Failed";
        /// <summary>
        /// Cập nhập thông tin người dùng cho phép thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.UserPermissions.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin người dùng cho phép thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.UserPermissions.Update.Failed";
        /// <summary>
        /// Xóa thông tin người dùng cho phép thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.UserPermissions.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin người dùng cho phép thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.UserPermissions.Delete.Failed";
        #endregion
    }

    [DisplayName("Permissions")]
    public static class Permissions
    {
        #region Check Permissions message
        /// <summary>
        /// Không tìm được code
        /// </summary>
        public const string CodeNotFound = "Mes.Permissions.Code.NotFound";
        /// <summary>
        /// Code đã tồn tại
        /// </summary>
        public const string CodeAlreadyExist = "Mes.Permissions.Code.AlreadyExist";
        #endregion
        #region Permissions validation message
        /// <summary>
        /// Code là bắt buộc
        /// </summary>
        public const string CodeIsRequired = "Mes.Permissions.Code.Required";
        /// <summary>
        /// Code không được để trống
        /// </summary>
        public const string CodeNotEmpty = "Mes.Permissions.Code.NotEmpty";
        /// <summary>
        /// Tên không được để trống
        /// </summary>
        public const string NameNotEmpty = "Mes.Permissions.Name.NotEmpty";
        /// <summary>
        /// Tên không hợp lệ
        /// </summary>
        public const string NameInValid = "Mes.Permissions.Name.InValid";
        #endregion

        #region Permissions action message
        /// <summary>
        /// Lấy thồng tin chi tiết sự cho phép thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Permissions.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin sự cho phép thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Permissions.GetAll.Successfully";
        /// <summary>
        /// Tạo thông tin sự cho phép thành công
        /// </summary>
        public const string CreateSuccessfully = "Mes.Permissions.Create.Successfully";
        /// <summary>
        /// Tạo thông tin sự cho phép thất bại
        /// </summary>
        public const string CreateFailed = "Mes.Permissions.Create.Failed";
        /// <summary>
        /// Xóa thông tin sự cho phép thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Permissions.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin sự cho phép thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Permissions.Update.Failed";
        /// <summary>
        /// Xóa thông tin sự cho phép thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Permissions.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin sự cho phép thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Permissions.Delete.Failed";
        #endregion

    }

    [DisplayName("Products")]
    [Description("Products Messages")]
    public static class Products
    {
        #region Check Products message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.Products.Id.NotFound";
        #endregion

        #region Products validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập id
        /// </summary>
        public const string IdIsRequired = "Mes.Products.Id.IsRequired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string NameIsRequired = "Mes.Products.Name.IsRequired";
        /// <summary>
        /// Độ dài vượt quá giới hạn cho phép 255 kí tự
        /// </summary>
        public const string NameIsOverLength = "Mes.Products.Name.OverLength";
        /// <summary>
        /// Yêu cầu bắt buộc nhập thông tin chi tiết
        /// </summary>
        public const string DetailIsRequired = "Mes.Products.Detail.IsRequired";
        /// <summary>
        /// hình ảnh sản phẩm không hợp lệ
        /// </summary>
        public const string ImageInValid = "Mes.Products.Image.InValid";
        /// <summary>
        /// hình ảnh sản phẩm không hợp lệ
        /// </summary>
        public const string ImageIsRequired = "Mes.Products.Image.Required";
        /// <summary>
        /// thumb sản phẩm không hợp lệ
        /// </summary>
        public const string ThumbnailInValid = "Mes.Products.Thumbnail.InValid";
        /// <summary>
        /// thumb sản phẩm không hợp lệ
        /// </summary>
        public const string ThumbnailIsRequired = "Mes.Products.Thumbnail.Required";
        /// <summary>
        /// hình ảnh thông tin dinh dưỡng sản phẩm không hợp lệ
        /// </summary>
        public const string NutritionInfoIsRequired = "Mes.Products.NutritionInfo.IsRequired";
        /// <summary>
        /// BMI To phải lớn hơn BMI From
        /// </summary>
        public const string BMIToInvalid = "Mes.Products.BMITo.InValid";
        /// <summary>
        /// Tọa độ này đã có sản phẩm
        /// </summary>
        public const string CoordinateInvalid = "Mes.Products.Coordinate.Invalid";
        /// <summary>
        /// Tọa độ này đã có sản phẩm
        /// </summary>
        public const string CoordinateAlreadyExist = "Mes.Products.Coordinate.AlreadyExist";
        #endregion

        #region Products action message
        /// <summary>
        /// Lấy thồng tin chi tiết sản phẩm thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Products.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin sản phẩm thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Products.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Products.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Products.Update.Failed";
        /// <summary>
        /// Cập nhập hình sản phẩm thất bại
        /// </summary>
        public const string UpdateImageFailed = "Mes.Products.Update.Image.Failed";
        /// <summary>
        /// Cập nhập thumb sản phẩm thất bại
        /// </summary>
        public const string UpdateThumbnailFailed = "Mes.Products.Update.Thumbnail.Failed";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thất bại
        /// </summary>
        public const string CreateSuccessfully = "Mes.Products.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thất bại
        /// </summary>
        public const string CreateFailed = "Mes.Products.Create.Failed";
        /// <summary>
        /// Xóa thông tin sản phẩm thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Products.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin sản phẩm thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Products.Delete.Failed";
        #endregion
    }
    [DisplayName("ProductRecommends")]
    [Description("ProductRecommends Messages")]
    public static class ProductRecommends
    {
        #region Check ProductRecommends message
        /// <summary>
        /// Không tìm thấy sản phẩm khuyên dùng
        /// </summary>
        public const string NotFound = "Mes.ProductRecommends.NotFound";
        /// <summary>
        /// Không tìm thấy sản phẩm khuyên dùng
        /// </summary>
        public const string AlreadyExist = "Mes.ProductRecommends.AlreadyExist";
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.ProductRecommends.Id.NotFound";
        public const string ProductIndexInValid = "Mes.ProductRecommends.ProductIndex.InValid";
        #endregion

        #region ProductRecommends validation message
        #endregion

        #region ProductRecommends action message
        /// <summary>
        /// Lấy thồng tin chi tiết sản phẩm khuyên dùng thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.ProductRecommends.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin sản phẩm khuyên dùng thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.ProductRecommends.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm khuyên dùng thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.ProductRecommends.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm khuyên dùng thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.ProductRecommends.Update.Failed";
        /// <summary>
        /// Cập nhập thông tin sản phẩm khuyên dùng thất bại
        /// </summary>
        public const string CreateSuccessfully = "Mes.ProductRecommends.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm khuyên dùng thất bại
        /// </summary>
        public const string CreateFailed = "Mes.ProductRecommends.Create.Failed";
        /// <summary>
        /// Xóa thông tin sản phẩm khuyên dùng thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.ProductRecommends.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin sản phẩm khuyên dùng thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.ProductRecommends.Delete.Failed";
        #endregion
    }
    [DisplayName("ProductCategories")]
    [Description("ProductCategories Messages")]
    public static class ProductCategories
    {
        #region Check ProductCategories message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.ProductCategories.Id.NotFound";
        #endregion

        #region ProductCategories validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string IdIsRequired = "Mes.ProductCategories.Id.IsRequired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập vị trí
        /// </summary>
        public const string ProductIndexIsRequired = "Mes.ProductCategories.ProductIndex.IsRequired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string NameIsRequired = "Mes.ProductCategories.Name.IsRequired";
        /// <summary>
        /// Độ dài vượt quá giới hạn cho phép 255 kí tự
        /// </summary>
        public const string NameIsOverLength = "Mes.ProductCategories.Name.OverLength";
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string DescriptionIsRequired = "Mes.ProductCategories.Description.IsRequired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập hình
        /// </summary>
        public const string ImageIsRequired = "Mes.ProductCategories.Image.IsRequired";
        /// <summary>
        /// hình không hợp lệ
        /// </summary>
        public const string ImageInValid = "Mes.ProductCategories.Image.InValid";
        #endregion

        #region ProductCategories action message
        /// <summary>
        /// Lấy thồng tin chi tiết sản phẩm thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.ProductCategories.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin sản phẩm thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.ProductCategories.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.ProductCategories.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.ProductCategories.Update.Failed";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thất bại
        /// </summary>
        public const string CreateSuccessfully = "Mes.ProductCategories.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm thất bại
        /// </summary>
        public const string CreateFailed = "Mes.ProductCategories.Create.Failed";
        /// <summary>
        /// Xóa thông tin sản phẩm thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.ProductCategories.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin sản phẩm thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.ProductCategories.Delete.Failed";
        #endregion
    }
    [DisplayName("Shelfs")]
    [Description("Shelfs Messages")]
    public static class Shelfs
    {
        #region Check Shelfs message
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.Shelfs.Id.NotFound";
        /// <summary>
        /// Không tìm thấy code
        /// </summary>
        public const string CodeNotFound = "Mes.Shelfs.Code.NotFound";
        /// <summary>
        /// Không tìm thấy code
        /// </summary>
        public const string CodeAlreadyExist = "Mes.Shelfs.Code.AlreadyExist";
        #endregion

        #region Shelfs validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập id
        /// </summary>
        public const string IdIsRequired = "Mes.Shelfs.Id.IsRequired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập vị trí
        /// </summary>
        public const string PositionIsRequired = "Mes.Shelfs.Position.IsRequired";
        /// <summary>
        /// Yêu cầu bắt buộc nhập code
        /// </summary>
        public const string CodeIsRequired = "Mes.Shelfs.Code.IsRequired";
        /// <summary>
        /// <summary>
        /// Yêu cầu bắt buộc nhập tên
        /// </summary>
        public const string NameIsRequired = "Mes.Shelfs.Name.IsRequired";
        /// <summary>
        /// Độ dài vượt quá giới hạn cho phép 255 kí tự
        /// </summary>
        public const string NameIsOverLength = "Mes.Shelfs.Name.OverLength";
        /// <summary>
        /// video không hợp lệ
        /// </summary>
        public const string VideoInValid = "Mes.Shelfs.Video.InValid";
        #endregion

        #region Shelfs action message
        /// <summary>
        /// Lấy thồng tin chi tiết kệ thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.Shelfs.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin kệ thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.Shelfs.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin kệ thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.Shelfs.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin kệ thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.Shelfs.Update.Failed";
        /// <summary>
        /// Cập nhập thông tin kệ thất bại
        /// </summary>
        public const string CreateSuccessfully = "Mes.Shelfs.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin kệ thất bại
        /// </summary>
        public const string CreateFailed = "Mes.Shelfs.Create.Failed";
        /// <summary>
        /// Xóa thông tin kệ thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.Shelfs.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin kệ thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.Shelfs.Delete.Failed";
        #endregion
    }
    
    [DisplayName("ShelfProducts")]
    [Description("ShelfProducts Messages")]
    public static class ShelfProducts
    {
        #region Check ShelfProducts message
        /// <summary>
        /// đã tồn tại
        /// </summary>
        public const string AlreadyExist = "Mes.ShelfProducts.AlreadyExist";
        /// <summary>
        /// Không tìm thấy id
        /// </summary>
        public const string IdNotFound = "Mes.ShelfProducts.Id.NotFound";
        /// <summary>
        /// Không tìm thấy code
        /// </summary>
        public const string CodeNotFound = "Mes.ShelfProducts.Code.NotFound";
        /// <summary>
        /// Không tìm thấy code
        /// </summary>
        public const string CodeAlreadyExist = "Mes.ShelfProducts.Code.AlreadyExist";
        #endregion

        #region ShelfProducts validation message
        /// <summary>
        /// Yêu cầu bắt buộc nhập id
        /// </summary>
        public const string IdIsRequired = "Mes.ShelfProducts.Id.IsRequired";
        #endregion

        #region ShelfProducts action message
        /// <summary>
        /// Lấy thồng tin chi tiết sản phẩm trong kệ thành công
        /// </summary>
        public const string GetDetailSuccessfully = "Mes.ShelfProducts.GetDetail.Successfully";
        /// <summary>
        /// Lấy tất cả thông tin sản phẩm trong kệ thành công
        /// </summary>
        public const string GetAllSuccessfully = "Mes.ShelfProducts.GetAll.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm trong kệ thành công
        /// </summary>
        public const string UpdateSuccessfully = "Mes.ShelfProducts.Update.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm trong kệ thất bại
        /// </summary>
        public const string UpdateFailed = "Mes.ShelfProducts.Update.Failed";
        /// <summary>
        /// Cập nhập thông tin sản phẩm trong kệ thất bại
        /// </summary>
        public const string CreateSuccessfully = "Mes.ShelfProducts.Create.Successfully";
        /// <summary>
        /// Cập nhập thông tin sản phẩm trong kệ thất bại
        /// </summary>
        public const string CreateFailed = "Mes.ShelfProducts.Create.Failed";
        /// <summary>
        /// Xóa thông tin sản phẩm trong kệ thành công
        /// </summary>
        public const string DeleteSuccessfully = "Mes.ShelfProducts.Delete.Successfully";
        /// <summary>
        /// Xóa thông tin sản phẩm trong kệ thất bại
        /// </summary>
        public const string DeleteFailed = "Mes.ShelfProducts.Delete.Failed";
        #endregion
    }
}

