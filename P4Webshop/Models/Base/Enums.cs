namespace P4Webshop.Models.Base
{ 
    public enum Roles
    {
        Customer = 1,
        Employee = 2,
        Admin = 3
    }

    public enum EmployeeRoles
    {
        Foodstand = 1,
        Security = 2,
    }

    public enum ResponseCode
    {
        OK = 1,
        Error = 2,
        UnAuthorize = 3,
        Forbidden = 4,
    }

    public enum ProductCategory
    {
        Keyboard,
        Smartphone,
        Microphone,
        Mouse
    }

    public enum PowerModes
    {
        Resume,
        Suspend
    }

    public enum ControlWorker
    {
        DeliveryWorker,
        DataPullWorker
    }
}
