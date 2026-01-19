namespace AppCode.Extensions.GoogleRecaptchaV3
{
  // Response vom Google reCAPTCHA siteverify Endpoint
  public class RecaptchaResponse
  {
    public bool Success { get; set; }
    public double? Score { get; set; }
    public string Challenge_ts { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
  }
}
