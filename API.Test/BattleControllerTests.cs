using API.Controllers;
using API.Test.Fixtures;
using FluentAssertions;
using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Test;

public class BattleControllerTests
{
    private readonly Mock<IBattleOfMonstersRepository> _repository;

    public BattleControllerTests()
    {
        this._repository = new Mock<IBattleOfMonstersRepository>();
    }

    [Fact]
    public async void Get_OnSuccess_ReturnsListOfBattles()
    {
        this._repository
            .Setup(x => x.Battles.GetAllAsync())
            .ReturnsAsync(BattlesFixture.GetBattlesMock());

        BattleController sut = new BattleController(this._repository.Object);
        ActionResult result = await sut.GetAll();
        OkObjectResult objectResults = (OkObjectResult) result;
        objectResults?.Value.Should().BeOfType<Battle[]>();
    }
    
    [Fact]
    public async Task Post_BadRequest_When_StartBattle_With_nullMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        
        Battle b = new Battle()
        {
            MonsterA = null,
            MonsterB = monstersMock[1].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(this._repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult) result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Missing ID", objectResults.Value);
    }
    
    [Fact]
    public async Task Post_OnNoMonsterFound_When_StartBattle_With_NonexistentMonster()
    {
        int? idMock = 5;
        this._repository.Setup(x=>x.Monsters.FindAsync(idMock))
            .ReturnsAsync(() => null);

        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle battle = new Battle()
        {
            MonsterA = idMock,
            MonsterB = monstersMock[1].Id
        };

        BattleController battleController = new BattleController(_repository.Object);
        ActionResult result = await battleController.Add(battle);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning()
    {
       Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle battle = new Battle()
        {
            MonsterA = monstersMock[1].Id,
            MonsterB = monstersMock[0].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(It.IsAny<Battle>())).Verifiable();
        this._repository.Setup(x => x.Save()).Verifiable();

        //monsterA
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterA))
            .ReturnsAsync(() => monstersMock[1]);
        //mosterB
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterB))
            .ReturnsAsync(() => monstersMock[0]);

        BattleController battleController = new BattleController(_repository.Object);
        ActionResult result = await battleController.Add(battle);

        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult objectResult = (OkObjectResult) result;
        objectResult.Value.As<Battle>().Winner.Should().Be(battle.MonsterA);
    }


    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle battle = new Battle()
        {
            MonsterA = monstersMock[5].Id,
            MonsterB = monstersMock[6].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(It.IsAny<Battle>())).Verifiable();
        this._repository.Setup(x => x.Save()).Verifiable();

        //monsterA
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterA))
            .ReturnsAsync(() => monstersMock[5]);
        //mosterB
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterB))
            .ReturnsAsync(() => monstersMock[6]);

        BattleController battleController = new BattleController(_repository.Object);
        ActionResult result = await battleController.Add(battle);

        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult objectResult = (OkObjectResult)result;
        objectResult.Value.As<Battle>().Winner.Should().Be(battle.MonsterB);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirSpeedsSame_And_MonsterA_Has_Higher_Attack()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle battle = new Battle()
        {
            MonsterA = monstersMock[0].Id,
            MonsterB = monstersMock[5].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(It.IsAny<Battle>())).Verifiable();
        this._repository.Setup(x => x.Save()).Verifiable();

        //monsterA
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterA))
            .ReturnsAsync(() => monstersMock[0]);
        //mosterB
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterB))
            .ReturnsAsync(() => monstersMock[5]);

        BattleController battleController = new BattleController(_repository.Object);
        ActionResult result = await battleController.Add(battle);

        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult objectResult = (OkObjectResult)result;
        objectResult.Value.As<Battle>().Winner.Should().Be(battle.MonsterA);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning_When_TheirSpeedsSame_And_MonsterB_Has_Higher_Attack()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle battle = new Battle()
        {
            MonsterA = monstersMock[5].Id,
            MonsterB = monstersMock[0].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(It.IsAny<Battle>())).Verifiable();
        this._repository.Setup(x => x.Save()).Verifiable();

        //monsterA
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterA))
            .ReturnsAsync(() => monstersMock[5]);
        //mosterB
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterB))
            .ReturnsAsync(() => monstersMock[0]);

        BattleController battleController = new BattleController(_repository.Object);
        ActionResult result = await battleController.Add(battle);

        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult objectResult = (OkObjectResult)result;
        objectResult.Value.As<Battle>().Winner.Should().Be(battle.MonsterB);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirDefensesSame_And_MonsterA_Has_Higher_Speed()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();

        Battle battle = new Battle()
        {
            MonsterA = monstersMock[1].Id,
            MonsterB = monstersMock[0].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(It.IsAny<Battle>())).Verifiable();
        this._repository.Setup(x => x.Save()).Verifiable();

        //monsterA
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterA))
            .ReturnsAsync(() => monstersMock[1]);
        //mosterB
        this._repository
            .Setup(x => x.Monsters.FindAsync(battle.MonsterB))
            .ReturnsAsync(() => monstersMock[0]);

        BattleController battleController = new BattleController(_repository.Object);
        ActionResult result = await battleController.Add(battle);

        result.Should().BeOfType<OkObjectResult>();
        OkObjectResult objectResult = (OkObjectResult)result;
        objectResult.Value.As<Battle>().Winner.Should().Be(battle.MonsterA);
    }

    [Fact]
    public async Task Delete_OnSuccess_RemoveBattle()
    {
        int idDelete = 13;
        this._repository
            .Setup(x => x.Battles.RemoveAsync(It.IsAny<int>()))
            .Verifiable();

        BattleController battle = new BattleController(_repository.Object);
        ActionResult result = await battle.Remove(idDelete);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_OnNoBattleFound_Returns404()
    {
        int idDelete = 999;
        this._repository
            .Setup(x => x.Battles.RemoveAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception());

        BattleController battle = new BattleController(_repository.Object);
        ActionResult result = await battle.Remove(idDelete);
        result.Should().BeOfType<NotFoundResult>();
    }
}
