namespace EtehadBar.Domain
{
    public enum DefinitionType
    {
        Car,
        BillType,
        BankBranch,
        CostAccount,
        BankName,
        LoadFactorOrigin
    }

    public enum ApplicationRoleType
    {
        Admin,
        User,
        RegisterUser,
        Partner,
        Investor
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
        MehrcomPars,
        Mayan
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

    public enum SazehGostarLoadType
    {
        OneWay,
        TwoWay
    }

    public enum ExcelExportType
    {
        WithAllPrices,
        OnlyReceivingPrice,
        OnlyDriverPrice,
        WithoutPrice
    }

    public enum SystemCacheNames
    {
        UserProfileList
    }

    public enum BankAccountBookAmountType
    {
        Debtor,
        Creditor
    }

    public enum SystemLogEventType
    {
        //debug
        GenerateItems = 1000,

        //information
        ListItems = 1001,
        GetItem = 1002,
        AddItem = 1003,
        UpdateItem = 1004,
        DeleteItem = 1005,

        ActionExecuting = 1100,
        ActionExecuted = 1101,

        //fatal
        TestItem = 3000,


        //fatal
        GetItemNotFound = 4000,
        UpdateItemNotFound = 4001,
        DeleteItemNotFound = 4002,

        //error
        DatabaseException = 5000
    }

    public enum BankAccountBookType
    {
        Salary,
        AdvanceMoney,
        Cost,
        PaymentToDriver,
        Other
    }

    public enum TurnoverType
    {
        Partner,
        Investor
    }
}
