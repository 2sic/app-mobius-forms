using ToSic.Razor.Blade;
using ThisApp.Data;
using ThisApp.Fields;


namespace ThisApp.Csv
{
  public class CsvHelper : Custom.Hybrid.CodeTyped
  {
    // TODO: @2dg & @2ro - this is how it should be done using Link.To(api: )
    // plus it should be shared, as one link doesnt work ATM
    public string GetDownloadLink()
    {
      var mid = MyContext.Module.Id;
      return Link.To(api: $"{MyView.Edition}/api/Csv/Csv?PageId={MyContext.Page.Id}&ModuleId={mid}&id{mid}");
    }

  }
}