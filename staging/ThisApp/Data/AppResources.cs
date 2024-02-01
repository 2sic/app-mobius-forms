namespace ThisApp.Data
{
  public partial class AppResources { 
    public FormResources DefaultFormResources => _formResources ??= Child<FormResources>(nameof(DefaultFormResources));
    private FormResources _formResources;
  }

}