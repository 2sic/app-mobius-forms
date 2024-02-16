using System.Linq;
namespace AppCode.Csv
{
  public class CsvHelper : Custom.Hybrid.CodeTyped
  {
    public string GetDownloadLink(int formId)
    {
      var mid = MyContext.Module.Id;
      return Link.To(api: $"{MyView.Edition}/api/Csv/Csv?PageId={MyContext.Page.Id}&ModuleId={mid}&id={formId}");
    }

  }
}