﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1.ChefWebService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ChefWebService.IChefWebService")]
    public interface IChefWebService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChefWebService/StartChef", ReplyAction="http://tempuri.org/IChefWebService/StartChefResponse")]
        void StartChef(string ChefArgs);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChefWebService/GetExitCode", ReplyAction="http://tempuri.org/IChefWebService/GetExitCodeResponse")]
        int GetExitCode();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChefWebService/GetProcessOutput", ReplyAction="http://tempuri.org/IChefWebService/GetProcessOutputResponse")]
        ChefService.WebService.ProcessOutput GetProcessOutput();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChefWebService/HasExited", ReplyAction="http://tempuri.org/IChefWebService/HasExitedResponse")]
        bool HasExited();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChefWebService/ClearError", ReplyAction="http://tempuri.org/IChefWebService/ClearErrorResponse")]
        void ClearError();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IChefWebServiceChannel : ConsoleApplication1.ChefWebService.IChefWebService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ChefWebServiceClient : System.ServiceModel.ClientBase<ConsoleApplication1.ChefWebService.IChefWebService>, ConsoleApplication1.ChefWebService.IChefWebService {
        
        public ChefWebServiceClient() {
        }
        
        public ChefWebServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ChefWebServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChefWebServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChefWebServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void StartChef(string ChefArgs) {
            base.Channel.StartChef(ChefArgs);
        }
        
        public int GetExitCode() {
            return base.Channel.GetExitCode();
        }
        
        public ChefService.WebService.ProcessOutput GetProcessOutput() {
            return base.Channel.GetProcessOutput();
        }
        
        public bool HasExited() {
            return base.Channel.HasExited();
        }
        
        public void ClearError() {
            base.Channel.ClearError();
        }
    }
}
