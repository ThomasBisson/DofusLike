var Player = require('../Characters/Player/Player');
var PlayerGroup = require('../Characters/Player/PlayerGroup')
var Spell = require('../Characters/Spell.js');
var EnnemyGroup = require('../Characters/Ennemy/EnnemyGroup.js');
var MyMongoBDD = require('../Database/MyMongoBDD.js');
var Map = require('./Map')

var MyVector2 = require('../utils/MyVector2.js');

var TimeUtils = require('../utils/UtilsTime');

module.exports = class Environment {

    constructor() {
        this.map = new Map([15, 15]);

        this.players = [];
        this.monsterGroups = [];

        this.playerGroups = [];

        this.db = new MyMongoBDD();

        this.stopCombat = false;
        this.mustEndTurn = false;
        this.idCurrentTurn;
    }

    /**
     * 
     * @param {*} isPlayer : must search in players array or monster group array
     * @param {*} id : the id of the player or monster group
     * @param {*} idMonster : the possible id of the monster in the group
     * @param {*} act : the action like {action : "move/useSpell/endTurn", move : {x : 3, y : -1}, spell : { spellID : "XXXXXX", x : 15, y : 4}}
     */
    async step(isPlayer, id, idMonster, act) {

    }

    GetState(playerID, ennemyGroupID) {
        //TODO : Adapt to ennemy group and players group
        return [this.players[playerID].baseCharacteristic.type, this.players[playerID].currentHealth, this.players[playerID].currentActionPoints, this.players[playerID].currentMovementPoints,
            this.players[playerID].positionArrayFight.x, this.players[playerID].positionArrayFight.y, 
            this.monsterGroups[ennemyGroupID].monsters[0].baseCharacteristic.type, this.monsterGroups[ennemyGroupID].monsters[0].currentHealth, this.monsterGroups[ennemyGroupID].monsters[0].currentActionPoints, this.monsterGroups[ennemyGroupID].monsters[0].currentMovementPoints,
            this.monsterGroups[ennemyGroupID].monsters[0].positionArrayFight.x, this.monsterGroups[ennemyGroupID].monsters[0].positionArrayFight.y];
    }

    async ConnectToDB() {
        await this.db.connection();
    }

    async AddPlayer(login, password, isAI = false, isTrainable=false) {
        let playerInDB = await this.db.getPlayer(login, password);
        if (playerInDB == null) {
            return null;
        }
        let character = await this.db.getCharacter(playerInDB.baseCharacter);
        let spells = await this.db.getSpells(character.spells);
        let player = new Player('Thomas', character, spells, isAI, isTrainable);
        this.players[player.id] = player;
        //        return this.players[player.id];
        return player.id;
    }

    async AddMonsterGroup(isAI = false, isTrainable=false) {
        //Create a new group of ennemies
        let monsterGroup = new EnnemyGroup(isAI, isTrainable);

        await monsterGroup.FillMonster(this.db, 1);

        this.monsterGroups[monsterGroup.id] = monsterGroup;

        return monsterGroup.id;
    }

    //#region IO

    UpdatePosition(data, playerID) {
        let positionJson = JSON.parse(data);

        this.players[playerID].positionInWorld.x = positionJson.positionInWorld.x;
        this.players[playerID].positionInWorld.y = positionJson.positionInWorld.y;
        this.players[playerID].positionInWorld.z = positionJson.positionInWorld.z;

        this.players[playerID].positionArrayMain.x = positionJson.positionArrayMain.x;
        this.players[playerID].positionArrayMain.y = positionJson.positionArrayMain.y;

        //player.positionArrayFight.x = positionJson.positionArrayFight.x;
        //player.positionArrayFight.y = positionJson.positionArrayFight.y;

        let pos = {
            id: playerID,
            position: {
                x: this.players[playerID].positionInWorld.x,
                y: this.players[playerID].positionInWorld.y,
                z: this.players[playerID].positionInWorld.x,
            }
        }

        return pos;
    }

    EngageBattle(data, playerID) {
        let monsterGroupJson = JSON.parse(data);

        if ((monsterGroupJson.id in this.monsterGroups)) {
            if (this.monsterGroups[monsterGroupJson.id].isInBattle == false) {
                if (this.monsterGroups[monsterGroupJson.id].position.Distance(this.players[playerID].positionInWorld) <= 1) {
                    
                    //Create a group of player
                    let playerGroup = new PlayerGroup();
                    playerGroup.AddPlayer(this.players[playerID]);
                    this.playerGroups[playerGroup.id] = playerGroup;

                    this.players[playerID].isInBattle = true;

                    //TODO : Delete the player in broadcast

                    this.monsterGroups[monsterGroupJson.id].isInBattle = true;
                    return [true, playerGroup.id, monsterGroupJson.id];
                }
            }
        }
        return [false, "", ""];
    }

    async StartBattle(playerGroupID, idEnnemyGroupInBattle, updateStateCallback) {
        let everyone = [];
        //everyone.push(this.players[playerID]);
        
        for(let key in this.playerGroups[playerGroupID].players) {
            everyone.push(this.playerGroups[playerGroupID].players[key]);
        }

        for(let key in this.monsterGroups[idEnnemyGroupInBattle].monsters) {
            everyone.push(this.monsterGroups[idEnnemyGroupInBattle].monsters[key]);
        }
        // for (let i = 0; i < this.monsterGroups[idEnnemyGroupInBattle].monsters.length; i++) {
        //     everyone.push(this.monsterGroups[idEnnemyGroupInBattle].monsters[i]);
        // };
        
        let currentTurn = 0;
        this.idCurrentTurn = everyone[currentTurn].id;

        while (!this.stopCombat) {

            //Refresh time and stats
            everyone[currentTurn].RefreshActionAndMovementPoint();
            everyone[currentTurn].RefreshTimeInTurn();

            //each second
            while (!this.stopCombat && !this.mustEndTurn && !everyone[currentTurn].IsTimeUp()) {

                everyone[currentTurn].actualTimeInTurn -= 1;
                updateStateCallback(everyone[currentTurn]);

                await TimeUtils.sleepTurn(1000);
            }
            //Reduce cooldown
            everyone[currentTurn].ReduceAllCooldown();

            //If end turn button was activated
            if (this.mustEndTurn) {
                this.mustEndTurn = false;
                everyone[currentTurn].actualTimeInTurn = 0;
            }

            //next turn
            currentTurn = (currentTurn >= (everyone.length - 1) ? 0 : currentTurn + 1);
            this.idCurrentTurn = everyone[currentTurn].id;
        }
        this.stopCombat = false;
    }

    async TryToHitSpell(data, playerID, idEnnemyGroupInBattle, callbacks) {
        let myjson = JSON.parse(data);
        let spell = await this.db.getSpell(myjson.spellID);

        // ???
        let spellvars = new Spell();
        spellvars.FillSpell(spell);

        //Is range Ok
        let XY = new MyVector2(myjson.posXY.x, myjson.posXY.y);
        let dist = this.players[playerID].positionArrayFight.CircleDistance(XY);
        if (dist <= spell['range']) {
            if (this.players[playerID].UseActionPoint(spell['actionPointsConsuption'])) {
                if (this.players[playerID].GainCooldown(spell['_id'])) {

                    //Check monsters
                    for(let key in this.monsterGroups[idEnnemyGroupInBattle].monsters) {
                        if (XY.CircleDistance(this.monsterGroups[idEnnemyGroupInBattle].monsters[key].positionArrayFight) <= spellvars.areaRange) {
                            //TODO : See if I can't also put this line at the end of each turn (think about poison)
                            this.monsterGroups[idEnnemyGroupInBattle].monsters[key].TakeDamage(spellvars.damage, function () {});

                            //Send message
                            callbacks(['UpdateCharacterStats', this.monsterGroups[idEnnemyGroupInBattle].monsters[key]]);
                        }
                    }

                    // //Check monsters
                    // this.monsterGroups[idEnnemyGroupInBattle].monsters.forEach(function (monster) {
                    //     //If spell has hit
                    //     if (XY.CircleDistance(monster.positionArrayFight) <= spellvars.areaRange) {
                    //         //TODO : See if I can't also put this line at the end of each turn (think about poison)
                    //         monster.TakeDamage(spellvars.damage, function () {});

                    //         //Send message
                    //         callbacks(['UpdateCharacterStats', monster]);
                    //     }
                    // });

                    //check players
                    if (XY.CircleDistance(this.players[playerID].positionArrayFight) <= spellvars.areaRange) {
                        this.players[playerID].TakeDamage(spellvars.damage, CheckIfAllPlayersDied(this.players[playerID]));
                        this.players[playerID].GainShieldPoints(spellvars.shield);
                    }

                    //Send message
                    callbacks(['UpdateCharacterStats', this.players[playerID]]);

                    if (CheckIfAllEnnemiesAreDead(this.monsterGroups[idEnnemyGroupInBattle])) {
                        //Stop the fight
                        this.stopCombat = true;

                        //End the fight
                        let endFightData = {
                            monsterGroupID: idEnnemyGroupInBattle,
                            positionArrayMain: {
                                x: this.players[playerID].positionArrayMain.x,
                                y: this.players[playerID].positionArrayMain.y
                            }
                        }
                        callbacks(['EndFight', endFightData]);
                        delete this.monsterGroups[idEnnemyGroupInBattle];
                    }
                    else {
                        let myjsonSellHit = {
                            spellID: myjson.spellID,
                            startPosition: {
                                x: this.players[playerID].positionArrayFight.x,
                                y: this.players[playerID].positionArrayFight.y
                            },
                            endPosition: {
                                x: XY.x,
                                y: XY.y
                            }
                        }

                        //Make the spell display itself
                        callbacks(['SpellHasHit', myjsonSellHit]);
                    }
                }
            }
        }
    }

    TryToMove(data, playerID, callbacks) {
        let jsonreceived = JSON.parse(data);
        let XY = new MyVector2(jsonreceived.posInBattle.x, jsonreceived.posInBattle.y);
        if (this.players[playerID].UseMovementPoint(this.players[playerID].positionArrayFight.CircleDistance(XY))) {
            this.players[playerID].positionArrayFight = XY;

            callbacks(['UpdateCharacterStats', this.players[playerID]]);

            let myjson = {
                position: {
                    x: this.players[playerID].positionArrayFight.x,
                    y: this.players[playerID].positionArrayFight.y,
                }
            }

            callbacks(['NewDestination', myjson]);
        }
    }

    EndTurn(playerID) {
        if (this.idCurrentTurn == playerID) {
            this.mustEndTurn = true;
        }
    }

    //#endregion
}

//#region Battle

function CheckIfAllEnnemiesAreDead(monsterGroup) {
    let allDead = true;
    for(let key in monsterGroup.monsters) {
        if (!monsterGroup.monsters[key].IsDead()) {
            allDead = false;
        }
    }
    // monsterGroup.monsters.forEach(function (monster) {
    //     if (!monster.IsDead()) {
    //         allDead = false;
    //     }
    // });
    return allDead;
};

function CheckIfAllPlayersDied(player) {
    if (player.IsDead()) {
        //End the fight
        console.log("Player dead");
    }
};

//#endregion