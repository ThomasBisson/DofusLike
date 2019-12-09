var shortID = require('shortid');
var Vector3 = require('../utils/MyVector3')
var Vector2 = require('../utils/MyVector2')
var SpellDot = require('./SpellDot')

var DQN = require('../AI/DQN')

module.exports = class Character {
    constructor(baseCharacteristic, spells, isAI = false, isTrainable=false) {
        //ID
        this.id = shortID.generate();

        //Charac
        this.baseCharacteristic = baseCharacteristic;
        this.aditionnalHealth = 0;
        this.aditionnalFireIntel = 0;
        this.aditionnalWaterLuck = 0;
        this.aditionnalEarthStrength = 0;
        this.aditionnalWindAgility = 0;
        this.health = this.baseCharacteristic.baseHealthPoints + this.aditionnalHealth;
        this.currentHealth = this.health;
        this.currentShield = 0;
        this.actionPoints = this.baseCharacteristic.baseActionPoints;
        this.currentActionPoints = this.actionPoints;
        this.movementPoints = this.baseCharacteristic.baseMovementPoints;
        this.currentMovementPoints = this.movementPoints;
        
        //Spells management
        this.spells = spells;
        this.spellsDot = [];
        
        this.spellsCooldown = {};
        for (var spell in this.spells) {
            this.spellsCooldown[this.spells[spell]._id] = 0;
        }

        //Battles
        this.timeInTurn = 15;
        this.actualTimeinTurn = this.timeInTurn;
        this.isInBattle = false;

        //Pos
        this.positionArrayFight = new Vector2(Math.floor(Math.random() * Math.floor(10)), Math.floor(Math.random() * Math.floor(9)));

        //AI
        this.isAI = isAI;
        this.isTrainable = isTrainable;
        this.dqn = new DQN();
    }

//#region Base_charact_handling

    IsDead() {
        return this.currentHealth <= 0;
    }

    RefreshActionAndMovementPoint() {
        this.currentActionPoints = this.actionPoints;
        this.currentMovementPoints = this.movementPoints;
    }

    TakeDamage(damage, callbackFunction) {
        this.currentShield -= damage;
        if (this.currentShield < 0) {
            this.currentHealth -= Math.abs(this.currentShield);
            this.currentShield = 0;

            if (this.currentHealth <= 0) {
                this.currentHealth = 0;
                callbackFunction();
            }
        }

    }

    GainHealthPoints(hp = -1) {
        if (hp = -1) {
            this.currentHealth = this.health;
        } else {
            if (this.currentHealth + hp > this.health) {
                this.currentHealth = this.health;
            } else {
                this.currentHealth += hp;
            }
        }
    }

    GainShieldPoints(shield) {
        this.currentShield += shield;
    }

    UseActionPoint(pa) {
        if (this.currentActionPoints >= pa) {
            this.currentActionPoints -= pa;
            return true;
        } else {
            return false;
        }
    }

    GainActionPoint(pa) {
        this.currentActionPoints += pa;
    }

    UseMovementPoint(pm) {
        if (this.currentMovementPoints >= pm) {
            this.currentMovementPoints -= pm;
            return true;
        } else {
            return false;
        }
    }

    GainMovementPoint(pm) {
        this.currentMovementPoints += pm;
    }

    GainCooldown(spellID) {
        if(this.spellsCooldown[spellID] <= 0) {
            this.spellsCooldown[spellID] = this.spells[spellID].cooldown;
            return true;
        }
        return false;
    }

    ReduceAllCooldown() {
        for(let key in this.spellsCooldown) {
            if(this.spellsCooldown[key] > 0) {
                this.spellsCooldown[key] -= 1;
            }
        }
    }

//#endregion Base_charact_handling

//#region Time_Management

    RefreshTimeInTurn() {
        this.actualTimeInTurn = this.timeInTurn;
    }

    IsTimeUp() {
        return this.actualTimeInTurn <= 0;
    }

//#endregion Time_Management

    //#region AI

    PickAction(ennemySt) {
        if(this.isAI) {
            let st = [this.baseCharacteristic.type, this.currentHealth, this.currentActionPoints, this.currentMovementPoints, this.positionArrayFight.x, this.positionArrayFight.y,
            ennemySt[0], ennemySt[1], ennemySt[2], ennemySt[3], ennemySt[4], ennemySt[5]];
            //{pm : X, pa : X, map : {x : X, y : X}, pos : {x : X, y : X}, spells : [{...}, {...}, ...], actions : ["move", "spell", "endTurn"]}
            let data = {
                pm : this.currentMovementPoints,
                pa : this.currentActionPoints,
                map : {
                    x : 15,
                    y : 15
                },
                pos : {
                    x : this.positionArrayFight.x,
                    y : this.positionArrayFight.y
                },
                spells : this.spells,
                spellKeys : Object.keys(this.spells),
                actions : [
                    "move",
                    "spell",
                    "endTurn"
                ]
            }
            console.log(data);
            let act = this.dqn.PickAction(st, data);
            return act;
        }
        return null;
    }

    //#endregion AI

}