//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace X.Pages {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("/Users/lldm0040/Desktop/Projects/x-app/x-mobile/X/Pages/SignUp.xaml")]
    public partial class SignUp : global::Xamarin.Forms.ContentPage {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::X.TransparentEntry fname;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::X.TransparentEntry lname;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::X.TransparentEntry email;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::X.EmailValidator emailValidator;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::X.TransparentEntry password;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(SignUp));
            fname = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::X.TransparentEntry>(this, "fname");
            lname = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::X.TransparentEntry>(this, "lname");
            email = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::X.TransparentEntry>(this, "email");
            emailValidator = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::X.EmailValidator>(this, "emailValidator");
            password = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::X.TransparentEntry>(this, "password");
        }
    }
}
