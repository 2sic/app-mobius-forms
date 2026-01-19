namespace AppCode.Extensions.GoogleRecaptchaV3
{
  public class RecaptchaResult
  {
    public bool IsValid { get; set; }
    public double? Score { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
    public string Error { get; set; }
  }
}
