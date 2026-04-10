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

        public static string PaymentSuccess(string fullName, string orderId, string amount, string orderDetailLink)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Thanh toán thành công - MercuryOMS</title>
                </head>
                <body style='margin:0; padding:0; background-color:#f4f7fa; font-family: Arial, Helvetica, sans-serif;'>
                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color:#f4f7fa; padding: 40px 0;'>
                        <tr>
                            <td align='center'>
                                <table role='presentation' width='560' cellspacing='0' cellpadding='0' style='max-width:560px; background-color:#ffffff; border-radius:12px; overflow:hidden;'>

                                    <!-- Header -->
                                    <tr>
                                        <td style='background:#e65100; padding:28px 36px; text-align:center;'>
                                            <h1 style='margin:0; color:#ffffff; font-size:20px; font-weight:600; letter-spacing:0.5px;'>
                                                Mercury<span style='color:#ffe0b2;'>OMS</span>
                                            </h1>
                                            <p style='margin:6px 0 0; color:#ffe0b2; font-size:13px;'>
                                                Hệ thống quản lý đơn hàng thông minh
                                            </p>
                                        </td>
                                    </tr>

                                    <!-- Body -->
                                    <tr>
                                        <td style='padding:36px 36px 28px; text-align:center;'>

                                            <!-- Icon check -->
                                            <div style='width:56px; height:56px; border-radius:50%; background-color:#eaf3de; margin:0 auto 20px; display:flex; align-items:center; justify-content:center;'>
                                                <img src='https://cdn-icons-png.flaticon.com/512/190/190411.png' width='28' height='28' alt='success' />
                                            </div>

                                            <h2 style='margin:0 0 6px; font-size:20px; color:#1a1a1a;'>Thanh toán thành công</h2>
                                            <p style='margin:0 0 24px; font-size:14px; color:#64748b;'>Cảm ơn bạn đã tin tưởng MercuryOMS</p>

                                            <!-- Chi tiết -->
                                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0'
                                                   style='background:#f8fafc; border-radius:10px; padding:20px 24px; text-align:left; margin-bottom:24px;'>
                                                <tr>
                                                    <td style='padding:4px 0 12px; font-size:11px; font-weight:600; color:#94a3b8; text-transform:uppercase; letter-spacing:0.8px;' colspan='2'>
                                                        Chi tiết giao dịch
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:6px 0; font-size:14px; color:#64748b;'>Mã đơn hàng</td>
                                                    <td style='padding:6px 0; font-size:14px; font-weight:600; color:#1a1a1a; text-align:right;'>#{orderId}</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding:6px 0; font-size:14px; color:#64748b;'>Khách hàng</td>
                                                    <td style='padding:6px 0; font-size:14px; color:#1a1a1a; text-align:right;'>{fullName}</td>
                                                </tr>
                                                <tr style='border-top:1px solid #e2e8f0;'>
                                                    <td style='padding:12px 0 4px; font-size:14px; font-weight:600; color:#475569;'>Tổng thanh toán</td>
                                                    <td style='padding:12px 0 4px; font-size:18px; font-weight:700; color:#e65100; text-align:right;'>{amount} ₫</td>
                                                </tr>
                                            </table>

                                            <!-- Thông báo giao hàng -->
                                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0'
                                                   style='background:#eaf3de; border-radius:8px; padding:12px 16px; margin-bottom:28px;'>
                                                <tr>
                                                    <td style='font-size:13px; color:#3B6D11; text-align:left;'>
                                                        Đơn hàng đang được xử lý và sẽ giao trong <strong>2–3 ngày làm việc</strong>.
                                                    </td>
                                                </tr>
                                            </table>

                                            <!-- CTA button -->
                                            <a href='{orderDetailLink}'
                                               style='display:inline-block; background:#e65100; color:#ffffff; padding:13px 32px;
                                                      text-decoration:none; border-radius:8px; font-size:15px; font-weight:600;'>
                                                Xem chi tiết đơn hàng
                                            </a>
                                        </td>
                                    </tr>

                                    <!-- Footer -->
                                    <tr>
                                        <td style='border-top:1px solid #e2e8f0; padding:20px 36px; background:#f8fafc; text-align:center;'>
                                            <p style='margin:0 0 6px; font-size:12px; color:#94a3b8;'>© 2026 MercuryOMS. All rights reserved.</p>
                                            <p style='margin:0; font-size:12px; color:#94a3b8;'>
                                                Hỗ trợ: <a href='mailto:crynow2003@gmail.com' style='color:#e65100; text-decoration:none;'>crynow2003@gmail.com</a>
                                            </p>
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
        }
    }
}