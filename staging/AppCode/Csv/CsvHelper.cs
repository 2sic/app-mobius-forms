namespace AppCode.Csv
{
  public class CsvHelper : Custom.Hybrid.CodeTyped
  {
    public string GetDownloadLink(int formId)
    {
      return Link.To(api: $"{MyView.Edition}/api/Csv/Csv?PageId={MyContext.Page.Id}&ModuleId={MyContext.Module.Id}&id={formId}");
    }
  }
}