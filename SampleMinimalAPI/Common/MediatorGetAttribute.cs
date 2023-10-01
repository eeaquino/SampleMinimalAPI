namespace SampleMinimalAPI.Common
{
    public abstract class MediatorHttpMethodAttribute : Attribute
    {
        private bool secure;
        public string Route { get; set; }
        public string Tag { get; set; }
        public DataBindEnum DataBind { get; set; }

        public MediatorHttpMethodAttribute(string route, string tag, bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters)
        {
            this.Route = route;
            this.Tag = tag;
            this.secure = secure;
            this.DataBind = dataBind;
        }
    }
    public class MediatorGetAttribute : MediatorHttpMethodAttribute
    {
        public MediatorGetAttribute(string route, string tag, bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters) : base(route, tag, secure, dataBind)
        {
        }
    }

    public class MediatorPostAttribute : MediatorHttpMethodAttribute
    {
        public MediatorPostAttribute(string route, string tag, bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters) : base(route, tag, secure, dataBind)
        {
        }
    }

    public class MediatorDeleteAttribute : MediatorHttpMethodAttribute
    {
        public MediatorDeleteAttribute(string route, string tag, bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters) : base(route, tag, secure, dataBind)
        {
        }
    }

    public class MediatorPutAttribute : MediatorHttpMethodAttribute
    {
        public MediatorPutAttribute(string route, string tag, bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters) : base(route, tag, secure, dataBind)
        {
        }
    }

    public class MediatorPatchAttribute : MediatorHttpMethodAttribute
    {
        public MediatorPatchAttribute(string route, string tag, bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters) : base(route, tag, secure, dataBind)
        {
        }
    }
    //public class MediatorGetAttribute:Attribute
    //{
    //    private bool Secure { get; set; }
    //    public string Route { get; set; }
    //    public string Tag { get; set; }
    //    public DataBindEnum DataBind { get; set; }

    //    public MediatorGetAttribute(string route,string tag,bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters)
    //    {
    //        Secure = secure;
    //        Route = route;
    //        DataBind = dataBind;
    //        Tag = tag;
    //    }

        
    //}
    //public class MediatorPostAttribute:Attribute
    //{
    //    private bool Secure { get; set; }
    //    public string Route { get; set; }
    //    public string Tag { get; set; }
    //    public DataBindEnum DataBind { get; set; }

    //    public MediatorPostAttribute(string route,string tag,bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters)
    //    {
    //        Secure = secure;
    //        Route = route;
    //        DataBind = dataBind;
    //        Tag = tag;
    //    }

        
    //}
    //public class MediatorDeleteAttribute:Attribute
    //{
    //    private bool Secure { get; set; }
    //    public string Route { get; set; }
    //    public string Tag { get; set; }
    //    public DataBindEnum DataBind { get; set; }

    //    public MediatorDeleteAttribute(string route,string tag,bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters)
    //    {
    //        Secure = secure;
    //        Route = route;
    //        DataBind = dataBind;
    //        Tag = tag;
    //    }

        
    //}
    //public class MediatorPutAttribute:Attribute
    //{
    //    private bool Secure { get; set; }
    //    public string Route { get; set; }
    //    public string Tag { get; set; }
    //    public DataBindEnum DataBind { get; set; }

    //    public MediatorPutAttribute(string route,string tag,bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters)
    //    {
    //        Secure = secure;
    //        Route = route;
    //        DataBind = dataBind;
    //        Tag = tag;
    //    }

        
    //}
    //public class MediatorPatchAttribute:Attribute
    //{
    //    private bool Secure { get; set; }
    //    public string Route { get; set; }
    //    public string Tag { get; set; }
    //    public DataBindEnum DataBind { get; set; }

    //    public MediatorPatchAttribute(string route,string tag,bool secure = false, DataBindEnum dataBind = DataBindEnum.AsParameters)
    //    {
    //        Secure = secure;
    //        Route = route;
    //        DataBind = dataBind;
    //        Tag = tag;
    //    }

        
    //}
}
