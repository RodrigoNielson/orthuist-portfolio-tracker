using FluentMigrator;

namespace Migrations.Migrations;

[Migration(2024_09_06_000000)]
public class Initial : Migration
{
    const string PortfolioTable = "Portfolio";
    const string PortfolioAssetTable = "PortfolioAsset";
    const string MovementTable = "Movement";

    public override void Up()
    {
        Create.Table(PortfolioTable)
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(50).NotNullable()
            .WithColumn("Created_At").AsDateTime().NotNullable()
            .WithColumn("Updated_At").AsDateTime().Nullable();

        Create.Table(PortfolioAssetTable)
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn($"Portfolio_Id").AsGuid()
            .WithColumn("Code").AsString().NotNullable()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("Type").AsInt16().NotNullable()
            .WithColumn("Created_At").AsDateTime().NotNullable()
            .WithColumn("Updated_At").AsDateTime().Nullable();

        Create.ForeignKey()
            .FromTable(PortfolioAssetTable).ForeignColumn($"Portfolio_Id")
            .ToTable(PortfolioTable).PrimaryColumn("Id");

        Create.Table(MovementTable)
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn($"Portfolio_Asset_Id").AsGuid().ForeignKey()
            .WithColumn("Quantity").AsDecimal().NotNullable()
            .WithColumn("Price").AsDecimal().NotNullable()
            .WithColumn("Type").AsInt16().NotNullable()
            .WithColumn("Movement_Date").AsDateTime().NotNullable()
            .WithColumn("Created_At").AsDateTime().NotNullable()
            .WithColumn("Updated_At").AsDateTime().Nullable();

        Create.ForeignKey()
            .FromTable(MovementTable).ForeignColumn("Portfolio_Asset_Id")
            .ToTable(PortfolioAssetTable).PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete.Table(PortfolioTable);
        Delete.Table(PortfolioAssetTable);
        Delete.Table(MovementTable);
    }
}