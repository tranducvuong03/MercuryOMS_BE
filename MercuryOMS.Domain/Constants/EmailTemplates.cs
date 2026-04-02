namespace MercuryOMS.Domain.Constants
{
    public static class EmailTemplates
    {
        public static string ConfirmEmail(string confirmLink)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Xác nhận email - MercuryOMS</title>
                </head>
                <body style='margin:0; padding:0; background-color:#f4f7fa; font-family: Arial, Helvetica, sans-serif;'>
                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color:#f4f7fa; padding: 40px 0;'>
                        <tr>
                            <td align='center'>
                                <!-- Container chính -->
                                <table role='presentation' width='600' cellspacing='0' cellpadding='0' style='max-width:600px; background-color:#ffffff; border-radius:8px; overflow:hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
                       
                                    <!-- Header - Gradient Cam -->
                                    <tr>
                                        <td style='background: linear-gradient(135deg, #e65100, #f57c00); padding: 30px 40px; text-align:center;'>
                                            <h1 style='margin:0; color:#ffffff; font-size:28px; font-weight:700; letter-spacing:1px;'>
                                                Mercury<span style='color:#ffe0b2;'>OMS</span>
                                            </h1>
                                            <p style='margin:8px 0 0 0; color:#ffe0b2; font-size:15px;'>
                                                Hệ thống quản lý đơn hàng thông minh
                                            </p>
                                        </td>
                                    </tr>

                                    <!-- Nội dung -->
                                    <tr>
                                        <td style='padding: 50px 40px 40px; text-align:center;'>
                                            <h2 style='margin:0 0 20px 0; color:#e65100; font-size:24px;'>
                                                Xác nhận địa chỉ email của bạn
                                            </h2>
                               
                                            <p style='margin:0 0 30px 0; color:#475569; font-size:16px; line-height:1.6;'>
                                                Cảm ơn bạn đã đăng ký tài khoản MercuryOMS.<br>
                                                Vui lòng nhấn vào nút bên dưới để xác nhận email và kích hoạt tài khoản.
                                            </p>

                                            <!-- Nút xác nhận - Màu cam nổi bật -->
                                            <a href='{confirmLink}'
                                               style='display:inline-block; background-color:#f57c00; color:#ffffff;
                                                      padding:16px 36px; text-decoration:none; border-radius:6px;
                                                      font-size:17px; font-weight:600; margin:10px 0; box-shadow: 0 4px 10px rgba(245, 124, 0, 0.3);'>
                                                Xác nhận email ngay
                                            </a>

                                            <p style='margin:35px 0 0 0; color:#64748b; font-size:14px; line-height:1.5;'>
                                                Link này sẽ hết hạn sau <strong>24 giờ</strong>.<br>
                                                Nếu bạn không thực hiện đăng ký này, vui lòng bỏ qua email.
                                            </p>
                                        </td>
                                    </tr>

                                    <!-- Divider -->
                                    <tr>
                                        <td style='border-top:1px solid #e2e8f0;'></td>
                                    </tr>

                                    <!-- Footer -->
                                    <tr>
                                        <td style='padding:30px 40px; background-color:#f8fafc; text-align:center; font-size:13px; color:#64748b;'>
                                            <p style='margin:0 0 10px 0;'>
                                                © 2026 MercuryOMS. All rights reserved.
                                            </p>
                                            <p style='margin:0;'>
                                                Nếu cần hỗ trợ, vui lòng liên hệ:
                                                <a href='mailto:crynow2003@gmail.com' style='color:#f57c00; text-decoration:none;'>crynow2003@gmail.com</a>
                                            </p>
                                        </td>
                                    </tr>
                                </table>

                                <!-- Preheader / Footer nhỏ -->
                                <p style='margin:20px 0 0 0; color:#94a3b8; font-size:12px;'>
                                    Email này được gửi từ MercuryOMS
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
        }
    }
}