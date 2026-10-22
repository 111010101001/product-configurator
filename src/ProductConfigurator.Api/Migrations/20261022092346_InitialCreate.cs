using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductConfigurator.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sku = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptionGroups_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedConfigurations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OptionGroupId = table.Column<int>(type: "int", nullable: false),
                    PriceDelta = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationOptions_OptionGroups_OptionGroupId",
                        column: x => x.OptionGroupId,
                        principalTable: "OptionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompatibilityRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SourceOptionId = table.Column<int>(type: "int", nullable: false),
                    TargetOptionId = table.Column<int>(type: "int", nullable: false),
                    RuleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompatibilityRules", x => x.Id);
                    table.CheckConstraint("CK_CompatibilityRule_ExcludesOrdered", "[RuleType] <> 2 OR [SourceOptionId] < [TargetOptionId]");
                    table.CheckConstraint("CK_CompatibilityRule_NotSelf", "[SourceOptionId] <> [TargetOptionId]");
                    table.ForeignKey(
                        name: "FK_CompatibilityRules_ConfigurationOptions_SourceOptionId",
                        column: x => x.SourceOptionId,
                        principalTable: "ConfigurationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibilityRules_ConfigurationOptions_TargetOptionId",
                        column: x => x.TargetOptionId,
                        principalTable: "ConfigurationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibilityRules_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationSelections",
                columns: table => new
                {
                    SavedConfigurationId = table.Column<int>(type: "int", nullable: false),
                    ConfigurationOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationSelections", x => new { x.SavedConfigurationId, x.ConfigurationOptionId });
                    table.ForeignKey(
                        name: "FK_ConfigurationSelections_ConfigurationOptions_ConfigurationOptionId",
                        column: x => x.ConfigurationOptionId,
                        principalTable: "ConfigurationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationSelections_SavedConfigurations_SavedConfigurationId",
                        column: x => x.SavedConfigurationId,
                        principalTable: "SavedConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilityRules_ProductId",
                table: "CompatibilityRules",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilityRules_SourceOptionId_TargetOptionId_RuleType",
                table: "CompatibilityRules",
                columns: new[] { "SourceOptionId", "TargetOptionId", "RuleType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilityRules_TargetOptionId",
                table: "CompatibilityRules",
                column: "TargetOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationOptions_OptionGroupId",
                table: "ConfigurationOptions",
                column: "OptionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationSelections_ConfigurationOptionId",
                table: "ConfigurationSelections",
                column: "ConfigurationOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionGroups_ProductId",
                table: "OptionGroups",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedConfigurations_ProductId",
                table: "SavedConfigurations",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompatibilityRules");

            migrationBuilder.DropTable(
                name: "ConfigurationSelections");

            migrationBuilder.DropTable(
                name: "ConfigurationOptions");

            migrationBuilder.DropTable(
                name: "SavedConfigurations");

            migrationBuilder.DropTable(
                name: "OptionGroups");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
