namespace EtehadBar.Domain
{
    public enum DefinitionType
    {
        Car
    }

    public enum ApplicationRoleType
    {
        Admin,
        User
    }

    public enum PaymentType
    {
        AdvanceMoney,
        Salary
    }

    public enum CustomerType
    {
        SaipaPlasco,
        SaipaPress,
        SazehGostar,
        MehrcomPars
    }

    public enum ShippingFeeType
    {
        Normal,
        Custom
    }

    public enum LoadRouteType
    {
        Origin,
        Destionation
    }

    public enum SaipaPressLoadType
    {
        OneFloor,
        TwoFloor
    }

    public enum ExcelExportType
    {
        WithAllPrices,
        OnlyReceivingPrice,
        OnlyDriverPrice,
        WithoutPrice
    }
}
