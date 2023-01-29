using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtehadBar.Infra.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AdminTheme",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminTheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Birth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LockoutReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendar",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Config",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    SmsCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SmsUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SmsPass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailSmtpDomain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailPassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MailDisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    LoadFactorDeductions = table.Column<double>(type: "float", nullable: false),
                    WithholdingTax = table.Column<double>(type: "float", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Definition",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DefinitionType = table.Column<int>(type: "int", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoadRoute",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RouteType = table.Column<int>(type: "int", maxLength: 128, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadRoute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaipaPressLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoadType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoadFactorId = table.Column<long>(type: "bigint", maxLength: 50, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaipaPressLoadFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SazehGostarLoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", maxLength: 50, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegisterCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Certain = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Nature = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false),
                    DetailedCostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LoadFactorId = table.Column<long>(type: "bigint", maxLength: 50, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SazehGostarLoadFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFeeLoadType",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFeeLoadType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LeftNumber = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    NumberWord = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    RightNumber = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IranStateNumber = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "dbo",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cost",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cost_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cost_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentContractId = table.Column<long>(type: "bigint", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contract_Contract_ParentContractId",
                        column: x => x.ParentContractId,
                        principalSchema: "dbo",
                        principalTable: "Contract",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contract_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerIncome",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CalendarId = table.Column<long>(type: "bigint", maxLength: 50, nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerIncome", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerIncome_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerIncome_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CalendarId = table.Column<long>(type: "bigint", maxLength: 50, nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoadFactor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    DriverFee = table.Column<double>(type: "float", nullable: false),
                    TonnagePrice = table.Column<double>(type: "float", nullable: true),
                    DriverTonnagePrice = table.Column<double>(type: "float", nullable: true),
                    LoadNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoadNumberGov = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExitNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    LoadFactorDeductions = table.Column<double>(type: "float", nullable: false),
                    WithholdingTax = table.Column<double>(type: "float", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ShippingFeeId = table.Column<long>(type: "bigint", nullable: false),
                    Tonnage = table.Column<double>(type: "float", nullable: true),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    CalendarId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    SaipaPressLoadFactorId = table.Column<long>(type: "bigint", nullable: true),
                    SazehGostarLoadFactorId = table.Column<long>(type: "bigint", nullable: true),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadFactor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadFactor_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "dbo",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadFactor_Calendar_CalendarId",
                        column: x => x.CalendarId,
                        principalSchema: "dbo",
                        principalTable: "Calendar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadFactor_Contract_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "dbo",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadFactor_LoadRoute_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoadFactor_LoadRoute_OriginId",
                        column: x => x.OriginId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoadFactor_SaipaPressLoadFactor_SaipaPressLoadFactorId",
                        column: x => x.SaipaPressLoadFactorId,
                        principalSchema: "dbo",
                        principalTable: "SaipaPressLoadFactor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoadFactor_SazehGostarLoadFactor_SazehGostarLoadFactorId",
                        column: x => x.SazehGostarLoadFactorId,
                        principalSchema: "dbo",
                        principalTable: "SazehGostarLoadFactor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoadFactor_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "dbo",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFee",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint", nullable: false),
                    Vehicle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    DriverPrice = table.Column<double>(type: "float", nullable: false),
                    TonnagePrice = table.Column<double>(type: "float", nullable: true),
                    DriverTonnagePrice = table.Column<double>(type: "float", nullable: true),
                    ShippingFeeType = table.Column<int>(type: "int", nullable: false),
                    ShippingFeeLoadTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    RowId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingFee_Contract_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "dbo",
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingFee_LoadRoute_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingFee_LoadRoute_OriginId",
                        column: x => x.OriginId,
                        principalSchema: "dbo",
                        principalTable: "LoadRoute",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingFee_ShippingFeeLoadType_ShippingFeeLoadTypeId",
                        column: x => x.ShippingFeeLoadTypeId,
                        principalSchema: "dbo",
                        principalTable: "ShippingFeeLoadType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Config",
                columns: new[] { "Id", "Address", "Domain", "Email", "LoadFactorDeductions", "MailDisplayName", "MailPassword", "MailSmtpDomain", "MailUserName", "RowId", "SmsCenter", "SmsPass", "SmsUser", "Tel", "VAT", "WithholdingTax", "Year" },
                values: new object[] { 1, null, null, null, 5.0, null, null, null, null, "8bd8d4c9-7595-4b03-95c7-91ab91046965", null, null, null, null, 9.0, 3.0, "1401" });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Customer",
                columns: new[] { "Id", "CustomerType", "Name", "RowId", "Status" },
                values: new object[,]
                {
                    { 1L, 0, "پلاسکو کار سایپا", "29f78114-f72a-427a-a3f1-8864e6eeb13c", true },
                    { 2L, 1, "سایپا پرس", "e1cbee6e-f7a1-4a84-a1c5-e740fb84fa7d", true },
                    { 3L, 2, "سازه گستر", "df204398-5c7c-4caf-98c0-0c9b9be54a6f", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminTheme_RowId",
                schema: "dbo",
                table: "AdminTheme",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "dbo",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "dbo",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "dbo",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "dbo",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "dbo",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_RowId",
                schema: "dbo",
                table: "Calendar",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_CustomerId",
                schema: "dbo",
                table: "Contract",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_ParentContractId",
                schema: "dbo",
                table: "Contract",
                column: "ParentContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_RowId",
                schema: "dbo",
                table: "Contract",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cost_CalendarId",
                schema: "dbo",
                table: "Cost",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Cost_RowId",
                schema: "dbo",
                table: "Cost",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cost_UserId",
                schema: "dbo",
                table: "Cost",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_RowId",
                schema: "dbo",
                table: "Customer",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIncome_CalendarId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIncome_CustomerId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIncome_RowId",
                schema: "dbo",
                table: "CustomerIncome",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Definition_RowId",
                schema: "dbo",
                table: "Definition",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_CalendarId",
                schema: "dbo",
                table: "LoadFactor",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_ContractId",
                schema: "dbo",
                table: "LoadFactor",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_DestinationId",
                schema: "dbo",
                table: "LoadFactor",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_DriverId",
                schema: "dbo",
                table: "LoadFactor",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_OriginId",
                schema: "dbo",
                table: "LoadFactor",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_RowId",
                schema: "dbo",
                table: "LoadFactor",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_SaipaPressLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SaipaPressLoadFactorId",
                unique: true,
                filter: "[SaipaPressLoadFactorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_SazehGostarLoadFactorId",
                schema: "dbo",
                table: "LoadFactor",
                column: "SazehGostarLoadFactorId",
                unique: true,
                filter: "[SazehGostarLoadFactorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoadFactor_VehicleId",
                schema: "dbo",
                table: "LoadFactor",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadRoute_RowId",
                schema: "dbo",
                table: "LoadRoute",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CalendarId",
                schema: "dbo",
                table: "Payment",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RowId",
                schema: "dbo",
                table: "Payment",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_VehicleId",
                schema: "dbo",
                table: "Payment",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaipaPressLoadFactor_RowId",
                schema: "dbo",
                table: "SaipaPressLoadFactor",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SazehGostarLoadFactor_RowId",
                schema: "dbo",
                table: "SazehGostarLoadFactor",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFee_ContractId",
                schema: "dbo",
                table: "ShippingFee",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFee_DestinationId",
                schema: "dbo",
                table: "ShippingFee",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFee_OriginId",
                schema: "dbo",
                table: "ShippingFee",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFee_RowId",
                schema: "dbo",
                table: "ShippingFee",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFee_ShippingFeeLoadTypeId",
                schema: "dbo",
                table: "ShippingFee",
                column: "ShippingFeeLoadTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFeeLoadType_RowId",
                schema: "dbo",
                table: "ShippingFeeLoadType",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_RowId",
                schema: "dbo",
                table: "UploadedFiles",
                column: "RowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RowId",
                schema: "dbo",
                table: "Vehicles",
                column: "RowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminTheme",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Config",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Cost",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CustomerIncome",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Definition",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LoadFactor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingFee",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UploadedFiles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SaipaPressLoadFactor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SazehGostarLoadFactor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Calendar",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Vehicles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Contract",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LoadRoute",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingFeeLoadType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "dbo");
        }
    }
}
