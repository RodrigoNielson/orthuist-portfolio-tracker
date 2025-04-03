using Application.Domain.Portfolios;
using Ardalis.Result;
using AutoFixture;
using FluentAssertions;

namespace UnitTests.Domain;

public class PortfolioTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void CreateMovement_ShouldReturnNotFound_WhenAssetDoesNotExist()
    {
        // Arrange
        var portfolio = new Portfolio();
        var assetId = _fixture.Create<Guid>();
        var price = _fixture.Create<decimal>();
        var quantity = _fixture.Create<decimal>();
        var movementType = _fixture.Create<MovementType>();
        var date = _fixture.Create<DateTime>();

        // Act
        var result = portfolio.CreateMovement(assetId, price, quantity, date, movementType);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void CreateMovement_ShouldReturnSuccess_WhenAssetExists()
    {
        // Arrange
        var portfolio = new Portfolio();
        var asset = _fixture.Create<PortfolioAsset>();

        portfolio.Assets.Add(asset);

        var price = _fixture.Create<decimal>();
        var quantity = _fixture.Create<decimal>();
        var movementType = _fixture.Create<MovementType>();
        var date = _fixture.Create<DateTime>();

        // Act
        var result = portfolio.CreateMovement(asset.Id, price, quantity, date, movementType);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void DeleteMovement_ShouldReturnNotFound_WhenAssetDoesNotExist()
    {
        // Arrange
        var portfolio = new Portfolio();
        var assetId = _fixture.Create<Guid>();
        var movementId = _fixture.Create<Guid>();

        // Act
        var result = portfolio.DeleteMovement(assetId, movementId);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void DeleteMovement_ShouldReturnSuccess_WhenAssetExists()
    {
        // Arrange
        var portfolio = new Portfolio();

        var positiveMovements = _fixture.Build<Movement>()
            .With(c => c.Quantity, 10)
            .With(c => c.Type, MovementType.Add)
            .CreateMany()
            .ToList();

        var asset = _fixture.Build<PortfolioAsset>()
                            .With(c => c.Movements, positiveMovements)
                            .Create();

        var movement = _fixture.Create<Movement>();

        asset.Movements.Add(movement);
        portfolio.Assets.Add(asset);

        // Act
        var result = portfolio.DeleteMovement(asset.Id, movement.Id);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void CreateAsset_ShouldReturnConflict_WhenAssetAlreadyExists()
    {
        // Arrange
        var portfolio = new Portfolio();
        var asset = _fixture.Create<PortfolioAsset>();

        portfolio.Assets.Add(asset);

        var date = _fixture.Create<DateTime>();

        // Act
        var result = portfolio.CreateAsset(asset.Code, asset.Name, asset.Type, asset.Movements.First().Price, asset.Movements.First().Quantity, date);

        // Assert
        result.Status.Should().Be(ResultStatus.Conflict);
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void CreateAsset_ShouldReturnSuccess_WhenAssetDoesNotExist()
    {
        // Arrange
        var portfolio = new Portfolio();
        var code = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var type = _fixture.Create<PortfolioAssetType>();
        var price = _fixture.Create<decimal>();
        var quantity = _fixture.Create<decimal>();
        var date = _fixture.Create<DateTime>();

        // Act
        var result = portfolio.CreateAsset(code, name, type, price, quantity, date);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void DeleteAsset_ShouldReturnNotFound_WhenAssetDoesNotExist()
    {
        // Arrange
        var portfolio = new Portfolio();
        var assetId = _fixture.Create<Guid>();

        // Act
        var result = portfolio.DeleteAsset(assetId);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void DeleteAsset_ShouldReturnSuccess_WhenAssetExists()
    {
        // Arrange
        var portfolio = new Portfolio();
        var asset = _fixture.Create<PortfolioAsset>();

        portfolio.Assets.Add(asset);

        // Act
        var result = portfolio.DeleteAsset(asset.Id);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
    }
}
