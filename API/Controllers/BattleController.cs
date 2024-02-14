using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Lib.Repository.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class BattleController : BaseApiController
{
    private readonly IBattleOfMonstersRepository _repository;

    public BattleController(IBattleOfMonstersRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        IEnumerable<Battle> battles = await _repository.Battles.GetAllAsync();
        return Ok(battles);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Add([FromBody] Battle battle)
    {
        try
        {
            if (battle.MonsterA is null || battle.MonsterB is null)
                throw new Exception("Missing ID");

            var battleWinner = await StartBattle(battle);
            if (battleWinner is null)
                return NotFound();

            await _repository.Battles.AddAsync(battleWinner);
            await _repository.Save();

            return Ok(battleWinner);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<Battle> StartBattle(Battle battle)
    {
        var monsterA = await _repository.Monsters.FindAsync(battle.MonsterA);
        var monsterB = await _repository.Monsters.FindAsync(battle.MonsterB);

        if (monsterA is null || monsterB is null)
            return null;

        var firstAttacker = FindFirstMonsterToAttack(monsterA, monsterB);
        var secondAttacker = firstAttacker.Id == monsterA.Id ? monsterB : monsterA;

        var winner = Fight(firstAttacker, secondAttacker);
        battle.Winner = winner.Id;
        return battle;
    }

    private Monster Fight(Monster firstAttacker, Monster secondAttacker)
    {
        while (secondAttacker.Hp > 0 || firstAttacker.Hp > 0)
        {
            var damage1 = CalculateDamage(firstAttacker.Attack, secondAttacker.Defense);
            secondAttacker.Hp -= damage1;

            var damage2 = CalculateDamage(secondAttacker.Attack, firstAttacker.Defense);
            firstAttacker.Hp -= damage2;

            if(firstAttacker.Hp <= 0 || secondAttacker.Hp <=0)
                break;
        }
        return firstAttacker.Hp > 0? firstAttacker : secondAttacker;
    }

    private int CalculateDamage(int attack, int defense)
    {
        if (attack <= defense) return 0;
        return attack - defense;
    }

    private Monster FindFirstMonsterToAttack(Monster monsterA, Monster monsterB)
    {
        if(monsterA.Speed == monsterB.Speed)
            return GetMonsterHigherAttack(monsterA, monsterB);

        return GetMonsterHigherSpeed(monsterA, monsterB);
    }

    private Monster GetMonsterHigherSpeed(Monster monsterA, Monster monsterB)
    {
        return monsterA.Speed > monsterB.Speed ? monsterA : monsterB;
    }

    private Monster GetMonsterHigherAttack(Monster monsterA, Monster monsterB)
    {
        return monsterA.Attack > monsterB.Attack ? monsterA : monsterB;
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Remove(int id)
    {
        try
        {
            await _repository.Battles.RemoveAsync(id);
            await _repository.Save();
            return Ok();
        }
        catch
        {
            return NotFound();
        }
    }
}


