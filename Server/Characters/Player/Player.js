var shortID = require('shortid');
var Vector3 = require('../../utils/MyVector3')
var Vector2 = require('../../utils/MyVector2')
var SpellDot = require('../SpellDot')
//var SpellCooldown = require('../SpellCooldown')


module.exports = class Player {
    constructor(username, baseCharacteristic, spells) {
        this.id = shortID.generate();
        this.username = username;

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

        //Pos
        this.positionInWorld = new Vector3();
        this.positionArrayMain = new Vector2();
        this.positionArrayFight = new Vector2(Math.floor(Math.random() * Math.floor(10)), Math.floor(Math.random() * Math.floor(9)));

        //Other
        this.isInBattle = false;
    }

    IsDead() {
        return this.currentHealth <= 0;
    }

    GetBaseCaracteristicAsJson() {
        let mydata = '{' +
            '"id" : "' + this.id + '",' +
            '"currentHealth" : ' + this.currentHealth + ',' +
            '"currentShield" : ' + this.currentShield + ',' +
            '"currentActionPoints" : ' + this.currentActionPoints + ',' +
            '"currentMovementPoints" : ' + this.currentMovementPoints +// ',' +
            //'"spellsCooldown" : ' + this.spellsCooldown +
            '}';
        return JSON.parse(mydata);
    }

    RefreshActionAndMovementPoint() {
        this.currentActionPoints = this.actionPoints;
        this.currentMovementPoints = this.movementPoints;
    }

    TakeDamage(damage, callbackFunction) {
        this.currentShield -= damage;
        if (this.currentShield < 0) {
            this.currentHealthPoints -= Math.abs(this.currentShield);
            this.currentShield = 0;

            if (this.currentHealthPoints <= 0) {
                this.currentHealthPoints = 0;
                callbackFunction();
            }
        }

    }

    GainHealthPoints(hp = -1) {
        if (hp = -1) {
            this.currentHealthPoints = this.healthPoints;
        } else {
            if (this.currentHealthPoints + hp > this.healthPoints) {
                this.currentHealthPoints = this.healthPoints;
            } else {
                this.currentHealthPoints += hp;
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

    RefreshTimeInTurn() {
        this.actualTimeInTurn = this.timeInTurn;
    }

    IsTimeUp() {
        return this.actualTimeInTurn <= 0;
    }
}
